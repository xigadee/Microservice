using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    public static class MondayMorningBluesHelper
    {

        public static IEnumerable<Tuple<string, string>> ToReferences(this MondayMorningBlues entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Email))
                return new[] { new Tuple<string, string>("email", entity.Email) };

            return new Tuple<string, string>[] { };
        }
    }
}
