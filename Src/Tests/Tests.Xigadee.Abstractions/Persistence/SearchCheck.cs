using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Linq.Expressions;

namespace Test.Xigadee
{
    [TestClass]
    public class SearchFilterCheck
    {
        private IEnumerable<string> Examples()
        {
            yield return "Address eq 'Redmond'";
            yield return "not Address eq 'Redmond'";
            yield return "Address eq 'Redmond' or not Price gt 20.90 and Other eq 'he)llo'";
            yield return "Address eq        'Redmond' or (not Price gt 200 and Other eq 'he)llo')";
            yield return "(Address eq 'Redmond') or (not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio'))";
            yield return "( (Address eq 'Redmond') or ((not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio')) ) )";
            yield return "((Address eq 'Redmond') or (not Price gt '(56)' and ((Other eq 'h(e)llo' and Freddo eq 22 )xor Dock eq 'Coolio')))";
        }

        [TestMethod]
        public void FilterParserTest()
        {
            List<ODataTokenCollection> nodes = null;
            try
            {
                nodes = Examples().Select(l => new ODataTokenCollection(l)).ToList();
                var hmm1 = nodes[3].ToString();
                var hmm2 = nodes[3].ToString(true);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

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
            var sr1 = (SearchRequest)"$filter=Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'&$orderby=Address DESC, Price ASC&$select=Address, Price";
            var sr2 = (SearchRequest)"$filter=Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'";
            var sr3 = (SearchRequest)"$filter=Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'";
            var s4r = (SearchRequest)"$filter=Address eq 'Redmond' or not Price gt 200 or Other eq 'hello'";


            var strSr1 = sr1.ToString();
            sr1.AssignFilter(sr1.Filter + "and userId eq 'paul'");
            var strSr2 = sr1.ToString();

        }


        [TestMethod]
        public void AppendTests()
        {
            var sr = new SearchRequest();

            sr.AppendFilter("typeaction", "eq", "actionType", "and");

            sr.AppendFilter("typetemplate", "eq", "templateType", "and");

            sr.AppendFilter("state", "eq", "state", "and");

            var result = sr.ToString();

        }

        [TestMethod]
        public void AppendTestsUnderscore()
        {
            var sr = new SearchRequest();

            sr.AppendFilter("type_action", "eq", "actionType", "and");

            sr.AppendFilter("typetemplate", "eq", "template_Type", "and");

            sr.AppendFilter("state", "eq", "state", "and");

            var result = sr.ToString();

        }



        [TestMethod]
        public void SplitTest()
        {
            var filter0 = new FilterCollection("Address eq 'Redmond'");
            var filtern0 = new FilterCollection("not Address eq 'Redmond'");
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
