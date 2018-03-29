using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using UXR.Models;

namespace UXR.Modules
{
    public class IdentityDbModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IdentityDatabase>().ToSelf();
        }
    }
}
