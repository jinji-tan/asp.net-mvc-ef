using Microsoft.AspNetCore.Mvc;
using mvc_ef.DTOs.User;
using mvc_ef.Helpers.interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace mvc_ef.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthHelper _auth;

        public AuthController(IAuthHelper auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            var user = await _auth.Login(loginDto);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(loginDto);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);

            if (await _auth.UserExists(registerDto.Email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View(registerDto);
            }

            bool registered = await _auth.Register(registerDto);
            if (!registered)
            {
                ModelState.AddModelError("", "Failed to register");
                return View(registerDto);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
