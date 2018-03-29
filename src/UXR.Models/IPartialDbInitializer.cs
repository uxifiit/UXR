using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Models
{
    public interface IPartialDbInitializer<in TContext>
    {
        void Seed(TContext context);
    }
}
