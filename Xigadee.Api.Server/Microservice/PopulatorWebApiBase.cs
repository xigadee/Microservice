using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class PopulatorWebApiBase<M, C>: PopulatorBase<M, C>
                where M : Microservice, new()
        where C : ConfigBase, new()
    {
    }
}
