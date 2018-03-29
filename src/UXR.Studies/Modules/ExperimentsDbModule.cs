using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using UXR.Studies;

namespace UXR.Studies.Modules
{
    public class StudiesDbModule : NinjectModule
    {
        public override void Load()
        {
            Bind<StudiesDatabase>().ToSelf();
        }
    }
}
