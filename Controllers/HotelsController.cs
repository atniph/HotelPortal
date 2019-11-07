using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelPortal.Models;
using HotelPortal.Models.HotelViewModels;
using Microsoft.AspNetCore.Http;
using HotelPortal.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace HotelPortal.Controllers
{
    public class HotelsController : BaseController
    {
        public HotelsController(IPortalService _portalService) : base(_portalService)
        {
        }

        /// <summary>
        /// Lists all of the hotels in the database.
        /// </summary>
        /// <returns>A view with all the hotels.</returns>
        public IActionResult Index()
        {
            return View("Index", _portalService.Hotels);
        }

        /*        public IActionResult Search(int cityId, int stars, int? min, int? max, int? guest, DateTime? from, DateTime? to)
                {
                    return View("Index", _portalService.SearchHotel(cityId,stars,min,max,guest,from,to));
                }*/

        /// <summary>
        /// Detailed view of a hotel, with its rooms, images, and reviews.
        /// </summary>
        /// <param name="id">Id of the hotel.</param>
        /// <returns>A detailed view of the hotel.</returns>
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = _portalService.GetHotel(id);
            if (hotel == null)
            {
                return NotFound();
            }
            ViewData["ImageIds"] = _portalService.GetHotelImageIds(id);
            ViewData["Reviews"] = _portalService.GetReviews(id);
            return View(hotel);
        }

        /// <summary>
        /// Admins can add new hotels to the database.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_portalService.Cities, "Id", "Name");
            ViewData["Stars"] = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            return View();
        }

        // POST: Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel hotel)
        {
            if (ModelState.IsValid)
            {
                var id = _portalService.CreateHotelAsync(hotel).Result;
                if (hotel.Images != null)
                {
                    await UploadImagesAsync(hotel.Images, id);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        /// <summary>
        /// Validates and saves all of the uploaded images to the database.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task UploadImagesAsync(List<IFormFile> images, int id)
        {
            var theMain = true;
            foreach (IFormFile image in images)
            {
                IFormFile a = image;
                var formFileContent =
                await FileHelper.ProcessFormFile<HotelViewModel>(
                image, ModelState, _permittedExtensions,
                _fileSizeLimit);

                var file = new HotelImage()
                {
                    Content = formFileContent,
                    UntrustedName = image.FileName,
                    HotelId = id,
                    IsMain = theMain
                };
                await _portalService.CreateHotelImage(file);
                theMain = false;
            }
        }

        /// <summary>
        /// Get the main image for the Hotel.
        /// </summary>
        /// <param name="parentId">Id of the given hotel.</param>
        /// <returns>The main image of the hotel</returns>
        [ResponseCache(VaryByHeader = "*", Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public FileResult HotelMainImage(int? parentId)
        {
            Byte[] imageContent = _portalService.GetMainImage(parentId);

            if (imageContent == null)
                return File("~/images/no_image.jpg", "image/jpg");

            return File(imageContent, "image/jpg");
        }

        /// <summary>
        /// Get an image for the Hotel.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns>An image of the hotel</returns>
        public FileResult HotelImage(int? imageId)
        {
            Byte[] imageContent = _portalService.GetHotelImage(imageId);

            if (imageContent == null)
                return File("~/images/no_image.jpg", "image/jpg");

            return File(imageContent, "image/jpg");
        }

        /// <summary>
        /// Deletes the chosen image, only for Admins.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveHotelImage(int? imageId)
        {
            await _portalService.RemoveHotelImage(imageId);
            return Redirect(Request.Headers["Referer"]);
        }

        /// <summary>
        /// Admins can edit the hotel, add rooms, etc..
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HotelEditViewModel hotelEditViewModel = _portalService.PrepareEditHotel(id);

            if (hotelEditViewModel == null)
            {
                return NotFound();
            }

            ViewData["CityId"] = new SelectList(_portalService.Cities, "Id", "Name");
            ViewData["Stars"] = new SelectList(new List<int> { 1, 2, 3, 4, 5 });
            ViewData["ImageIds"] = _portalService.GetHotelImageIds(id).ToList();
            return View(hotelEditViewModel);
        }

        // POST: Hotels/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HotelEditViewModel hotel)
        {
            if (ModelState.IsValid)
            {
                await _portalService.EditHotelAsync(hotel);
                if (hotel.Images != null)
                {
                    await UploadImagesAsync(hotel.Images, hotel.Id);
                }

                return RedirectToAction("Details", new { id = hotel.Id });
            }
            return View(hotel);
        }

        /// <summary>
        /// Admins can delete the chosen hotel.
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

            var hotel = _portalService.GetHotel(id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _portalService.DeleteHotel(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
