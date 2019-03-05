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

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public string Id { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Is Confirmed")]
        public bool IsConfirmed { get; set; }

        [Display(Name = "Approve")]
        public bool IsApproved { get; set; }
    }
}
