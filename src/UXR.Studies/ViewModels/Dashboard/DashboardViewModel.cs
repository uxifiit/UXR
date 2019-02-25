using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardViewModel(IEnumerable<NodeStatusGroupViewModel> groups)
        {
            Groups = new List<NodeStatusGroupViewModel>(groups);
        }

        public List<NodeStatusGroupViewModel> Groups { get; set; }
    }

    public class NodeStatusGroupViewModel
    {
        public NodeStatusGroupViewModel(string name, IEnumerable<NodeStatusViewModel> nodes)
        {
            Name = name;
            Nodes = new List<NodeStatusViewModel>(nodes);
        }

        public string Name { get; set; }

        public List<NodeStatusViewModel> Nodes { get; set; }
    }

    public class NodeStatusViewModel
    {
        public string NodeName { get; set; }
        public string NodeGroupName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH\\:mm}")]
        public DateTime UpdatedAt { get; set; }

        public bool IsRecording { get; set; }

        public string CurrentSession { get; set; }
    }
}
