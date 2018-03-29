using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace UXR.Studies.ViewModels.GroupNodes
{
    public class GroupNodesViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        public IPagedList<GroupNodeViewModel> Nodes { get; set; }
    }
}
