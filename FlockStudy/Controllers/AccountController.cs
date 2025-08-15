using FlockStudy.Models.ViewModels;
using FlockStudy.Models;
using FlockStudy.Service;
using Microsoft.AspNetCore.Mvc;

namespace FlockStudy.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }   


        public IActionResult Index()
        {
            return View();
        }
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _userService.LoginAsync(model.Email, model.Password);
            if (!success)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Route("register")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,

            };

            var result = await _userService.CreateUserAsync(user, model.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Registration failed. Email or Username Already In Use");
                return View(model);
            }
            else
            {
                await  Login(new LoginViewModel { Email = model.Email, Password = model.ConfirmPassword }); 
            }

            return RedirectToAction("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}
