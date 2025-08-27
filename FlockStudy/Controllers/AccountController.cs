using FlockStudy.Models.ViewModels;
using FlockStudy.Models;
using FlockStudy.Service;
using Microsoft.AspNetCore.Mvc;

namespace FlockStudy.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Accessed Account Index page.");
            return View();
        }

        [Route("login")]
        public IActionResult Login()
        {
            _logger.LogInformation("Accessed Login page.");
            return View();
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login model submitted for email {Email}", model.Email);
                return View(model);
            }

            var success = await _userService.LoginAsync(model.Email, model.Password);
            if (!success)
            {
                _logger.LogWarning("Failed login attempt for email {Email}", model.Email);
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            _logger.LogInformation("User {Email} logged in successfully.", model.Email);
            return RedirectToAction("Index", "Home");
        }

        [Route("register")]
        public IActionResult Register()
        {
            _logger.LogInformation("Accessed Register page.");
            return View();
        }

        [HttpPost]
        [Route("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration model submitted for email {Email}", model.Email);
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
            };

            var result = await _userService.CreateUserAsync(user, model.Password);
            if (!result)
            {
                _logger.LogWarning("Registration failed for email {Email}. Email or username already in use.", model.Email);
                ModelState.AddModelError("", "Registration failed. Email or Username Already In Use");
                return View(model);
            }
            else
            {
                _logger.LogInformation("User {Email} registered successfully.", model.Email);

                await Login(new LoginViewModel { Email = model.Email, Password = model.ConfirmPassword });
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogoutAsync();
            _logger.LogInformation("User logged out successfully.");
            return RedirectToAction("Login");
        }
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            User user = await _userService.GetCurrentUserAsync();
            return View(user);
        }
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsername(UpdateUsernameViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var userId = await _userService.GetCurrentUserId();
            var result = await _userService.UpdateUsernameAsync(userId, model.NewUsername);

            if (!result)
            {
                TempData["Error"] = "Failed to update username. It might already be taken.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Username updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid || model.NewPassword != model.ConfirmNewPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("Index");
            }

            var userId = await _userService.GetCurrentUserId();
            var result = await _userService.UpdatePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (!result)
            {
                TempData["Error"] = "Failed to update password. Check your current password.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("Index");
        }
    }
}