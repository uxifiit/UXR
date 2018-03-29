using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Sessions;
using UXR.Studies.ViewModels.SessionTemplates;
using UXR.Studies.ViewModels.Users;

namespace UXR.Studies.ViewModels.Projects
{
    /// <summary>
    /// <see cref="UXR.Studies.Models.Project"/>
    /// </summary>
    public class EditProjectViewModel : EditProjectViewModelPost
    {
        public int Id { get; set; }

        public string OriginalName { get; set; }

        [Display(Name = "Owner")]
        public UserNameViewModel Owner { get; set; }

        [AutoMapper.IgnoreMap]
        public List<SessionTemplateViewModel> SessionDefinitionTemplates { get; set; }
    }


    public class EditProjectViewModelPost
    {
        private string name = String.Empty;

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get { return name; }  set { name = value?.Trim() ?? String.Empty; } }


        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        private string definition = String.Empty;
        [Display(Name = "Session template")]
        [DataType(DataType.MultilineText)]
        public string SessionDefinitionTemplate { get { return definition; } set { definition = value?.Trim() ?? String.Empty; } }
    }
}
