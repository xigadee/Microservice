using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xigadee
{
    public static class FilterParser
    {
        //public static FilterParserNodeRoot Parse(string filter)
        //{

        //    var tokens = 
            

        //    var root = new FilterParserNodeRoot(filter);

            
            
        //    return root;
        //}


        //private static ODataTokenCollection Tokenise(string filter)
        //{
        //    ODataTokenCollection rootToken;

        //    for (int pos = 0; pos < filter.Length ; pos++)
        //    {
        //        rootToken.Write(


        //    }
        //}
    }

    public abstract class FilterParserNode
    {
        public string Filter { get; protected set; }

    }

    [DebuggerDisplay("{Filter}")]
    public class FilterParserNodeRoot: FilterParserNode
    {
        public FilterParserNodeRoot(string filter)
        {
            Filter = filter;
        }



    }

    public class FilterExpression : FilterOperation
    {

    }

    public class FilterOperation : FilterParserNode
    {
        public FilterLogical Next { get; set; }
    }

    public class FilterLogical : FilterParserNode
    {
        public FilterLogical(ODataLogicalOperators op)
        {
            Operator = op;
        }

        public ODataLogicalOperators Operator { get; }

        public FilterOperation Next { get; set; }

    }

}
