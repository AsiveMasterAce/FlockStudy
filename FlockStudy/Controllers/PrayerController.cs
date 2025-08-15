using FlockStudy.Models.ViewModels;
using FlockStudy.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlockStudy.Controllers
{

    public class PrayerController : Controller
    {
        private readonly IPrayerService _prayerService;
        private readonly IUserService _userService;

        public PrayerController(IPrayerService prayerService,IUserService userService)
        {
            _prayerService = prayerService;
            _userService = userService;
        }

        [Route("add-prayer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("add-prayer")]

        public async Task<IActionResult> Create(AddPrayerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = await _userService.GetCurrentUserId();
            await _prayerService.CreatePrayerRequestAsync(userId, model.Title, model.Description);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("edit-prayer")]

        public async Task<IActionResult> Edit(int id)
        {
            var userId = await _userService.GetCurrentUserId();

            var request = await _prayerService.GetPrayerRequestByIdAsync(id,userId);
            if (request == null)
                return NotFound();

            var update = new UpdatePrayerViewModel
            {
                Id = request.Id,
                Title=request.Title,
                Description=request.Description
            };
            return View(update);
        }

        [HttpPost]
        [Route("edit-prayer")]

        public async Task<IActionResult> Edit(UpdatePrayerViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);
            var userId = await _userService.GetCurrentUserId();

            await _prayerService.UpdatePrayerRequestAsync(model.Id,userId, model.Title, model.Description);
            return RedirectToAction("Index", "Home");
        }
    }
}
