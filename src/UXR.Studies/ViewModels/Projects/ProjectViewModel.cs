using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Users;

namespace UXR.Studies.ViewModels.Projects
{
    /// <summary>
    /// <see cref="UXR.Studies.Models.Project"/> 
    /// </summary>
    public class ProjectViewModel
    {
        [Display(AutoGenerateField = false)]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required (ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Owner")]
        public UserNameViewModel Owner { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedAt { get; set; }
    }
}
