using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using ITWALA.BusinessModel;
using Microsoft.AspNet.Identity;
using ITWALA.Models;
using ITWALA.Mappers;
using Newtonsoft.Json.Linq;

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
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }

        //
        // POST: /Account/Login
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login_POST(LoginViewModel model, string returnUrl)
        {
            try
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Account", "Login");
            }
            
        }

        [AllowAnonymous]
        public ActionResult Login_Validate(LoginViewModel model)
        {
            var isValidUser = false;
            if (ModelState.IsValid)
            {
                try
                {
                    isValidUser = _myMembership.ValidateUser(model.Username, model.Password);
                }
                catch (Exception ex)
                {
                    return HandleError(ex,"Account","Login");
                }
                if(!isValidUser)
                {
                    ModelState.AddModelError("Username", "User does not exist");
                }
            }
            var response = new
            {
                result = isValidUser,
                errors = GetErrorListFromModelState(ModelState)
            };
            return Json(response,JsonRequestBehavior.AllowGet);
        }

        private ViewResult HandleError(Exception ex,string controllerName,string actionName)
        {
            return View("Error", new HandleErrorInfo(ex, controllerName, actionName));
        }

        [AllowAnonymous]
        public ActionResult Register_Validate(RegisterViewModel model)
        {
            var isNewUser = false;
            if (ModelState.IsValid)
            {
                try
                {
                    isNewUser = _myMembership.IsNewUser(model.Username, model.Email);
                }
                catch (Exception ex)
                {
                    return HandleError(ex, "Account", "Register");
                }
                if (!isNewUser)
                {
                    ModelState.AddModelError("Username", "User Already Exists");
                }
            }
            var response = new
            {
                result = isNewUser,
                errors = GetErrorListFromModelState(ModelState)
            };
            return Json(response,JsonRequestBehavior.AllowGet);
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register_POST(RegisterViewModel model)
        {
            try
            {
                bool success = _myMembership.CreateNewUser(UserMapper.MapToUserType(model));
                if (success)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, true);
                    return RedirectToAction("Index", "Home");
                }
                return HandleError(new Exception("Something Wrong"), "Account", "Register");
            }
            catch(Exception ex)
            {
                return HandleError(ex,"Account","Register");
            }
            
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

        private static List<string> GetErrorListFromModelState(ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            return query.ToList();
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

        
        [AllowAnonymous]
        public ActionResult FacebookLogin()
        {
            var client = new FacebookClient();
            return Redirect(string.Format(client.BaseUrl, client.AppId, client.RedirectUrl));
        }

        [AllowAnonymous]
        public ActionResult FacebookLoginCallback(string code)
        {
            var client = new FacebookClient();
            var result = client.VerifyAuthentication(code);
            if (result == null)
            {
                return RedirectToAction("Login", "Account");
            }
            FormsAuthentication.SetAuthCookie(result.email, true);
            return Redirect(Url.Action("Index", "Home"));
        }

        [AllowAnonymous]
        public ActionResult GoogleLogin()
        {
            GoogleClient client = new GoogleClient();
            return Redirect(string.Format(client.BaseUrl,client.AppId,client.RedirectUrl));
        }

        [AllowAnonymous]
        public ActionResult GoogleLoginOK(string code)
        {
            GoogleClient client = new GoogleClient();
            var result = client.VerifyAuthentication(code);
            if (result != null)
            {
                FormsAuthentication.SetAuthCookie(result.email, true);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public void TwitterLogin()
        {
            AuthenticationResult result = new TwitterClient().GetUserData();
        }
        [AllowAnonymous]
        public ActionResult TwitterLoginOK()
        {
            AuthenticationResult result = new TwitterClient().VerifyAuthentication();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public GoogleUserViewModel MapGoogleUserViewModel(string result)
        {
            var user = JObject.Parse(result);
            var googleUser = new GoogleUserViewModel
            {
                email = user["emails"][0]["value"].ToString(),
                given_name = user["name"]["givenName"].ToString(),
                family_name = user["name"]["familyName"].ToString(),
                name = user["displayName"].ToString()
            };
            return googleUser;
        }

        [AllowAnonymous]
        public ActionResult EditProfile()
        {
            string username = User.Identity.GetUserName();
            var user = _myMembership.GetUser(username);
            return View(user);
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile_POST(User user)
        {
            User userModel = _myMembership.GetUserByUserid(user.Userid);
            TryUpdateModel(userModel,null,new string[] {"FirstName","LastName"});
            if (ModelState.IsValid)
            {
                var isUpdated = _myMembership.UpdateUser(userModel);
                if (isUpdated)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("EditProfile",userModel);
                }
            }
            ModelState.AddModelError("Username", "User Doesn't exists");
            return RedirectToAction("EditProfile",user);
        }
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ChangePassword_Post(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.GetUserName();
                if (_myMembership.ChangePassword(username, viewModel.OldPassword, viewModel.NewPassword))
                {
                    return RedirectToAction("Index","Home");
                }
                ModelState.AddModelError("OldPassword", "The Current Password is incorrect");
            }
            return View("ChangePassword",viewModel);
        }

    }
}