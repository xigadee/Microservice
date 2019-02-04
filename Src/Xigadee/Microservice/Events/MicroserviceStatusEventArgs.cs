using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to log status change for the Microservice during start up and stop requests.
    /// </summary>
    public class MicroserviceStatusEventArgs:EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceStatusEventArgs"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="title">The title.</param>
        public MicroserviceStatusEventArgs(MicroserviceComponentStatusChangeAction status, string title)
        {
            Status = status;
            Title = title;
        }
        /// <summary>
        /// Gets the status.
        /// </summary>
        public MicroserviceComponentStatusChangeAction Status { get;}
        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public MicroserviceComponentStatusChangeState State { get; set; } =  MicroserviceComponentStatusChangeState.Beginning;
        /// <summary>
        /// Gets or sets the last exception.
        /// </summary>
        public MicroserviceStatusChangeException Ex { get; set; }
        /// <summary>
        /// Displays the status as a debug string.
        /// </summary>
        /// <returns></returns>
        public string Debug()
        {
            return $"{Status}: {Title} = {State}";
        }
    }

    /// <summary>
    /// This is the status change action type for a Microservice component.
    /// </summary>
    public enum MicroserviceComponentStatusChangeAction
    {
        /// <summary>
        /// The component service is starting
        /// </summary>
        Starting,
        /// <summary>
        /// The component service is stopping
        /// </summary>
        Stopping
    }

    /// <summary>
    /// This enumeration is used to identify a generic state change for a component 
    /// </summary>
    public enum MicroserviceComponentStatusChangeState
    {
        /// <summary>
        /// The component is beginning a state change.
        /// </summary>
        Beginning,
        /// <summary>
        /// The component has completed it's state change.
        /// </summary>
        Completed,
        /// <summary>
        /// The component has failed to change state.
        /// </summary>
        Failed
    }
}
