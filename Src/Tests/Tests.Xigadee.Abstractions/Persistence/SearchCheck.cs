using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class SearchFilterCheck
    {
        /// Eq Equal	/Suppliers?$filter=Address/City eq 'Redmond'
        /// Ne Not equal	/Suppliers?$filter=Address/City ne 'London'
        /// Gt Greater than	/Products?$filter=Price gt 20
        /// Ge Greater than or equal	/Products?$filter=Price ge 10
        /// Lt Less than	/Products?$filter=Price lt 20
        /// Le Less than or equal	/Products?$filter=Price le 100
        /// And Logical and	/Products?$filter=Price le 200 and Price gt 3.5
        /// Or Logical or	/Products?$filter=Price le 3.5 or Price gt 200
        /// Not Logical negation	/Products?$filter=not endswith(Description,'milk')
        [TestMethod]
        public void Filter1()
        {
            var sr = (SearchRequest)"$filter=Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'";



        }

        [TestMethod]
        public void SplitTest()
        {
            var h0 = BracketDefinition.Parse("Address eq 'Redmond' or not Price gt 200 and Other eq 'he)llo'"
                , out var brackets0);
            var h1 = BracketDefinition.Parse("Address eq 'Redmond' or (not Price gt 200 and Other eq 'he)llo')"
                , out var brackets1);
            var h2 = BracketDefinition.Parse("(Address eq 'Redmond') or (not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio'))"
                , out var brackets2);
            var h3 = BracketDefinition.Parse("( (Address eq 'Redmond') or ((not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio')) ) )"
                , out var brackets3);
            var h4 = BracketDefinition.Parse("((Address eq 'Redmond') or (not Price gt '(56)' and ((Other eq 'h(e)llo' and Freddo eq 22 )xor Dock eq 'Coolio')))"
                , out var brackets4);

            var filter0 = new FilterCollection("Address eq 'Redmond'");
            var filter1 = new FilterCollection("Address eq 'Redmond' xor not Price gt 200");
            var filter2 = new FilterCollection("Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'");
            var filterb1 = new FilterCollection("Address eq 'Redmond' or (not Price gt 200 and Other eq 'hello')");

        }

        public void FilterManual()
        {
            var sr = new SearchRequest();

            //sr.Filters.Add("userid", "eq", "paul");
        }
    }
}
