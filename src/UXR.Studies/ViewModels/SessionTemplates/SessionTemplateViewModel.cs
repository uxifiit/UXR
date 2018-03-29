using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Users;

namespace UXR.Studies.ViewModels.SessionTemplates
{
    public class SessionTemplateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Template")]
        public string Definition { get; set; }

        [Display(Name = "Author")]
        public UserNameViewModel Author { get; set; }
    }
}
