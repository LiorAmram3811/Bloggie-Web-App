using Bloggie.Web.Data;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email
            };
            var identityResult = await _userManager.CreateAsync(identityUser, registerRequest.Password);
            if (identityResult.Succeeded)
            {
                var roleIdentityResult = await _userManager.AddToRoleAsync(identityUser, "User");
                if (roleIdentityResult.Succeeded)
                {
                    return RedirectToAction("Register");
                }
            }
            // TODO: show errors
            return View();
        }
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            var model = new LoginRequest { ReturnUrl = ReturnUrl };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, false, false);
            if (signInResult != null && signInResult.Succeeded)
            {
                if(!string.IsNullOrWhiteSpace(loginRequest.ReturnUrl))
                {
                    return Redirect(loginRequest.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            // TODO: show errors
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
