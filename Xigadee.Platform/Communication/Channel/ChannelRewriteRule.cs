using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The rewrite rule is used to reroute incoming messages to their command destination.
    /// </summary>
    public class ChannelRewriteRule
    {
        /// <summary>
        /// This is the private constructor for the static method.
        /// </summary>
        private ChannelRewriteRule()
        {
            Match = (p) => false;
            Rewrite = (p) => { };
            CanCache = true;
        }

        /// <summary>
        /// This is the public constructor.
        /// </summary>
        /// <param name="match">The match function.</param>
        /// <param name="rewrite">The rewrite action.</param>
        /// <param name="canCache">Specifies whether the rewrite hit can be cached.</param>
        public ChannelRewriteRule(Func<TransmissionPayload, bool> match
            , Action<TransmissionPayload> rewrite
            , bool canCache = true)
        {
            if (match == null)
                throw new ArgumentNullException("match cannot be null.");
            Match = match;

            if (rewrite == null)
                throw new ArgumentNullException("rewrite cannot be null.");
            Rewrite = rewrite;

            CanCache = canCache;
        }


        /// <summary>
        /// This is the unique Id for the rule.
        /// </summary>
        public Guid Id => Guid.NewGuid();
        
        /// <summary>
        /// This method states whether it is a match for the incoming message.
        /// </summary>
        /// <param name="inMessage"></param>
        /// <returns></returns>
        public bool IsMatch(TransmissionPayload inMessage)
        {
            return Match(inMessage);
        }

        /// <summary>
        /// This action rewrites the header to the new value.
        /// </summary>
        public Action<TransmissionPayload> Rewrite { get; }
        /// <summary>
        /// This function returns true if the message should be rewritten.
        /// </summary>
        public Func<TransmissionPayload, bool> Match { get; }
        /// <summary>
        /// This property specifies whether the match can be cached.
        /// </summary>
        public bool CanCache { get; }

        /// <summary>
        /// This static method can be used to provide an rewrite placeholder that doesn't do anything. This 
        /// allows for fast look up for incoming messages that have already been scanned.
        /// </summary>
        public static ChannelRewriteRule DoNothing => new ChannelRewriteRule();
    }
}
