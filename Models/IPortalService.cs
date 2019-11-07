using HotelPortal.Models.BookingViewModels;
using HotelPortal.Models.HotelViewModels;
using HotelPortal.Models.RoomViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelPortal.Models
{
    public interface IPortalService
    {
        /// <summary>
        /// List of cities.
        /// </summary>
        IEnumerable<City> Cities { get; }

        /// <summary>
        /// List of hotels.
        /// </summary>
        IEnumerable<Hotel> Hotels { get; }

        /// <summary>
        /// List of bookings.
        /// </summary>
        IEnumerable<Booking> Bookings { get; }

        /// <summary>
        /// List of reviews.
        /// </summary>
        IEnumerable<Review> Reviews { get; }

        // IEnumerable<Hotel> SearchHotel(int cityId, int star, int? min, int? max, int? guest, DateTime? from, DateTime? to);

        /// <summary>
        /// Lists all the rooms that meets the criteria
        /// </summary>
        /// <param name="cityId">Id of the chosen city</param>
        /// <param name="stars">Minimum number of stars</param>
        /// <param name="min">Cheapest rooms price</param>
        /// <param name="max">Most expensive rooms price</param>
        /// <param name="guest">Number of guests for the room</param>
        /// <param name="from">Date of arrival</param>
        /// <param name="to">Date of leaving</param>
        /// <returns></returns>
        IEnumerable<Room> SearchRoom(int cityId, int star, int min, int? max, int guest, DateTime? from, DateTime? to);


        /// <summary>
        /// Returns chosen hotel.
        /// </summary>
        /// <param name="cityId">Id of the chosen hotel.</param>
        Hotel GetHotel(int? cityId);

        /// <summary>
        /// Lists all the hotels from the chosen city.
        /// </summary>
        /// <param name="cityId">Id of the chosen city.</param>
        IEnumerable<Hotel> GetHotels(int? cityId);

        /// <summary>
        /// Lists all the reviews from the chosen hotel.
        /// </summary>
        /// <param name="hotelId">Id of the chosen hotel.</param>
        IEnumerable<Review> GetReviews(int? hotelId);

        /// <summary>
        /// Get the rooms of the chosen hotel.
        /// </summary>
        /// <param name="hotelId">Id of the chosen hotel</param>
        Hotel GetHotelWithRooms(int? hotelId);

        /// <summary>
        /// Create new hotel in the database.
        /// </summary>
        /// <param name="hotel">The new hotel</param>
        /// <returns>Id of the new hotel.</returns>
        Task<int> CreateHotelAsync(HotelViewModel hotel);

        /// <summary>
        /// Creates a new room in the database.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="hotelId"></param>
        /// <returns>Id of the new room.</returns>
        Task<int> CreateRoomAsync(RoomViewModel room, int? hotelId);

        /// <summary>
        /// Saves the image for the hotel to the database.
        /// </summary>
        /// <param name="hotelImage"></param>
        /// <returns></returns>
        Task CreateHotelImage(HotelImage hotelImage);

        /// <summary>
        /// Saves the image for the room to the database.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        Task CreateRoomImage(RoomImage image);

        /// <summary>
        /// Deletes the image of the hotel from the database.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task RemoveHotelImage(int? imageId);

        /// <summary>
        /// Deletes the image of the room from the database.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task RemoveRoomImage(int? imageId);

        /// <summary>
        /// Deletes the chosen booking.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveBooking(int? id);

        /// <summary>
        /// Prepares the viewmodel for editing a hotel.
        /// </summary>
        /// <param name="id">Id of the hotel</param>
        /// <returns>The viewmodel with the inputs</returns>
        HotelEditViewModel PrepareEditHotel(int? id);

        /// <summary>
        /// Prepares the viewmodel for editing a room.
        /// </summary>
        /// <param name="id">Id of the room</param>
        /// <returns>The viewmodel with the inputs</returns>
        RoomEditViewModel PrepareEditRoom(int? id);

        /// <summary>
        /// Prepares the viewmodel for a booking.
        /// </summary>
        /// <param name="id">Id of the hotel</param>
        /// <returns>The viewmodel with the user data</returns>
        Task<BookingViewModel> PrepareBookingAsync(int? roomId, string userName);

        /// <summary>
        /// Prepares a review for saving
        /// </summary>
        /// <param name="id">Id of the hotel</param>
        /// <returns>The review with the inputs</returns>
        Task<Review> PrepareReviewAsync(int? hotelId, string userName);

        /// <summary>
        /// Validates and saves the booking
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="bookingViewModel"></param>
        /// <returns>True if the saving was successful</returns>
        Task<Boolean> CreateBookingAsync(int? roomId, BookingViewModel bookingViewModel);

        /// <summary>
        /// Saves the review for the hotel
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="review"></param>
        /// <returns>True if the saving was successful</returns>
        Task<Boolean> CreateReviewAsync(int? hotelId, Review review);

        /// <summary>
        /// Returns the main image for the chosen item.
        /// </summary>
        /// <param name="parentId">Id of the item</param>
        /// <returns></returns>
        Byte[] GetMainImage(int? parentId);
        
        /// <summary>
        /// Returns tha main image for the chosen room.
        /// </summary>
        /// <param name="parentId">Id of the room</param>
        /// <returns></returns>
        Byte[] GetRoomMainImage(int? parentId);

        /// <summary>
        /// Updates the hotel with the given informations
        /// </summary>
        /// <param name="hotel"></param>
        /// <returns></returns>
        Task EditHotelAsync(HotelEditViewModel hotel);

        /// <summary>
        /// Updates the room with the given informations
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        Task EditRoomAsync(RoomEditViewModel room);

        /// <summary>
        /// Deletes the chosen hotel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteHotel(int? id);

        /// <summary>
        /// Deletes the chosen room.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        Task DeleteRoom(int? id, Boolean delete);

        /// <summary>
        /// Deletes the chosen review.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteReview(int? id);

        /// <summary>
        /// Returns a list of image ids of the given hotel.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        IEnumerable<int> GetHotelImageIds(int? hotelId);

        /// <summary>
        /// Returns a list of image ids of the given room.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        IEnumerable<int> GetRoomImageIds(int? roomId);

        /// <summary>
        /// Returns a list of review ids of the given hotel.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        IEnumerable<int> GetHotelReviewIds(int? hotelId);

        /// <summary>
        /// Returns the image for the hotel
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Byte[] GetHotelImage(int? imageId);

        /// <summary>
        /// Returns the image for the room
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Byte[] GetRoomImage(int? imageId);

        /// <summary>
        /// Returns the chosen room
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Room GetRoom(int? roomId);

        /// <summary>
        /// Returns the chosen review
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Review GetReview(int? id);

        /// <summary>
        /// Returns a list of integers between 1 and the capacity of the room.
        /// </summary>
        /// <param name="id">Id of the room</param>
        /// <returns></returns>
        IEnumerable<int> CreateCapacityList(int? id);

    }
}
