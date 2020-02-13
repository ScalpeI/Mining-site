using Domain.Abstract;
using Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using reCAPTCHA.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        IUserRepository repository;

        public AccountController(IUserRepository repo)
        {
            repository = repo;
        }
        public ActionResult Login()
        {
            ViewBag.Header = "Login";
            return View();
        }
        [HttpPost]
        [CaptchaValidator]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, bool captchaValid)
        {
            //search into db
            if (ModelState.IsValid)
            {
                User user = repository.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: model.Password,
                    salt: Convert.FromBase64String(u.PasswordHash),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8)));
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("Index", "Rates");
                }
                else
                {
                    ModelState.AddModelError("", "User not found");
                }
            }
            return View(model);
        }
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Rates");
        }
    }
}