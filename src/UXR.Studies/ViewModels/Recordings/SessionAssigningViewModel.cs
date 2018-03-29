using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UXR.Studies.Models;

namespace UXR.Studies.ViewModels.Recordings
{
    public class SessionAssigningViewModel
    {
        public SessionAssigningViewModel()
        {

        }

        public SessionAssigningViewModel(List<Project> projects, List<RecordingViewModel> recordings)
        {
            ProjectSelectList = new SelectList(projects, "Id", "Name");

            SessionSelectLists = new List<SelectList>();
            foreach(Project p in projects)
            {
                SessionSelectLists.Add(new SelectList(p.Sessions, "Id", "Name"));
            }

            Recordings = recordings;
            RecordingSelections = Enumerable.Repeat(true, recordings.Count).ToList();
        }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        public SelectList ProjectSelectList { get; set; }

        [Display(Name = "Session")]
        public int SessionId { get; set; }

        public List<SelectList> SessionSelectLists { get; set; }

        public List<RecordingViewModel> Recordings { get; set; }

        public List<bool> RecordingSelections { get; set; }
    }
}
