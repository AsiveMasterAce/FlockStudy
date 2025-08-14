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
        public async Task<IActionResult> Create(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("", "Title and description are required.");
                return View();
            }

            var userId = await _userService.GetCurrentUserId();
            await _prayerService.CreatePrayerRequestAsync(userId, title, description);

            return RedirectToAction("Index", "Home");
        }
    }
}
