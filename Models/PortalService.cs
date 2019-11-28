using HotelPortal.Data;
using HotelPortal.Models.BookingViewModels;
using HotelPortal.Models.HotelViewModels;
using HotelPortal.Models.RoomViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPortal.Models
{
    public class PortalService : IPortalService
    {
        private readonly HotelPortalContext _context;
        private readonly UserManager<SiteUser> _userManager;


        public PortalService(HotelPortalContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IEnumerable<Hotel> Hotels => _context.Hotels.Include(h => h.City);
        public IEnumerable<City> Cities => _context.Cities;
        public IEnumerable<Booking> Bookings => _context.Bookings.Include(b => b.SiteUser).Include(b => b.Room).ThenInclude(h => h.Hotel);

        public IEnumerable<Review> Reviews => _context.Reviews.Include(r => r.Hotel).Include(u => u.SiteUser);

        /*        public IEnumerable<Hotel> SearchHotel(int cityId, int star, int? min, int? max, int? guest, DateTime? from, DateTime? to)
                {
                    if (from == null)
                        from = DateTime.Now;
                    var booked_rooms = _context.Bookings.Where(b => b.EndDate >= from).Select(b => b.RoomId).Distinct();

                    return _context.Hotels
                         .Include(b => b.City)
                         .Include(b => b.Rooms)
                         .Where(s => s.Rooms.All(r => !booked_rooms.Contains(r.Id)))
                         .Where(s => s.CityId == cityId)
                         .Where(s => s.Stars >= star)
                         .Where(s => s.Rooms.All(r => r.Price > min))
                         .Where(s => s.Rooms.All(r => r.Price < max))
                         .Where(s => s.Rooms.All(r => r.Capacity >= guest))
                         .ToList();

                }*/

        public IEnumerable<Room> SearchRoom(int cityId, int star, int min, int? max, int guest, DateTime? from, DateTime? to)
        {
            if (from == null)
                from = DateTime.Now.Date;
            if (max == null)
                max = _context.Rooms.Max(r => r.Price);
            var booked_rooms = _context.Bookings.Where(b => b.EndDate.Date >= from)
                                .Where(b => b.StartDate >= from && b.StartDate < to ||
                                 b.EndDate.Date >= from && b.EndDate.Date <= to ||
                                 b.StartDate.Date <= from && b.EndDate.Date >= to ||
                                 b.StartDate.Date >= from && b.EndDate.Date <= to).Select(b => b.RoomId).Distinct();

            return _context.Rooms
              .Include(r => r.Hotel)
              .Where(r => !booked_rooms.Contains(r.Id))
              .Where(r => r.Hotel.CityId == cityId)
              .Where(r => r.Hotel.Stars >= star)
              .Where(r => r.Price >= min && r.Price <= max)
              .Where(r => r.Capacity >= guest)
              .ToList();
        }

        /// <summary>
        /// Get the hotel with its rooms.
        /// </summary>
        /// <param name="HotelId">Id of the hotel.</param>
        public Hotel GetHotel(int? hotelId)
        {
            if (hotelId == null)
                return null;

            return _context.Hotels
                .Include(b => b.City)
                .Include(b => b.Rooms)
                .FirstOrDefault(Hotel => Hotel.Id == hotelId);
        }

        public async Task DeleteHotel(int? id)
        {
            var hotel = GetHotel(id);
            if (hotel != null)
            {
                foreach (int roomId in GetHotelRoomIds(id))
                {
                    await DeleteRoom(roomId, false);
                }
                _context.Hotels.Remove(hotel);
                var images = _context.HotelImages.Where(a => a.HotelId == id);
                _context.HotelImages.RemoveRange(images);
                var reviews = _context.Reviews.Where(r => r.HotelId == id);
                _context.Reviews.RemoveRange(reviews);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRoom(int? id, Boolean delete)
        {
            var room = GetRoom(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                var images = _context.RoomImages.Where(a => a.RoomId == id);
                _context.RoomImages.RemoveRange(images);
                var bookings = _context.Bookings.Where(b => b.RoomId == id);
                _context.Bookings.RemoveRange(bookings);
                if (delete)
                    await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteReview(int? id)
        {
            var review = GetReview(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public HotelEditViewModel PrepareEditHotel(int? id)
        {
            var hotel = GetHotel(id);
            if (hotel == null)
                return null;

            return new HotelEditViewModel
            {
                Id = hotel.Id,
                CityId = hotel.CityId,
                Description = hotel.Description,
                Latitude = hotel.Latitude,
                Longitude = hotel.Longitude,
                Stars = hotel.Stars,
                Name = hotel.Name
            };
        }

        public RoomEditViewModel PrepareEditRoom(int? id)
        {
            var room = GetRoom(id);
            if (room == null)
                return null;

            return new RoomEditViewModel
            {
                Id = room.Id,
                Capacity = room.Capacity,
                HotelId = room.HotelId,
                Price = room.Price
            };
        }

        public async Task<BookingViewModel> PrepareBookingAsync(int? id, string userName)
        {
            var room = GetRoom(id);
            var user = await _userManager.FindByNameAsync(userName);
            if (room == null || user == null)
                return null;

            BookingViewModel bookingViewModel = new BookingViewModel
            {
                Room = room,
                Address = user.Address,
                Email = user.Email,
                Name = user.Name,
                IDNumber = user.IDNumber,
                FirstDay = DateTime.Today,
                LastDay = DateTime.Today
            };

            return bookingViewModel;

        }

        public async Task<Review> PrepareReviewAsync(int? id, string userName)
        {
            var hotel = GetHotel(id);
            var user = await _userManager.FindByNameAsync(userName);
            if (hotel == null || user == null)
                return null;

            Review review = new Review
            {
                Hotel = hotel,
                SiteUser = user
            };

            return review;
        }

        public async Task<bool> CreateBookingAsync(int? roomId, BookingViewModel bookingViewModel)
        {
            if (roomId == null || bookingViewModel == null)
                return false;
            var user = await _userManager.FindByEmailAsync(bookingViewModel.Email);

            if (user == null)
                return false;

            if (bookingViewModel.FirstDay.Date < DateTime.Now.Date || bookingViewModel.LastDay.Date <= bookingViewModel.FirstDay.Date)
                return false;

            if (_context.Bookings.Where(b => b.RoomId == roomId && b.EndDate >= bookingViewModel.FirstDay)
                .Any(b => b.StartDate >= bookingViewModel.FirstDay && b.StartDate < bookingViewModel.LastDay ||
                     b.EndDate >= bookingViewModel.FirstDay && b.EndDate < bookingViewModel.LastDay ||
                     b.StartDate < bookingViewModel.FirstDay && b.EndDate > bookingViewModel.LastDay ||
                     b.StartDate > bookingViewModel.FirstDay && b.EndDate < bookingViewModel.LastDay))
                return false;

            _context.Bookings.Add(new Booking
            {
                RoomId = roomId.Value,
                SiteUserId = user.Id,
                StartDate = bookingViewModel.FirstDay,
                EndDate = bookingViewModel.LastDay,
                GuestCount = bookingViewModel.GuestCount
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Boolean> CreateReviewAsync(int? hotelId, Review review)
        {
            if (hotelId == null || review == null)
                return false;
            var user = await _userManager.FindByIdAsync(review.UserId.ToString());

            if (user == null)
                return false;

            _context.Reviews.Add(new Review
            {
                HotelId = review.HotelId,
                Description = review.Description,
                UserId = review.UserId,
                Stars = review.Stars
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Hotel> GetHotels(int? cityId)
        {
            if (cityId == null || !_context.Hotels.Any(Hotel => Hotel.CityId == cityId))
                return null;

            return _context.Hotels.Where(Hotel => Hotel.CityId == cityId);
        }

        public IEnumerable<Review> GetReviews(int? hotelId)
        {
            if (hotelId == null || !_context.Reviews.Any(r => r.HotelId == hotelId))
                return new List<Review>();

            return _context.Reviews.Where(r => r.HotelId == hotelId);
        }

        /// <summary>
        /// Get the hotel with its rooms.
        /// </summary>
        /// <param name="HotelId">Id of the hotel.</param>
        public Hotel GetHotelWithRooms(int? hotelId)
        {
            if (hotelId == null)
                return null;

            return _context.Hotels
                .Include(b => b.City)
                .Include(b => b.Rooms)
                .FirstOrDefault(b => b.Id == hotelId);
        }

        public async Task<int> CreateHotelAsync(HotelViewModel hotel)
        {
            Hotel newHotel = new Hotel
            {
                CityId = hotel.CityId,
                Name = hotel.Name,
                Description = hotel.Description,
                Longitude = hotel.Longitude,
                Latitude = hotel.Latitude,
                Stars = hotel.Stars

            };
            _context.Hotels.Add(newHotel);
            await _context.SaveChangesAsync();
            return newHotel.Id;
        }

        public async Task<int> CreateRoomAsync(RoomViewModel room, int? hotelId)
        {
            Room newRoom = new Room
            {
                HotelId = hotelId.Value,
                Capacity = room.Capacity,
                Price = room.Price

            };
            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();
            return newRoom.Id;
        }

        public async Task CreateHotelImage(HotelImage hotelImage)
        {
            _context.HotelImages.Add(new HotelImage
            {
                Content = hotelImage.Content,
                UntrustedName = hotelImage.UntrustedName,
                HotelId = hotelImage.HotelId,
                IsMain = hotelImage.IsMain
            });
            await _context.SaveChangesAsync();
        }

        public async Task CreateRoomImage(RoomImage roomImage)
        {
            _context.RoomImages.Add(new RoomImage
            {
                Content = roomImage.Content,
                UntrustedName = roomImage.UntrustedName,
                RoomId = roomImage.RoomId,
                IsMain = roomImage.IsMain
            });
            await _context.SaveChangesAsync();
        }

        public async Task RemoveHotelImage(int? imageId)
        {
            var RemoveFile = await _context.HotelImages.FindAsync(imageId);

            if (RemoveFile != null)
            {
                _context.HotelImages.Remove(RemoveFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRoomImage(int? imageId)
        {
            var removeFile = await _context.RoomImages.FindAsync(imageId);

            if (removeFile != null)
            {
                _context.RoomImages.Remove(removeFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveBooking(int? id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        //TODO isMain??
        public Byte[] GetMainImage(int? parentId)
        {
            if (parentId == null)
                return null;

            return _context.HotelImages
                .Where(image => image.HotelId == parentId)
                .Select(image => image.Content)
                .FirstOrDefault();
        }

        public Byte[] GetRoomMainImage(int? parentId)
        {
            if (parentId == null)
                return null;

            return _context.RoomImages
                .Where(image => image.RoomId == parentId)
                .Select(image => image.Content)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get all the image ids that belong to the given hotel.
        /// </summary>
        /// <param name="HotelId">Id of the hotel.</param>
        public IEnumerable<int> GetHotelImageIds(int? hotelId)
        {
            if (hotelId == null)
                return null;

            return _context.HotelImages
                .Where(image => image.HotelId == hotelId)
                .Select(image => image.Id);
        }

        public IEnumerable<int> GetHotelReviewIds(int? hotelId)
        {
            if (hotelId == null)
                return null;

            return _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .Select(r => r.Id);
        }

        public IEnumerable<int> GetRoomImageIds(int? roomId)
        {
            if (roomId == null)
                return null;

            return _context.RoomImages
                .Where(image => image.RoomId == roomId)
                .Select(image => image.Id);
        }

        public IEnumerable<int> GetHotelRoomIds(int? hotelId)
        {
            if (hotelId == null)
                return null;

            return _context.Rooms
                .Where(room => room.HotelId == hotelId)
                .Select(room => room.Id);
        }

        public async Task EditHotelAsync(HotelEditViewModel hotel)
        {
            Hotel originalHotel = GetHotel(hotel.Id);
            originalHotel.Latitude = hotel.Latitude;
            originalHotel.Longitude = hotel.Longitude;
            originalHotel.Name = hotel.Name;
            originalHotel.Stars = hotel.Stars;
            originalHotel.Description = hotel.Description;
            originalHotel.CityId = hotel.CityId;
            _context.Hotels.Update(originalHotel);
            await _context.SaveChangesAsync();
        }

        public async Task EditRoomAsync(RoomEditViewModel room)
        {
            Room originalRoom = GetRoom(room.Id);
            originalRoom.Capacity = room.Capacity;
            originalRoom.Price = room.Price;
            _context.Rooms.Update(originalRoom);
            await _context.SaveChangesAsync();
        }

        public Byte[] GetHotelImage(int? imageId)
        {
            if (imageId == null)
                return null;
            Byte[] imageContent = _context.HotelImages
                .Where(image => image.Id == imageId)
                .Select(image => image.Content)
                .FirstOrDefault();

            return imageContent;
        }


        public Byte[] GetRoomImage(int? imageId)
        {
            if (imageId == null)
                return null;
            Byte[] imageContent = _context.RoomImages
                .Where(image => image.Id == imageId)
                .Select(image => image.Content)
                .FirstOrDefault();

            return imageContent;
        }

        public Room GetRoom(int? roomId)
        {
            if (roomId == null)
                return null;

            return _context.Rooms
                .Include(a => a.Hotel)
                .ThenInclude(b => b.City)
                .FirstOrDefault(Room => Room.Id == roomId);

        }

        public Review GetReview(int? id)
        {
            if (id == null)
                return null;

            return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<int> CreateCapacityList(int? id)
        {
            if (id == null)
                return null;

            List<int> res = new List<int>();
            var cap = _context.Rooms.Where(r => r.Id == id).FirstOrDefault();
            for (int i = 1; i <= cap.Capacity; i++)
                res.Add(i);
            return res;
        }

    }
}
