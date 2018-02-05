using System;
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension allows for specific data collection events to be inspected.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="action">An action to process the event.</param>
        /// <param name="supported">A flags enumeration to filter the specific event types that you wish to process. Leave this null to process all event types.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P OnDataCollection<P>(this P pipeline
            , Action<OnDataCollectionContext,EventHolder> action
            , DataCollectionSupport? supported = null)
            where P : IPipeline
        {
            if (action == null)
                throw new ArgumentNullException("action", "'action' cannot be null for OnEvent");

            var ms = pipeline.ToMicroservice();
            
            var comp = pipeline.Service.DataCollection.Register(new OnEventDataCollectionComponent(supported, action, () => ms.Dispatch));

            return pipeline;
        }

        /// <summary>
        /// This context is used to hold the necessary data for an in-line command request.
        /// </summary>
        public class OnDataCollectionContext : CommandContextBase<IMicroserviceDispatch>
        {
            /// <summary>
            /// This is the default constructor.
            /// </summary>
            /// <param name="serviceHandlers">The service handler container.</param>
            /// <param name="collector">The data collector.</param>
            /// <param name="sharedServices">The shared service context.</param>
            /// <param name="originatorId">This is the Microservice identifiers.</param>
            /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
            public OnDataCollectionContext(
              IServiceHandlers serviceHandlers
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , IMicroserviceDispatch outgoingRequest): base(serviceHandlers, collector, sharedServices, originatorId, outgoingRequest)
            {
            }
        }

        internal class OnEventDataCollectionComponent: DataCollectorBase, IRequireServiceHandlers, IRequireDataCollector
        {
            DataCollectionSupport? mSupportedFilter = null;
            Action<OnDataCollectionContext, EventHolder> mAction;
            Func<IMicroserviceDispatch> mDispatcherFunction;
            OnDataCollectionContext mContext;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="OnEventDataCollectionComponent"/> class.
            /// </summary>
            /// <param name="supported">The supported event types.</param>
            /// <param name="action">The action to process a supported event.</param>
            /// <param name="dispatcher">The dispatcher function.</param>
            internal OnEventDataCollectionComponent(DataCollectionSupport? supported, Action<OnDataCollectionContext, EventHolder> action, Func<IMicroserviceDispatch> dispatcher)
            {
                mSupportedFilter = supported;
                mAction = action;
                mDispatcherFunction = dispatcher;
            }

            /// <summary>
            /// This is the system wide Payload serializer.
            /// </summary>
            public IServiceHandlers ServiceHandlers { get; set; }

            /// <summary>
            /// This method loads the support.
            /// </summary>
            protected override void SupportLoadDefault()
            {
                Enum.GetValues(typeof(DataCollectionSupport))
                    .Cast<DataCollectionSupport>()
                    .Where(s => s != DataCollectionSupport.None)
                    .Where(s => !mSupportedFilter.HasValue || (s & mSupportedFilter.Value) > 0)
                    .ForEach((t) => SupportAdd(t, (e) => Action(e)));
            }

            /// <summary>
            /// This is the context.
            /// </summary>
            protected OnDataCollectionContext Context
            {
                get
                {
                    if (mContext == null)
                    {
                        var d = mDispatcherFunction();
                        mContext = new OnDataCollectionContext(ServiceHandlers, Collector, SharedServices, OriginatorId, d);
                    }

                    return mContext;
                }
            }

            /// <summary>
            /// This is the data collector.
            /// </summary>
            public IDataCollection Collector { get; set; }

            private void Action(EventHolder holder)
            {
                try
                {
                    mAction(Context, holder);
                }
                catch (Exception ex)
                {
                    //We're not interested in catching exceptions here. This is for the pipeline code to sort out.
                }
            }

        }
    }
}
