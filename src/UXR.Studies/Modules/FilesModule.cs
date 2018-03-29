using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using UXR.Studies.Files.Management;

namespace UXR.Studies.Modules
{
    public class FilesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<RecordingFilesManager>().ToSelf().InSingletonScope();
        }
    }
}
