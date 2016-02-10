#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public interface IRequireSharedServices
    {
        ISharedService SharedServices { get; set; }
    }

    public interface ISharedService
    {
        bool RegisterService<I>(I instance, string serviceName = null) where I : class;

        bool RegisterService<I>(Lazy<I> instance, string serviceName = null) where I : class;

        bool RemoveService<I>() where I : class;

        I GetService<I>() where I : class;

        bool HasService<I>() where I : class;
    }
}
