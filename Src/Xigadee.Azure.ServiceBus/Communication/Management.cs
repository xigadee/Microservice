
//#region using
//using System;
//using Microsoft.Azure.ServiceBus;
//using Microsoft.Azure.Management;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This is the specific helper for Azure ServiceBus Queue and Topics.
//    /// </summary>
//    public static partial class AzureServiceBusManagement
//    {
//        #region QueueDescriptionGet(string cName)
//        /// <summary>
//        /// This is the default queue description. You should override this if you need a different
//        /// definition.
//        /// </summary>
//        /// <param name="cName">The queue name.</param>
//        /// <returns>Returns a QueueDescription object with the default settings.</returns>
//        public static QueueDescription QueueDescriptionGet(string cName
//            , TimeSpan? defaultMessageTTL = null
//            , TimeSpan? lockDuration = null
//            )
//        {
//            return new QueueDescription(cName)
//            {
//                  EnableDeadLetteringOnMessageExpiration = true
//                , LockDuration = lockDuration ?? TimeSpan.FromMinutes(5)
//                , SupportOrdering = true
//                , EnableBatchedOperations = true
//                , DefaultMessageTimeToLive = defaultMessageTTL ?? TimeSpan.FromDays(7)
//                , MaxSizeInMegabytes = 5120

//                //, EnablePartitioning = true
//            };
//        }
//        #endregion

//        #region QueueFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName)
//        /// <summary>
//        /// This method creates the queue if it doesn't already exist.
//        /// </summary>
//        public static QueueDescription QueueFabricInitialize(this AzureServiceBusConnection conn, string name
//            , TimeSpan? defaultMessageTTL = null
//            , TimeSpan? lockDuration = null
//        )
//        {
//            if (!conn.NamespaceManager.QueueExists(name))
//            {
//                try
//                {
//                    return conn.NamespaceManager.CreateQueue(QueueDescriptionGet(name, defaultMessageTTL, lockDuration));
//                }
//                catch (MessagingEntityAlreadyExistsException)
//                {
//                    // Another service created it before we did - just retrieve the one it created
//                }
//            }

//            return conn.NamespaceManager.GetQueue(name);
//        }
//        #endregion

//        #region TopicDescriptionGet(string cName)
//        public static TopicDescription TopicDescriptionGet(string cName
//            , TimeSpan? defaultMessageTTL = null
//            )
//        {
//            return new TopicDescription(cName)
//            {
//                EnableBatchedOperations = true
//                ,
//                EnableFilteringMessagesBeforePublishing = false
//                ,
//                SupportOrdering = true
//                ,
//                DefaultMessageTimeToLive = defaultMessageTTL ?? TimeSpan.FromDays(7)
//                ,
//                MaxSizeInMegabytes = 5120
//                //,EnablePartitioning = true
//            };
//        }
//        #endregion

//        #region SubscriptionDescriptionGet(string tPath, string sName)
//        public static SubscriptionDescription SubscriptionDescriptionGet(string tPath, string sName
//            , TimeSpan? autoDelete = null
//            , TimeSpan? defaultMessageTTL = null
//            , TimeSpan? lockDuration = null
//            )
//        {
//            return new SubscriptionDescription(tPath, sName)
//            {
//                EnableDeadLetteringOnMessageExpiration = true,
//                EnableDeadLetteringOnFilterEvaluationExceptions = false,
//                LockDuration = lockDuration ?? TimeSpan.FromMinutes(5),
//                EnableBatchedOperations = true,
//                DefaultMessageTimeToLive = defaultMessageTTL ?? TimeSpan.FromDays(7),
//                AutoDeleteOnIdle = autoDelete ?? TimeSpan.MaxValue
//            };
//        }
//        #endregion

//        #region TopicFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName)
//        /// <summary>
//        /// This method creates the topic if it does not exist.
//        /// </summary>
//        public static TopicDescription TopicFabricInitialize(this AzureServiceBusConnection conn, string name
//            , TimeSpan? defaultMessageTTL = null
//            )
//        {
//            if (!conn.NamespaceManager.TopicExists(name))
//            {
//                try
//                {
//                    return conn.NamespaceManager.CreateTopic(TopicDescriptionGet(name, defaultMessageTTL));
//                }
//                catch (MessagingEntityAlreadyExistsException)
//                {
//                    // Another service created it before we did - just retrieve the one it created
//                }
//            }

//            return conn.NamespaceManager.GetTopic(name);
//        }
//        #endregion

//        #region SubscriptionFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName, string mSubscriptionId)
//        /// <summary>
//        /// This method creates the subscription if it does not exist.
//        /// </summary>
//        public static SubscriptionDescription SubscriptionFabricInitialize(this AzureServiceBusConnection conn, string name, string subscriptionId
//            , TimeSpan? autoDeleteSubscription = null
//            , TimeSpan? defaultMessageTTL = null
//            , TimeSpan? lockDuration = null)
//        {
//            if (!conn.NamespaceManager.SubscriptionExists(name, subscriptionId))
//            {
//                try
//                {
//                    return conn.NamespaceManager.CreateSubscription(
//                        SubscriptionDescriptionGet(name, subscriptionId, autoDeleteSubscription, defaultMessageTTL, lockDuration)
//                        );
//                }
//                catch (MessagingEntityAlreadyExistsException)
//                {
//                    // Another service created it before we did - just retrieve the one it created
//                }
//            }

//            return conn.NamespaceManager.GetSubscription(name, subscriptionId);
//        }
//        #endregion
//    }
//}
