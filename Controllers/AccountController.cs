using Arkenea_new.Data;
using Arkenea_new.Models;
using Arkenea_new.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

namespace Arkenea_new.Controllers
{
    public class AccountController : Controller
    {
        public readonly UserManager<User> _userManager;
        public readonly SignInManager<User> _signInManager;
        public readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserStore<User> _userStore;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext applicationDbContext,
            ILogger<AccountController> logger, IUserStore<User> userStore)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userStore = userStore;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var returnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(loginViewModel);

        }

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var returnUrl = Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User();
                user.FirstName = registerViewModel.FirstName;
                user.LastName = registerViewModel.LastName;

                await _userStore.SetUserNameAsync(user, registerViewModel.EmailAddress, CancellationToken.None);
                user.Email = registerViewModel.EmailAddress;
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = registerViewModel.EmailAddress, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index","Home");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(registerViewModel);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                // If user is already authenticated, redirect to another page (e.g., home page)
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                    // Send the reset password link to the user via email or any other method
                    // Example: SendEmail(model.Email, "Reset Password", $"Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

                    // Redirect the user to a confirmation page
                    return RedirectToAction("ResetPassword", "Account", new { email = model.Email, token });
                }
                // To display a generic error message to prevent account enumeration, do not reveal that the user does not exist
                ViewBag.ErrorMessage = "User with this email does not exist.";
            }
            return View(model);
        }

        public IActionResult ResetPassword(string email, string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                // If user is already authenticated, redirect to another page (e.g., home page)
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (email == null || token == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid password reset token.");
                }
                var model = new ResetPasswordViewModel { Email = email, Token = token };
                return View(model);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                // To display a generic error message to prevent account enumeration, do not reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


    }
}
