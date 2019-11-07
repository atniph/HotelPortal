using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelPortal.Models;
using HotelPortal.Models.RoomViewModels;
using HotelPortal.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelPortal.Controllers
{
    public class RoomsController : BaseController
    {
        public RoomsController(IPortalService _portalService) : base(_portalService)
        {
        }

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
        public IActionResult List(int cityId, int stars, int min, int? max, int guest, DateTime? from, DateTime? to)
        {
            return View("List", _portalService.SearchRoom(cityId, stars, min, max, guest, from, to));
        }

        /// <summary>
        /// Admins can add new rooms for the hotels.
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? hotelId)
        {
            RoomViewModel roomViewModel = new RoomViewModel { HotelId = hotelId.Value };
            return View("Create", roomViewModel);
        }

        // POST: Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel room, int? hotelId)
        {
            if (hotelId == null || room == null)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var id = _portalService.CreateRoomAsync(room, hotelId).Result;
                if (room.Images != null)
                {
                    await UploadImagesAsync(room.Images, id);
                }
                return RedirectToAction("Details", "Hotels", new { id = hotelId });
            }
            return View(room);
        }

        /// <summary>
        /// Details of the room.
        /// </summary>
        /// <param name="id">Id of the room</param>
        /// <returns>Detailed view of the room, with its pictures</returns>
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Room = _portalService.GetRoom(id);
            if (Room == null)
            {
                return NotFound();
            }
            ViewData["ImageIds"] = _portalService.GetRoomImageIds(id).ToList();
            return View(Room);
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
                await FileHelper.ProcessFormFile<RoomViewModel>(
                image, ModelState, _permittedExtensions,
                _fileSizeLimit);

                var file = new RoomImage()
                {
                    Content = formFileContent,
                    UntrustedName = image.FileName,
                    RoomId = id,
                    IsMain = theMain
                };
                await _portalService.CreateRoomImage(file);
                theMain = false;
            }
        }

        /// <summary>
        /// Get the main image for the Room.
        /// </summary>
        /// <param name="parentId">Id of the given Room.</param>
        /// <returns>The main image of the Room</returns>
        [ResponseCache(VaryByHeader = "*", Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public FileResult RoomMainImage(int? parentId)
        {
            Byte[] imageContent = _portalService.GetRoomMainImage(parentId);

            if (imageContent == null)
                return File("~/images/no_image.jpg", "image/jpg");

            return File(imageContent, "image/jpg");
        }

        /// <summary>
        /// Get an image for the Room.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns>An image of the room</returns>
        public FileResult RoomImage(int? imageId)
        {
            Byte[] imageContent = _portalService.GetRoomImage(imageId);

            if (imageContent == null)
                return File("~/images/no_image.jpg", "image/jpg");

            return File(imageContent, "image/jpg");
        }

        /// <summary>
        /// Deletes the chosen image, only for admins.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoomImage(int? imageId)
        {
            await _portalService.RemoveRoomImage(imageId);
            return Redirect(Request.Headers["Referer"]);
        }


        /// <summary>
        /// Admins can edit the chosen room.
        /// </summary>
        /// <param name="id">Id of the room.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoomEditViewModel roomEditViewModel = _portalService.PrepareEditRoom(id);

            if (roomEditViewModel == null)
            {
                return NotFound();
            }
            ViewData["ImageIds"] = _portalService.GetRoomImageIds(id).ToList();
            return View(roomEditViewModel);
        }

        // POST: Rooms/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomEditViewModel room)
        {
            if (ModelState.IsValid)
            {
                await _portalService.EditRoomAsync(room);
                if (room.Images != null)
                {
                    await UploadImagesAsync(room.Images, room.Id);
                }

                return RedirectToAction("Details", new { id = room.Id });

            }
            return View(room);
        }

        /// <summary>
        /// Admins can delete the room.
        /// </summary>
        /// <param name="id">Id of the room.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _portalService.GetRoom(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotelId = _portalService.GetRoom(id).HotelId;
            await _portalService.DeleteRoom(id, true);
            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }
    }
}