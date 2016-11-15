#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base class used to populate and set the correct configuration for the service.
    /// </summary>
    /// <typeparam name="M">The Microservice type.</typeparam>
    /// <typeparam name="C">The Configuration type.</typeparam>
    [Obsolete("Use the Microservice pipeline.")]
    public abstract class PopulatorBase<M, C>: IPopulator
        where M : Microservice, new()
        where C : ConfigBase, new()
    {
        #region Declarations
        /// <summary>
        /// This list contains the set of resource profiles used in the populator.
        /// </summary>
        protected IList<ResourceProfile> mResourceProfiles = new List<ResourceProfile>();

        private bool mConfigInitialised = false;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor that creates.
        /// </summary>
        protected PopulatorBase()
        {
            Service = new M();
            Service.StartRequested += ServiceStartRequested;
            Service.StartCompleted += ServiceStartCompleted;
            Service.StopRequested += ServiceStopRequested;
            Service.StopCompleted += ServiceStopCompleted;
            Service.StatisticsIssued += ServiceStatisticsIssued;
            Service.StatusChanged += ServiceStatusChanged;
        }
        #endregion

        /// <summary>
        /// This method is called when the service issues new statistics.
        /// You can use this method to log specific events or metrics.
        /// </summary>
        /// <param name="service">The issuing service.</param>
        /// <param name="statistics">The statistics.</param>
        protected virtual void ServiceStatisticsProcess(M service, MicroserviceStatistics statistics) { }

        protected virtual void ServiceStartRequested(object sender, StartEventArgs e) { }

        protected virtual void ServiceStartCompleted(object sender, StartEventArgs e) { }

        protected virtual void ServiceStopCompleted(object sender, StopEventArgs e) { }

        protected virtual void ServiceStopRequested(object sender, StopEventArgs e) { }

        protected virtual void ServiceStatusChanged(object sender, StatusChangedEventArgs e)
        {            
        }

        #region ServiceStatisticsIssued(object sender, StatisticsEventArgs e)
        /// <summary>
        /// This method is called when the service statistics are issued by the underlying service. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceStatisticsIssued(object sender, StatisticsEventArgs e)
        {
            try
            {
                ServiceStatisticsProcess(sender as M, e.Statistics);
            }
            catch (Exception)
            {
                //We do not throw exceptions here
            }
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// This method starts the underlying Microservice.
        /// </summary>
        public virtual void Start()
        {
            Service.Start();
        }
        /// <summary>
        /// This method stops the underlying Microservice.
        /// </summary>
        public virtual void Stop()
        {
            Service.Stop();
        }
        #endregion

        #region Service
        /// <summary>
        /// This is the Microservice.
        /// </summary>
        public Microservice Service { get; private set; }
        #endregion
        #region ServiceInitialise()
        /// <summary>
        /// This method is used to set the core Microservice settings.
        /// </summary>
        protected virtual void ServiceInitialise()
        {
        } 
        #endregion

        #region Config
        /// <summary>
        /// This is the system configuration.
        /// </summary>
        public C Config { get; private set; }
        #endregion
        #region ConfigInitialise(Func<string, string, string> resolver, bool resolverFirst)
        /// <summary>
        /// This method can be overriden to customise the config class.
        /// </summary>
        /// <param name="resolver">The resolver function used to set the key values from the appropriate store.</param>
        /// <param name="resolverFirst">A boolean property that determines whether the resolver is called first before falling back to the settings classes.</param>
        protected virtual void ConfigInitialise(Func<string, string, string> resolver, bool resolverFirst)
        {
            //We only want to do this once.
            if (!mConfigInitialised)
            {
                mConfigInitialised = true;

                Config = new C();
                if (resolver != null)
                {
                    //Config.PriorityAppSettings.HasValue
                    int priority = Config.PriorityAppSettings ?? 10;

                    if (Config.PriorityAppSettings.HasValue)
                    {
                        do
                        {
                            if (resolverFirst)
                                priority++;
                            else
                                priority--;

                        }
                        while (Config[priority] != null);
                    }

                    Config.ResolverSet(priority, new ConfigResolverFunction(resolver));
                }
            }
        }
        #endregion

        #region Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        /// <summary>
        /// This is the main method used to populate.
        /// </summary>
        /// <param name="resolver">The settings resolver.</param>
        /// <param name="resolverFirst">A boolean property that determines whether the resolver is called first.</param>
        public virtual void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        {
            ConfigInitialise(resolver, resolverFirst);

            ServiceInitialise();

            RegisterBoundaryLogger();
            RegisterResourceProfiles();
            RegisterSerializers();
            RegisterTelemetry();
            RegisterLogging();
            RegisterEventSources();

            RegisterSecurity();

            RegisterCommands();

            RegisterCommunication();
        } 
        #endregion

        /// <summary>
        /// This shortcut can be used for registering boundary loggers.
        /// </summary>
        protected virtual void RegisterBoundaryLogger()
        {
        }
        /// <summary>
        /// This shortcut can be used to register resource profilers.
        /// </summary>
        protected virtual void RegisterResourceProfiles()
        {
        }

        /// <summary>
        /// This method registers the serializers. By default the JsonContractSerializer is registered.
        /// Overload this method if you do not want to use this serializer.
        /// </summary>
        protected virtual void RegisterSerializers()
        {
            Service.RegisterPayloadSerializer(new JsonContractSerializer());
        }

        protected virtual void RegisterCommunication()
        {
        }

        protected virtual void RegisterCommands()
        {
        }

        protected virtual void RegisterSecurity()
        {
        }

        protected virtual void RegisterTelemetry()
        {
        }

        protected virtual void RegisterLogging()
        {
        }

        protected virtual void RegisterEventSources()
        {
        }
    }
}
