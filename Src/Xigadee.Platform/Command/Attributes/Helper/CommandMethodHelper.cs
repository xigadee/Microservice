using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xigadee
{
    /// <summary>
    /// This helper returns the appropriate reflection helpers for the commands that use attributes to map to a command method.
    /// </summary>
    public static class CommandMethodHelper
    {
        #region ValidateAttribute<A>(this CommandMethodSignature signature, bool throwException = false)
        /// <summary>
        /// Validates the attribute.
        /// </summary>
        /// <typeparam name="A">The attribute type.</typeparam>
        /// <param name="signature">The signature.</param>
        /// <param name="throwException">if set, throw an exception instead of returning false.</param>
        /// <returns>Returns true if the attribute is set for the method.</returns>
        /// <exception cref="CommandContractSignatureException"></exception>
        public static bool ValidateAttribute<A>(this CommandSignatureBase signature, bool throwException = false) where A : CommandMethodAttributeBase
        {
            var CommandAttributes = Attribute.GetCustomAttributes(signature.Method)
                .Where((a) => a is A)
                .Cast<A>()
                .ToList();

            //This shouldn't happen, but check anyway.
            if (CommandAttributes.Count == 0)
                if (throwException)
                    throw new CommandContractSignatureException($"Attributes of type '{typeof(A).Name}' are not defined for the method.");
                else
                    return false;

            return true;
        }
        #endregion
        #region Reference<A,S>(this CommandMethodHolder<A,S> holder)
        /// <summary>
        /// This is the command reference.
        /// </summary>
        /// <param name="holder">The method holder.</param>
        /// <returns>The reference id.</returns>
        public static string Reference<A,S>(this CommandMethodHolder<A,S> holder)
            where A : CommandMethodAttributeBase
            where S : CommandSignatureBase
        {
            return $"{holder.Signature.Command.FriendlyName}/{holder.Signature.Method.Name}/{holder.Attribute.Reference}";
        }
        #endregion

        private static CommandMethodHolder<A,S> GetMethodHolder<A,S>(ICommand command, (A attr, MethodInfo method) input, bool throwSignatureException = false)
            where A : CommandMethodAttributeBase
            where S : CommandSignatureBase, new()
        {
            var signature = new S();
            signature.Initialise(command, input.method, throwSignatureException);

            return new CommandMethodHolder<A, S> { Attribute = input.attr, Signature = signature };
        }

        private static S GetSignature<S>(ICommand command, MethodInfo method, bool throwSignatureException = false)
            where S : CommandSignatureBase, new()
        {
            var signature = new S();
            signature.Initialise(command, method, throwSignatureException);
            return signature;
        }

        /// <summary>
        /// This static helper returns the 
        /// </summary>
        public static IEnumerable<CommandMethodHolder<A, S>> CommandMethodSignatures<A, S>(this ICommand command, bool throwExceptions)
            where A : CommandMethodAttributeBase
            where S : CommandSignatureBase, new()
        {
            return command.CommandMethods<A>()
                .Select((m) => GetMethodHolder<A,S>(command, m, throwExceptions))
                .Where((t) => t.Signature.IsValid);
        }

        /// <summary>
        /// This static helper returns the list of methods that are decorated with the attribute type.
        /// </summary>
        public static IEnumerable<(A,MethodInfo)> CommandMethods<A>(this ICommand command)
            where A : Attribute
        {
            return command.GetType().CommandMethods<A>();
        }

        /// <summary>
        /// This static helper returns the list of methods that are decorated with the attribute type.
        /// </summary>
        public static IEnumerable<(A,MethodInfo)> CommandMethods<A>(this Type objectType)
            where A : Attribute
        {
            return objectType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Count((a) => a.AttributeType == typeof(A)) > 0)
                .SelectMany((m) => m.GetCustomAttributes(), (m,a) => ((A)a,m));
        }
    }
}
