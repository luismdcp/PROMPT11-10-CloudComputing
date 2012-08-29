using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;

namespace CloudNotes.WebRole.Controllers
{
    public class UsersController : Controller
    {
        #region Fields

        private readonly IUsersService _usersService;

        #endregion Fields

        #region Constructors

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        #endregion Constructors

        #region Actions

        // GET: /Users/Home
        public ActionResult Home(IPrincipal principal)
        {
            var userIsRegistered = _usersService.UserIsRegistered(principal);
            var authenticationInfo = _usersService.GetUserAuthenticationInfo(principal);

            if (!userIsRegistered)
            {
                ViewBag.UserName = authenticationInfo.Name;
                ViewBag.UserEmail = authenticationInfo.Email;
                return View();
            }
            else
            {
                var user = _usersService.GetByIdentifiers(authenticationInfo.UniqueIdentifier, authenticationInfo.IdentityProviderIdentifier);
                Session["CurrentUser"] = user;
                return RedirectToAction("Index", "TaskLists");
            }
        }

        // POST: /Users/Home
        [HttpPost, ActionName("Home")]
        public ActionResult HomePost(IPrincipal principal)
        {
            var user = new User();
            
            try
            {
                TryUpdateModel(user);

                // User name cannot have the character '-' because it is used to concatenate the user name and indentity provider in the Users table.
                if (user.Name.Contains("-"))
                {
                    ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult("User name cannot have the character '-'") };
                    return View(user);
                }

                if (user.IsValid())
                {
                    _usersService.FillAuthenticationInfo(user, principal);
                    _usersService.Create(user);
                    Session["CurrentUser"] = user;
                    return RedirectToAction("Index", "TaskLists");
                }
                else
                {
                    ViewBag.ValidationErrors = user.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("User creation exception: {0}", ex.Message)) };
            }

            return View(user);
        }

        #endregion Actions
    }
}