using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    public class EntityState
    {
        public Guid Versionid { get; set; }= Guid.NewGuid();

        public Guid Id { get; set; }= new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");

        public string Reference
        {
            get
            {
                return $"anyone+{Id.ToString("N")}@hotmail.com";
            }
        }
    }
}
