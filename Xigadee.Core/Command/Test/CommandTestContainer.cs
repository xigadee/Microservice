using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This container is used to unit test a particular command.
    /// </summary>
    /// <typeparam name="C">The command type.</typeparam>
    public class CommandTestContainer<C>
        where C : ICommand
    {
    }
}
