using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ITWALA.BusinessModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ITWALA.Models;
using ITWALA.Mappers;

namespace ITWALA.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private MyDatabaseEntities _myDatabase = new MyDatabaseEntities();
        private readonly MyMembershipProvider _myMembership = new MyMembershipProvider();

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool validUser = _myMembership.ValidateUser(model.Username, model.Password);
                if (validUser)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Invalid user","User does not exist");
            }
            ModelState.Remove("Password");
            return PartialView(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return PartialView();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isExistingUser = _myMembership.ValidateUserExists(model.Username, model.Email);
                if (isExistingUser)
                {
                    ModelState.AddModelError("UserExists", "Username already exists");
                }
                else
                {
                    bool success = _myMembership.CreateNewUser(UserMapper.MapToUserType(model));
                    if (success)
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
            }
            return PartialView(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            var returnPath = new Uri(returnUrl).AbsolutePath;
            if (Url.IsLocalUrl(returnPath))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}