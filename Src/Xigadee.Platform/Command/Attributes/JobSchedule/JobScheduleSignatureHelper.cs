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
    public static class JobScheduleSignatureHelper
    {
        /// <summary>
        /// This static helper returns the list of attributes, methods and references. Not one method may have multiple attributes assigned to them.
        /// </summary>
        public static List<Tuple<A, JobScheduleMethodSignature<A>, string>> ScheduleMethodAttributeSignatures<A>(
            this ICommand command, bool throwExceptions = false) 
            where A: JobScheduleAttributeBase
        {
            return command
                .ScheduleMethodSignatures<A>(throwExceptions)
                .SelectMany((s) => s.CommandAttributes.Select((a) => new Tuple<A, JobScheduleMethodSignature<A>, string>(a, s, s.Reference(a))))
                .ToList();
        }

        /// <summary>
        /// This static helper returns the 
        /// </summary>
        public static List<JobScheduleMethodSignature<A>> ScheduleMethodSignatures<A>(this ICommand command, bool throwExceptions)
            where A : JobScheduleAttributeBase
        {
            var results = command.CommandMethods<A>()
                .Select((m) => new JobScheduleMethodSignature<A>(command, m, throwExceptions))
                .Where((t) => t.IsValid)
                .ToList();

            return results;
        }

    }
}
