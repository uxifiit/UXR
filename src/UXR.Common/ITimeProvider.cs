using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Common
{
    public interface ITimeProvider
    {
        DateTime CurrentTime { get; }
    }
}
