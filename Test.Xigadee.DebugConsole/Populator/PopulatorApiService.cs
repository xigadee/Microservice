using System;
using Microsoft.Owin.Hosting;
using Xigadee;

namespace Test.Xigadee
{
    public class PopulatorApiService: IPopulatorConsole
    {
        public Microservice Service
        {
            get
            {
                return null;
            }
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        {

        }

        public Uri ApiUri { get; set; }

        /// <summary>
        /// This is the Api instancer service.
        /// </summary>
        public IDisposable ApiServer { get; set; }

        public ServiceStatus Status { get; private set; } = ServiceStatus.Created;

        private void OnStatusChanged(ServiceStatus status, string message = null)
        {

            var oldStatus = Status;
            Status = status;

            StatusChanged?.Invoke(this, new StatusChangedEventArgs() { StatusNew = status, StatusOld = oldStatus, Message = message });
        }

        public void Start()
        {
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add(ApiUri.ToString());
                OnStatusChanged(ServiceStatus.Starting, $" @ {ApiUri.ToString()}");

                Persistence = new ApiProviderAsyncV2<Guid, MondayMorningBlues>(ApiUri);
                ApiServer = WebApp.Start<Test.Xigadee.Api.Server.Startup>(options);

                OnStatusChanged(ServiceStatus.Running);

            }
            catch (Exception ex)
            {
                OnStatusChanged(ServiceStatus.Stopped, $" Error: {ex.Message}");

                ApiServer = null;
            }
        }

        public void Stop()
        {
            OnStatusChanged(ServiceStatus.Stopping);

            Persistence = null;
            ApiServer.Dispose();
            ApiServer = null;

            OnStatusChanged(ServiceStatus.Stopped);
        }

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence
        {
            get; private set;
        }

        public string Name
        {
            get
            {
                return "ApiService";
            }
        }
    }
}
