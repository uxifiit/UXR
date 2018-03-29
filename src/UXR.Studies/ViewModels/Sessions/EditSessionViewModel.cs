using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UXR.Studies.Models;
using UXR.Studies.ViewModels.Projects;
using UXR.Studies.ViewModels.Recordings;

namespace UXR.Studies.ViewModels.Sessions
{
    public class EditSessionViewModel : EditSessionViewModelPost
    {
        public int Id { get; set; }

        public string OriginalName { get; set; }

        [AutoMapper.IgnoreMap]
        public SelectList ProjectSelectList { get; set; }

        public void PopulateProjectSelectList(List<SelectProjectViewModel> projects)
        {
            ProjectSelectList = new SelectList(projects, nameof(SelectProjectViewModel.Id), nameof(SelectProjectViewModel.Name), ProjectId);
        }
    }


    public class EditSessionViewModelPost
    {
        private string name = String.Empty;

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get { return name; } set { name = value?.Trim() ?? String.Empty; } }

        [Display(Name = "Start time")]
        [Required(ErrorMessage = "Please set when the session is going to start.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Length")]
        [Required(ErrorMessage = "Please set the length of the session.")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Length { get; set; }

        private string definition = String.Empty;

        [Display(Name = "Definition")]
        [DataType(DataType.MultilineText)]
        public string Definition { get { return definition; } set { definition = value?.Trim() ?? String.Empty; } }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        public bool UseProjectDefinitionTemplate { get; set; }
    }
}
