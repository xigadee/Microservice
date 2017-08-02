using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This helper returns the appropriate helpers for the commands that use attributes to map a command method.
    /// </summary>
    public static class CommandMethodSignatureHelper
    {
        /// <summary>
        /// This static helper returns the list of attributes, methods and references. Not one method may have multiple attributes assigned to them.
        /// </summary>
        public static List<Tuple<A, CommandMethodSignature<A>, string>> CommandMethodAttributeSignatures<A>(
            this ICommand command, bool throwExceptions = false)
            where A: CommandContractAttributeBase
        {
            return command
                .CommandMethodSignatures<A>(throwExceptions)
                .SelectMany((s) => s.CommandAttributes.Select((a) => new Tuple<A, CommandMethodSignature<A>, string>(a,s, s.Reference(a))))
                .ToList();
        }

        /// <summary>
        /// This static helper returns the 
        /// </summary>
        public static List<CommandMethodSignature<A>> CommandMethodSignatures<A>(this ICommand command, bool throwExceptions)
            where A : CommandContractAttributeBase
        {
            var results = command.CommandMethods<A>()
                .Select((m) => new CommandMethodSignature<A>(command, m, throwExceptions))
                .Where((t) => t.IsValid)
                .ToList();

            return results;
        }

        /// <summary>
        /// This static helper returns the list of methods that are decorated with the attribute type.
        /// </summary>
        public static List<MethodInfo> CommandMethods<A>(this ICommand command)
            where A : Attribute
        {
            return command.GetType().CommandMethods<A>();
        }

        /// <summary>
        /// This static helper returns the list of methods that are decorated with the attribute type.
        /// </summary>
        public static List<MethodInfo> CommandMethods<A>(this Type objectType)
            where A : Attribute
        {
            var results = objectType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Count((a) => a.AttributeType == typeof(A)) > 0)
                .ToList();

            return results;
        }

    }
}
