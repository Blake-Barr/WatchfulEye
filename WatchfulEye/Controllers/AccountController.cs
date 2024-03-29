﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using WatchfulEye.Data;
using WatchfulEye.Models;
using WatchfulEye.ViewModels;

namespace WatchfulEye.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly WatchfulEyeContext _ctx;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, WatchfulEyeContext ctx)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var resp = new LoginViewModel();
            return View(resp);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid) return View(login);

            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, login.Password);
                if(passwordCheck)
                {
                    var res = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    if(res.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["Error"] = "Login Failed: Bad Password.";
                return View(login);
            }

            TempData["Error"] = "Login Failed: User Does Not Exist.";
            return View(login);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new NewLoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(NewLoginViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.Username,
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            else
            {
                if (!newUserResponse.Succeeded)
                {
                    foreach (var err in newUserResponse.Errors)
                    {
                        TempData["Error"] += err.Description + " <br />";   
                    }

                    return View(registerViewModel);
                }
                
            }
                

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Settings(EditLoginViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                    if (registerViewModel.NewPassword != null)
                    {
                        var passwordCheck = await _userManager.CheckPasswordAsync(user, registerViewModel.Password);

                            if (passwordCheck)
                            {

                            var result = await _userManager.ChangePasswordAsync(user, registerViewModel.Password, registerViewModel.NewPassword);

                            if (!result.Succeeded)
                            {
                                foreach (var err in result.Errors)
                                {
                                    TempData["Error"] += err.Description + " <br />";
                                }

                                return View(registerViewModel);
                            }
                    }
                    else
                    {
                        TempData["Error"] = "Please enter the correct password for your account.";
                        return View(registerViewModel);
                    }


                }
                if (registerViewModel.Username != null)
                {
                    user.UserName = registerViewModel.Username;
                }
                IdentityResult res = await _userManager.UpdateAsync(user);

                if(!res.Succeeded)
                {
                    foreach (var err in res.Errors)
                    {
                        TempData["Error"] += err.Description + " <br />";
                    }

                    return View(registerViewModel);
                }
            }
            return RedirectToAction("Settings", "Account");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Deactivate()
        {
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync();
            if (user != null)
            {
                var res = _userManager.DeleteAsync(user);
                //IdentityResult resp = await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
