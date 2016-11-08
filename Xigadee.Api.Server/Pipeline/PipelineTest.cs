using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Xigadee
{
    public class PipelineTest: IAppBuilder
    {
        public IDictionary<string, object> Properties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object Build(Type returnType)
        {
            throw new NotImplementedException();
        }

        public IAppBuilder New()
        {
            throw new NotImplementedException();
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
