using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UXR.Studies.ViewModels
{
    public class DropDownListViewModel
    {
        public string SelectedValue { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}
