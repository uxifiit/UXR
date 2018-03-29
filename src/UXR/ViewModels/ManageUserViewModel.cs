using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UXR.Models;
using UXR.Models.Entities;

namespace UXR.ViewModels
{
    public class ManageUserViewModel
    {
        public ManageUserViewModel()
        {

        }

        public ManageUserViewModel(ApplicationUser user, IdentityRole adminRole, IdentityRole approvedRole)
        {
            Username = user.UserName;
            Id = user.Id;
            IsAdmin = user.Roles.Any(r => r.RoleId == adminRole.Id);
            Approved = user.Roles.Any(r => r.RoleId == approvedRole.Id);
        }

        [Display(Name = "Username")]
        public string Username { get; set; }

        public string Id { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Approve")]
        public bool Approved { get; set; }
    }
}
