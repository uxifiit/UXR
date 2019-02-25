using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Projects;

namespace UXR.Studies.ViewModels.Recordings
{
    public class SelectProjectSessionViewModel : SelectProjectViewModel
    {
        public List<SelectSessionViewModel> Sessions { get; set; }
    }


    public class SelectSessionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
