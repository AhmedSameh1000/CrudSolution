using Entities.IdentityIntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.DTOs;
using System.Security.Policy;

namespace CrudUi.Controllers
{
    [Route("{Controller}/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
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
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var User = await _userManager.FindByEmailAsync(registerDTO.Email!.Trim());

            if (User is not null)
            {
                ModelState.AddModelError("Email", "User is Already Registred");
                return View("Register", registerDTO);
            }

            //Check for validation errors
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(registerDTO);
            }

            ApplicationUser user = new()
            {
                PersonName = registerDTO.PersonName,
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                //signIn
                await _signInManager.SignInAsync(user, isPersistent: false);//true mean Keep Me LoogIn
                return RedirectToAction("", "Person");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDTO);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInDTO loginDTO, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email!, loginDTO.Password!, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("", "Person");
                }
            }

            ModelState.AddModelError("Login", "Invalid email or password");
            return View(loginDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("", "Person");
        }
    }
}