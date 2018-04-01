using System;

namespace Xigadee
{
    /// <summary>
    /// The rewrite rule is used to redirect incoming messages to a different command destination.
    /// </summary>
    public class MessageRedirectRule
    {
        /// <summary>
        /// This is the private constructor for the static method.
        /// </summary>
        private MessageRedirectRule()
        {
            CanRedirect = (p) => false;
            Redirect = (p) => { };
            CanCache = true;
        }

        /// <summary>
        /// This is the public constructor.
        /// </summary>
        /// <param name="canRedirect">The match function.</param>
        /// <param name="redirect">The redirect action.</param>
        /// <param name="canCache">Specifies whether the redirect hit can be cached.</param>
        public MessageRedirectRule(Func<TransmissionPayload, bool> canRedirect
            , Action<TransmissionPayload> redirect
            , bool canCache = true)
        {
            if (canRedirect == null)
                throw new ArgumentNullException("match cannot be null.");
            CanRedirect = canRedirect;

            if (redirect == null)
                throw new ArgumentNullException("rewrite cannot be null.");
            Redirect = redirect;

            CanCache = canCache;
        }


        /// <summary>
        /// This is the unique Id for the rule.
        /// </summary>
        public Guid Id => Guid.NewGuid();
        
        /// <summary>
        /// This action rewrites the header to the new value.
        /// </summary>
        public Action<TransmissionPayload> Redirect { get; }
        /// <summary>
        /// This function returns true if the message should be rewritten.
        /// </summary>
        public Func<TransmissionPayload, bool> CanRedirect { get; }
        /// <summary>
        /// This property specifies whether the match can be cached.
        /// </summary>
        public bool CanCache { get; }

        /// <summary>
        /// This static method can be used to provide an rewrite placeholder that doesn't do anything. This 
        /// allows for fast look up for incoming messages that have already been scanned.
        /// </summary>
        public static MessageRedirectRule DoNothing => new MessageRedirectRule();
    }
}
