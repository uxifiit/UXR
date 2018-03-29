using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.SessionTemplates;

namespace UXR.Studies.ViewModels.Projects
{
    public class CreateProjectViewModel : CreateProjectViewModelPost
    {
        [AutoMapper.IgnoreMap]
        public List<SessionTemplateViewModel> SessionDefinitionTemplates { get; set; }
    }


    public class CreateProjectViewModelPost
    {
        private string name = String.Empty;

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get { return name; } set { name = value?.Trim() ?? String.Empty; } }


        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        private string definition = String.Empty;
        [Display(Name = "Session template")]
        [DataType(DataType.MultilineText)]
        public string SessionDefinitionTemplate { get { return definition; } set { definition = value?.Trim() ?? String.Empty; } }
    }
}
