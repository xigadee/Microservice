
using System;
using System.Linq;
using Xigadee;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension allows for specific events to be inspected.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="action">An action to process the event.</param>
        /// <param name="supported">A flags enumeration to filter the specific event types that you wish to process. Leave this null to process all event types.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P OnEvent<P>(this P pipeline
            , Action<EventHolder> action
            , DataCollectionSupport? supported = null)
            where P : IPipeline
        {
            if (action == null)
                throw new ArgumentNullException("action", "'action' cannot be null for OnEvent");

            var ms = pipeline.ToMicroservice();
            
            var comp = pipeline.Service.DataCollection.Register(new OnEventDataCollectionComponent(supported, action, ms.Dispatch));

            return pipeline;
        }

        internal class OnEventDataCollectionComponent: DataCollectorBase, IRequirePayloadManagement
        {
            DataCollectionSupport? mSupportedFilter = null;
            Action<EventHolder> mAction;
            IMicroserviceDispatch mDispatcher;

            /// <summary>
            /// Initializes a new instance of the <see cref="OnEventDataCollectionComponent"/> class.
            /// </summary>
            /// <param name="supported">The supported event types.</param>
            /// <param name="action">The action to process a supported event.</param>
            /// <param name="dispatcher">The dispatcher.</param>
            internal OnEventDataCollectionComponent(DataCollectionSupport? supported, Action<EventHolder> action, IMicroserviceDispatch dispatcher)
            {
                mSupportedFilter = supported;
                mAction = action;
                mDispatcher = dispatcher;
            }

            /// <summary>
            /// This is the system wide Payload serializer.
            /// </summary>
            public IPayloadSerializationContainer PayloadSerializer { get; set; }

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

            private void Action(EventHolder holder)
            {
                try
                {
                    mAction(holder);
                }
                catch
                {
                    //We're not interested in catching exceptions here. This is for the pipeline code to sort out.
                }
            }

        }
    }
}
