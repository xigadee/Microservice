using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the current entity state.
    /// </summary>
    public class EntityState
    {
        /// <summary>
        /// This is the version id.
        /// </summary>
        public Guid Versionid { get; set; }= Guid.NewGuid();
        /// <summary>
        /// This is the id.
        /// </summary>
        public Guid Id { get; set; }= new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");
        /// <summary>
        /// This is the entity references.
        /// </summary>
        public string Reference
        {
            get
            {
                return $"anyone+{Id.ToString("N")}@hotmail.com";
            }
        }
    }
}
