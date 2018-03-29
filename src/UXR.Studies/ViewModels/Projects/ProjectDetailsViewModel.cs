using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Sessions;

namespace UXR.Studies.ViewModels.Projects
{
    public class ProjectDetailsViewModel : ProjectViewModel
    {
        [AutoMapper.IgnoreMap]
        public List<SessionViewModel> Sessions { get; set; } = new List<SessionViewModel>();
    }
}
