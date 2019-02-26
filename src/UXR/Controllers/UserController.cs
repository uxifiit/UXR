using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using UXI.Common.Web.Extensions;
using UXR.Models;
using UXR.Models.Entities;
using UXR.ViewModels;
using UXR.Studies.Models.Queries;

namespace UXR.Controllers
{
    [System.Web.Mvc.Authorize]
    public class UserController : Controller
    {
        public static readonly string ControllerName = nameof(UserController).Replace("Controller", "");


        private IdentityDatabase _database;
        private ApplicationUserManager _userManager;

        public UserController(IdentityDatabase database, ApplicationUserManager userManager)
        {
            _database = database;
            _userManager = userManager;
        }


        [Authorize(Roles = UserRoles.ADMIN)]
        public ActionResult Index()
        {
            var adminRole = _database.Roles
                                     .Where(r => r.Name == UserRoles.ADMIN)
                                     .SingleOrDefault();

            var approvedRole = _database.Roles
                                        .Where(r => r.Name == UserRoles.APPROVED)
                                        .SingleOrDefault();

            var users = _database.Users
                                 .OrderBy(u => u.Email)
                                 .AsDbQuery()
                                 .ToList();
                 
            return View(new ManageUsersListViewModel(users, adminRole, approvedRole));
        }

        private const string cannotChangeSuperadminMessage = "You can't remove the Admin role from  the Super Admin";

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.ADMIN)]
        public async Task<ActionResult> Index(ManageUsersListViewModel userList)
        {
            Request.ThrowIfDifferentReferrer();
            
            var users = _database.Users
                                 .OrderBy(u => u.Email)
                                 .AsDbQuery()
                                 .ToList();

            var adminRole = _database.Roles
                                     .Where(r => r.Name == UserRoles.ADMIN)
                                     .SingleOrDefault();

            var superAdminRole = _database.Roles
                                          .Where(r => r.Name == UserRoles.SUPERADMIN)
                                          .SingleOrDefault();

            var approvedRole = _database.Roles
                                        .Where(r => r.Name == UserRoles.APPROVED)
                                        .SingleOrDefault();

            foreach (ManageUserViewModel userViewModel in userList.Users)
            {
                var user = users.Where(u => u.Id == userViewModel.Id).SingleOrDefault();

                if (user != null)
                {
                    if (user.EmailConfirmed == false && userViewModel.IsConfirmed)
                    {
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        await _userManager.ConfirmEmailAsync(user.Id, code);
                    }

                    if (userViewModel.Approved)
                    {
                        await _userManager.AddToRoleAsync(user.Id, UserRoles.APPROVED);
                    }
                    if (User.IsInRole(UserRoles.SUPERADMIN))
                    {
                        if (userViewModel.IsAdmin)
                        {
                            await _userManager.AddToRoleAsync(user.Id, UserRoles.ADMIN);
                            await _userManager.AddToRoleAsync(user.Id, UserRoles.APPROVED);
                        }
                        var isSuperAdmin = user.Roles.Any(r => r.RoleId == superAdminRole.Id);
                        if (!userViewModel.IsAdmin)
                        {
                            if (!isSuperAdmin)
                            {
                                await _userManager.RemoveFromRoleAsync(user.Id, UserRoles.ADMIN);
                            }
                            else
                            {
                                ModelState.AddModelError("", cannotChangeSuperadminMessage);
                            }
                        }
                    }
                }
            }


            return View(new ManageUsersListViewModel(users, adminRole, approvedRole));
        }
    }
}
