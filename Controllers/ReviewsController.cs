using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelPortal.Data;
using HotelPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HotelPortal.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IPortalService _portalService;

        public ReviewsController(IPortalService portalService)
        {
            _portalService = portalService;
        }

        /// <summary>
        /// Lists all the reviews of the hotel.
        /// </summary>
        /// <param name="hotelId">Id of the hotel.</param>
        /// <returns>A view with the reviews.</returns>
        public IActionResult List(int? hotelId)
        {
            return View(_portalService.GetReviews(hotelId));
        }

        /*public string GetReview(int? id)
        {
            return _portalService.GetReview(id).Description;
        }*/

        /// <summary>
        /// Registered users can write reviews of the hotels.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create(int? hotelId)
        {
            ViewData["Stars"] = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View(_portalService.PrepareReviewAsync(hotelId, User.Identity.Name).Result);
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HotelId,UserId,Description,Stars")] Review review, int? hotelId)
        {
            if (hotelId == null || review == null)
                return RedirectToAction("Index", "Home");

            await _portalService.CreateReviewAsync(hotelId, review);
            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }

        /// <summary>
        /// Admins can delete reviews.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = _portalService.GetReview(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotelId = _portalService.GetReview(id).HotelId;
            await _portalService.DeleteReview(id);
            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }
    }
}
