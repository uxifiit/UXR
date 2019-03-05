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
using AutoMapper;
using UXR.Models.Queries;

namespace UXR.Controllers
{
    [Authorize(Roles = UserRoles.ADMIN)]
    public class UserController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(UserController).Replace("Controller", "");

        static UserController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ManageUserViewModel>()
                   .ForMember(user => user.IsAdmin, opt => opt.Ignore())
                   .ForMember(user => user.IsConfirmed, opt => opt.Ignore())
                   .ForMember(user => user.IsApproved, opt => opt.Ignore());
            });

            Mapper = config.CreateMapper();
        }

        private readonly IdentityDatabase _database;
        private readonly ApplicationUserManager _userManager;

        public UserController(IdentityDatabase database, ApplicationUserManager userManager)
        {
            _database = database;
            _userManager = userManager;
        }


        private IdentityRole GetAdminRole() => _database.Roles.GetOrDefault(UserRoles.ADMIN);

        private IdentityRole GetApprovedRole() => _database.Roles.GetOrDefault(UserRoles.APPROVED);

        private IdentityRole GetSuperAdminRole() => _database.Roles.GetOrDefault(UserRoles.SUPERADMIN);


        private static IEnumerable<ManageUserViewModel> MapUsers(IEnumerable<ApplicationUser> users, IdentityRole approvedRole, IdentityRole adminRole)
        {
            foreach (var user in users)
            {
                yield return Mapper.Map<ApplicationUser, ManageUserViewModel>
                    (
                        user,
                        opts => opts.AfterMap((src, dest) => {
                            dest.IsAdmin = user.Roles.Any(r => r.RoleId == adminRole.Id);
                            dest.IsConfirmed = user.EmailConfirmed;
                            dest.IsApproved = user.Roles.Any(r => r.RoleId == approvedRole.Id);
                        })
                    );
            }
        }


        public ActionResult Index()
        {
            var approvedRole = GetApprovedRole();
            var adminRole = GetAdminRole();
            var superAdminRole = GetSuperAdminRole();

            var users = _database.Users
                                 .OrderBy(u => u.Email)
                                 .Where(u => u.Roles.Any(r => r.RoleId == superAdminRole.Id) == false)
                                 .ToList();
                 
            return View(MapUsers(users, approvedRole, adminRole).ToList());
        }


        private const string cannotChangeSuperadminMessage = "You can't remove the Admin role from the Super Admin";

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([System.Web.Http.FromBody] List<ManageUserViewModel> updates)
        {
            Request.ThrowIfDifferentReferrer();

            var superAdminRole = GetSuperAdminRole();

            var userIds = updates.Select(u => u.Id).ToList();
            var users = _database.Users
                                 .Where(u => userIds.Contains(u.Id))
                                 .Where(u => u.Roles.Any(r => r.RoleId == superAdminRole.Id) == false)
                                 .ToList();

            bool isCurrentUserSuperAdmin = User.IsInRole(UserRoles.SUPERADMIN);

            foreach (var update in updates)
            {
                var user = users.Where(u => u.Id == update.Id).SingleOrDefault();
                if (user != null)
                {
                    await UpdateUserAsync(user, update);

                    if (isCurrentUserSuperAdmin)
                    {
                        await UpdateUserAdminRightsAsync(user, update, superAdminRole);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<ActionResult> Approve(string userId)
        {
            Request.ThrowIfDifferentReferrer();

            if (User.IsInRole(UserRoles.ADMIN))
            {
                var user = _database.Users.FilterById(userId).SingleOrDefault();

                if (user != null)
                {
                    await ApproveUserAsync(user);
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(HomeController.Index), HomeController.ControllerName);
        }


        private async Task UpdateUserAsync(ApplicationUser user, ManageUserViewModel update)
        {
            if (user.EmailConfirmed == false && update.IsConfirmed)
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                await _userManager.ConfirmEmailAsync(user.Id, code);
            }


            bool isApproved = await _userManager.IsInRoleAsync(user.Id, UserRoles.APPROVED);
            if (update.IsApproved && isApproved == false)
            {
                await ApproveUserAsync(user);
            }
        }


        private async Task ApproveUserAsync(ApplicationUser user)
        {
            var approval = await _userManager.AddToRoleAsync(user.Id, UserRoles.APPROVED);

            if (approval != null && approval.Succeeded)
            {
                await _userManager.SendEmailAsync(user.Id, "Your account was approved", "");
            }
            
        }


        private async Task UpdateUserAdminRightsAsync(ApplicationUser user, ManageUserViewModel update, IdentityRole superAdminRole)
        {
            if (update.IsAdmin)
            {
                await _userManager.AddToRoleAsync(user.Id, UserRoles.ADMIN);
                await _userManager.AddToRoleAsync(user.Id, UserRoles.APPROVED);
            }
            else
            {
                var isSuperAdmin = user.Roles.Any(r => r.RoleId == superAdminRole.Id);
                if (isSuperAdmin)
                {
                    ModelState.AddModelError("", cannotChangeSuperadminMessage);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user.Id, UserRoles.ADMIN);
                }
            }
        }
    }
}
