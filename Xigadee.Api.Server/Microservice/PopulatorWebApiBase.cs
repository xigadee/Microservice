using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class provides the basic set of method to set up a BFF WebApi layer.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public abstract class PopulatorWebApiBase<M, C>: PopulatorBase<M, C>
        where M : Microservice, new()
        where C : ConfigBase, new()
    {

    }
}
