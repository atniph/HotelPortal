using System.Threading.Tasks;
using HotelPortal.Models;
using HotelPortal.Models.BookingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelPortal.Controllers
{
    public class BookingsController : Controller
    {

        private readonly IPortalService _portalService;

        public BookingsController(IPortalService portalService)
        {
            _portalService = portalService;
        }

        /// <summary>
        /// Lists all the bookings for the admin.
        /// </summary>
        /// <returns>A view of bookings.</returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_portalService.Bookings);
        }

        // GET: Bookings/Details/
        public ActionResult Details(int? id)
        {
            return View();
        }

        /// <summary>
        /// Creates a new booking for the room.
        /// </summary>
        /// <param name="id">Id of the chosen room.</param>
        /// <returns>The viewmodel of the booking.</returns>
        [Authorize(Roles = "Admin,User")]
        public ActionResult Create(int? id)
        {
            ViewData["Capacity"] = new SelectList(_portalService.CreateCapacityList(id));
            return View(_portalService.PrepareBookingAsync(id, User.Identity.Name).Result);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int? id, BookingViewModel bookingViewModel)
        {
            if (id == null || bookingViewModel == null)
                return RedirectToAction("Index", "Bookings");

            if (!await _portalService.CreateBookingAsync(id, bookingViewModel))
                ModelState.AddModelError("LastDay", "The room is already reserved at that day!");

            if (!ModelState.IsValid)
            {
                ViewData["Capacity"] = new SelectList(_portalService.CreateCapacityList(id));
                bookingViewModel.Room = _portalService.GetRoom(id);
                return View("Create", bookingViewModel);
            }

            //TODO: return to the result view of the booking
            return RedirectToAction("Index", "Hotels");
        }

        // GET: Bookings/Edit/
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Bookings/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Deletes the chosen booking.
        /// </summary>
        /// <param name="id">Id of the booking.</param>
        /// <returns>A view with the remaining bookings</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveBooking(int? id)
        {
            await _portalService.RemoveBooking(id);
            return Redirect(Request.Headers["Referer"]);
        }
    }
}