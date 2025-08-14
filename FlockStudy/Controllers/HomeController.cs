using System.Diagnostics;
using System.Security.Claims;
using FlockStudy.Models;
using FlockStudy.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlockStudy.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPrayerService _prayerService;
        private readonly IUserService _userService;
        public HomeController(ILogger<HomeController> logger,IPrayerService prayerService, IUserService userService)
        {
            _logger = logger;
            _prayerService=prayerService;
            _userService = userService;
        }
        private async Task<int> getUserId()
        {
            var id = await _userService.GetCurrentUserId();
            return id;
        }
        public async  Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var dashboard = await _prayerService.GetDashboardAsync(await getUserId());
            return View(dashboard);
        }
        [HttpPost]
        public async Task<IActionResult> CommitToPrayer(int prayerRequestId)
        {
          
            await _prayerService.CommitToPrayerAsync(await getUserId(), prayerRequestId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkCompleted(int prayerRequestId)
        {
           
            await _prayerService.MarkCompletedAsync(await getUserId(), prayerRequestId);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
