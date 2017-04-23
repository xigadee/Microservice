using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Xigadee
{
    public interface IPipelineWebApiUnity:IPipelineWebApi
    {
        /// <summary>
        /// This is the Unity container used within the application.
        /// </summary>
        IUnityContainer Unity { get; }
    }
}
