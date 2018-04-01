using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// The channel class is used to simplify the connection between communication and command components.
    /// </summary>
    [DebuggerDisplay("{Id}->{Direction} (Auto={IsAutoCreated} Internal={InternalOnly} BL={BoundaryLoggingActive}){Description}")]
    public class Channel
    {
        #region Declarations
        /// <summary>
        /// This is the list of destination rewrite rules.
        /// </summary>
        protected ConcurrentDictionary<Guid, MessageRedirectRule> mRedirectRules = new ConcurrentDictionary<Guid, MessageRedirectRule>();
        /// <summary>
        /// This is the cache that is constructed when a rewrite rule is implemented. This is used to speed up the resolution on rules, as ChannelRewriteRules use partial matching.
        /// </summary>
        protected ConcurrentDictionary<ServiceMessageHeader, Guid?> mRedirectCache = new ConcurrentDictionary<ServiceMessageHeader, Guid?>();
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The channel Id.</param>
        /// <param name="direction">The direction of the channel - Incoming or outgoing</param>
        /// <param name="description">The optional description</param>
        /// <param name="bLoggerActive">This is the boundary logger active status for the channel.</param>
        /// <param name="internalOnly">This property specifies that the channel should only be used for internal messaging.</param>
        /// <param name="rewriteRules">A selection of rewrite rules for the channel.</param>
        /// <param name="isAutocreated">A boolean property that specifies whether the channel was created automatically by the communications container.</param>
        public Channel(string id
            , ChannelDirection direction
            , string description = null
            , bool? bLoggerActive = null
            , bool internalOnly = false
            , IEnumerable<MessageRedirectRule> rewriteRules = null
            , bool isAutocreated = false)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id cannot be null or empty.");

            Id = id;
            Direction = direction;
            Description = description;
            InternalOnly = internalOnly;
            BoundaryLoggingActive = bLoggerActive;
            IsAutoCreated = isAutocreated;

            if (rewriteRules != null)
                mRedirectRules = new ConcurrentDictionary<Guid, MessageRedirectRule>(
                    rewriteRules.Select((r) => new KeyValuePair<Guid, MessageRedirectRule>(r.Id,r)));
        } 
        #endregion

        /// <summary>
        /// This boolean property is set if the channel was created by a command request and not explicitly 
        /// during the microservice set up.
        /// </summary>
        public bool IsAutoCreated { get; }
        /// <summary>
        /// The channel Id.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// An optional description of the channel
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The direction of the channel - Incoming or outgoing
        /// </summary>
        public ChannelDirection Direction { get; }

        /// <summary>
        /// This property specifies that the channel should only be used for internal messaging.
        /// </summary>
        public bool InternalOnly { get; }

        /// <summary>
        /// This is the boundary logger active status for the channel. 
        /// If null, the default communication policy will be used.
        /// </summary>
        public bool? BoundaryLoggingActive {get;set;}

        /// <summary>
        /// This is a set of resource profiles attached to the channel.
        /// </summary>
        public List<ResourceProfile> ResourceProfiles { get; set; } = new List<ResourceProfile>();

        /// <summary>
        /// This property contains the partitions set for the channel. This is IEnumerable to enable covariant support.
        /// </summary>
        public IEnumerable<PartitionConfig> Partitions { get; set; }  = null;

        /// <summary>
        /// This method adds a rewrite rule to the channel.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>Returns true if the rule has been added.</returns>
        public bool RedirectAdd(MessageRedirectRule rule)
        {
            bool success = mRedirectRules.TryAdd(rule.Id, rule);
            if (success)
                mRedirectCache.Clear();

            return success;
        }
        /// <summary>
        /// This method removes a rule from the collection.
        /// </summary>
        /// <param name="ruleId">The rule to be removed based on its id.</param>
        /// <returns>Returns true if the rule has been removed.</returns>
        public bool RedirectRemove(Guid ruleId)
        {
            MessageRedirectRule rule;
            bool success = mRedirectRules.TryRemove(ruleId, out rule);
            if (success)
                mRedirectCache.Clear();
            return success;
        }

        /// <summary>
        /// This is the set of rules currently held by the channel.
        /// </summary>
        public IEnumerable<MessageRedirectRule> Redirects
        {
            get
            {
                return mRedirectRules.Values;
            }
        }

        /// <summary>
        /// This method returns true if the channel has rewrite rules set.
        /// </summary>
        public bool CouldRedirect
        {
            get
            {
                return mRedirectRules.Count>0;
            }
        }

        /// <summary>
        /// This method rewrites the rule.
        /// </summary>
        /// <param name="payload">The payload to adjust the incoming header information.</param>
        public void Redirect(TransmissionPayload payload)
        {
            var header = payload.Message.ToServiceMessageHeader();

            Guid? id = null;

            if (!mRedirectCache.ContainsKey(header))
                id = RedirectBuildCacheEntry(header, payload);
            else
                id = mRedirectCache[header];

            //There is an entry, but this may be null if there isn't a match.
            if (id.HasValue)
            {
                mRedirectRules[id.Value].Redirect(payload);
                payload.TraceWrite("Rule matched: {id.Value}", "Channel/Redirect");

            }

            return;
        }

        /// <summary>
        /// This method specifically builds the redirect cache.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="payload"></param>
        private Guid? RedirectBuildCacheEntry(ServiceMessageHeader header, TransmissionPayload payload)
        {
            Guid? id = null;

            var result = mRedirectRules.Where((r) => r.Value.CanRedirect(payload)).ToList();

            id = (result.Count == 0)?default(Guid?):result[0].Key;

            if (id.HasValue && result[0].Value.CanCache)
                mRedirectCache.AddOrUpdate(header, id, (h, g) => id);

            return id;
        }

        /// <summary>
        /// This identifier is set when we require an symmetric encryption to be applied to the traffic.
        /// </summary>
        public EncryptionHandlerId Encryption { get; set; }

        /// <summary>
        /// This identifier is set when we require authentication of the incoming message.
        /// </summary>
        public AuthenticationHandlerId Authentication { get; set; }
    }
}
