using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    internal abstract class PopulatorConsoleBase<M>:PopulatorBase<M, ConfigConsole>
        where M : MicroserviceBase, new()
    {
    }
}
