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
            yield return "Address eq 'Redmond' or not Price gt 20.90 and Other eq 'he)llo'";
            yield return "Address eq        'Redmond' or (not Price gt 200 and Other eq 'he)llo')";
            yield return "(Address eq 'Redmond') or (not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio'))";
            yield return "( (Address eq 'Redmond') or ((not Price gt '(56)' and (Other eq 'h(e)llo' xor Dock eq 'Coolio')) ) )";
            yield return "((Address eq 'Redmond') or (not Price gt '(56)' and ((Other eq 'h(e)llo' and Freddo eq 22 )xor Dock eq 'Coolio')))";
        }

        [TestMethod]
        public void FilterParserTest()
        {
            var nodes = Examples().Select(l => new ODataTokenCollection(l)).ToList();
            var hmm1 = nodes[3].ToString();
            var hmm2 = nodes[3].ToString(true);

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


        public class Student
        {
            public int Age { get; set; }
        }


        [TestMethod]
        public void ExpressionCheck()
        {
            //ODataUriParser p;

            var paramX = Expression.Parameter(typeof(bool), "x");
            var paramY = Expression.Parameter(typeof(bool), "y");
            var paramZ = Expression.Parameter(typeof(bool), "z");
            var expr = Expression.AndAlso(Expression.OrElse(paramX, paramY), paramZ);

            var hmm = Expression.Lambda<Func<bool,bool,bool, bool>>(expr, new[] { paramX, paramY, paramZ }).Compile();

            var r = hmm(true, false, false);

            //Func<int, bool> expr1 = num => (num & 0b001) == 0b001;
            Expression<Func<int, bool>> expr1 = num => (num & 1) == 1;
            Expression<Func<int, bool>> expr2 = num => (num & 2) == 2;
            var pr = expr1.Parameters;

            ParameterExpression pe = Expression.Parameter(typeof(int), "s");
            var ExpressionTree1 = Expression.Lambda<Func<int, bool>>(expr1.Body, new[] { pe });

            var re = ExpressionTree1.Compile()(3);

            ConstantExpression constant = Expression.Constant(18, typeof(int));

            BinaryExpression body = Expression.GreaterThanOrEqual(pe, constant);

            var ExpressionTree = Expression.Lambda<Func<int, bool>>(body, new[] { pe });

            var toGo = ExpressionTree.Compile();

            var resgo = toGo(34);

            Expression<Func<int,int, bool>> bitFilter = (n1,n2) => (n1 & n2) == n2;

            var param1 = Expression.Parameter(typeof(int), "measure");

            //expr1.Parameters.
            

            BinaryExpression greaterThanExpression = Expression.And(
                left: expr1, right: expr2); // int32 > 0

            var result = Expression.And(expr1, expr2);

            //Func<int, bool> result2 = result.Compile();

            //Expression.Or(
            //Expression.ExclusiveOr

        }
    }
}
