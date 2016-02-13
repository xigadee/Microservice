using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ICommand: IMessageHandlerDynamic, IMessageInitiator, IJob
    {
    }
}
