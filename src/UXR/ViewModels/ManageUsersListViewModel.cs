using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UXR.Models.Entities;

namespace UXR.ViewModels
{
    public class ManageUsersListViewModel
    {
        public ManageUsersListViewModel()
        {

        }

        public ManageUsersListViewModel(List<ApplicationUser> users, IdentityRole adminRole, IdentityRole approvedRole)
        {
            Users = new List<ManageUserViewModel>();
            foreach (ApplicationUser user in users)
            {
                Users.Add(new ManageUserViewModel(user, adminRole, approvedRole));
            }
        }

        public List<ManageUserViewModel> Users { get; set; }
    }
}
