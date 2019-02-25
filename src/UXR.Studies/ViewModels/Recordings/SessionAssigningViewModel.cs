using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        public SessionAssigningViewModel(IEnumerable<SelectableRecordingViewModel> recordings)
        {
            Recordings = recordings?.ToList();
        }


        public void ResetProjectSelection(IEnumerable<SelectProjectSessionViewModel> projects)
        {
            ProjectSelection = new SelectList(projects, nameof(SelectProjectSessionViewModel.Id), nameof(SelectProjectSessionViewModel.Name), ProjectId);

            SessionSelections = projects.Select(p => new SelectList(p.Sessions, nameof(SelectSessionViewModel.Id), nameof(SelectSessionViewModel.Name), SessionId))
                                        .ToList();
        }


        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [ReadOnly(true)]
        public SelectList ProjectSelection { get; private set; }

        [Display(Name = "Session")]
        public int SessionId { get; set; }

        [ReadOnly(true)]
        public List<SelectList> SessionSelections { get; private set; }

        public List<SelectableRecordingViewModel> Recordings { get; set; }
    }
}
