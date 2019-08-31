using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the linked list for bracket definition.
    /// </summary>
    [DebuggerDisplay("{Start}->{End}|I={IndentLevel}|P={HasParent} '{FilterFragment}'")]
    public class BracketDefinition
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="filter"></param>
        /// <param name="parent"></param>
        public BracketDefinition(int start, string filter, BracketDefinition parent)
        {
            Start = start;
            Filter = filter;
            Parent = parent;
        }
        /// <summary>
        /// This method is set when the end bracket is reached.
        /// </summary>
        /// <param name="end">The end bracket position.</param>
        /// <param name="level">The current indent level.</param>
        public void Finish(int end, int level)
        {
            End = end;
            IndentLevel = level;
            if (HasParent)
                Parent.Children.Add(this);
        }
        /// <summary>
        /// This is the optional parent if the string is indented.
        /// </summary>
        public BracketDefinition Parent { get; }
        /// <summary>
        /// Specifies whether the bracket has children.
        /// </summary>
        public List<BracketDefinition> Children { get; } = new List<BracketDefinition>();

        /// <summary>
        /// Specifies whether it has a parent defined.
        /// </summary>
        public bool HasParent => Parent != null;
        /// <summary>
        /// Specifies whether this has a children.
        /// </summary>
        public bool HasChildren => Children.Count > 0;

        /// <summary>
        /// Specifies whether this is a duplicate and can be removed. This happens when we have a section
        /// with extra brackets.
        /// </summary>
        public bool CanRemove => Children.Count == 1 && IsBracketStart && IsBracketEnd
            && FilterFragment.Substring(1, FilterFragment.Length - 2) == Children[0].FilterFragment;

        private bool IsBracketStart => FilterFragment.StartsWith("(");

        private bool IsBracketEnd => FilterFragment.EndsWith(")");
        /// <summary>
        /// This is the start position
        /// </summary>
        public int Start { get; }
        /// <summary>
        /// This is the endposition
        /// </summary>
        public int? End { get; private set; }

        /// <summary>
        /// This is the shared raw filter.
        /// </summary>
        public string Filter { get; private set; }

        public string FilterValue => Filter.Substring(Start, End.Value - Start).Trim();

        /// <summary>
        /// This is the parsed filter section with the bracketed section trimmed out.
        /// </summary>
        public string FilterFragment => Filter.Substring(Start + 1, End.Value - Start - 1).Trim();

        public bool IsLeafVertex => FilterIsLeaf(FilterFragment);
        /// <summary>
        /// This is the bracket indent level.
        /// </summary>
        public int IndentLevel { get; private set; }

        /// <summary>
        /// This method parses out the brackets in the logical expression.
        /// </summary>
        /// <param name="filter">The incoming filter.</param>
        /// <param name="bracketsOut">The position of the start and end brackets.</param>
        /// <returns>Returns the parsed string.</returns>
        public static string Parse(string filter, out List<BracketDefinition> bracketsOut)
        {

            //Remove any whitespace
            filter = filter.Trim();

            var posStack = new Stack<BracketDefinition>();
            var brackets = new List<BracketDefinition>();

            Action<int> push = (int pos) => {
                var bracket = new BracketDefinition(pos, filter, posStack.Count > 0 ? posStack.Peek() : null);
                brackets.Add(bracket);
                posStack.Push(bracket);
            };

            Action<int> pull = (int pos) => {
                var start = posStack.Pop();
                start.Finish(pos, posStack.Count);
            };

            //Set the first bracket as root.
            push(0);

            var sb = new StringBuilder();

            ProcessFilter(filter, push, pull, (c) => sb.Append(c));

            //Remove the first bracket.
            pull(filter.Length - 1);

            bracketsOut = brackets;

            //Return the filtered string with the brackets removed.
            return sb.ToString();
        }

        private static bool FilterIsLeaf(string filter)
        {
            bool isLeaf = true;

            ProcessFilter(filter, (i) => isLeaf = false, (i) => { }, null, () => isLeaf);

            return isLeaf;
        }

        private static void ProcessFilter(string filter, Action<int> start, Action<int> end, Action<char> increment = null, Func<bool> cont = null)
        {
            bool withinStringLiteral = false;

            for (int pos = 0; pos < filter.Length && (cont?.Invoke() ?? true); pos++)
            {
                var ch = filter[pos];
                switch (ch)
                {
                    case '\'':
                        //We may get brackets within a text section. We should not strip them out.
                        withinStringLiteral = !withinStringLiteral;
                        increment?.Invoke(ch);
                        break;
                    case '(':
                        if (withinStringLiteral)
                            increment?.Invoke(ch);
                        else
                            start(pos);
                        break;
                    case ')':
                        if (withinStringLiteral)
                            increment?.Invoke(ch);
                        else
                            end(pos);
                        break;
                    default:
                        increment?.Invoke(ch);
                        break;
                }
            }
        }
    }

}
