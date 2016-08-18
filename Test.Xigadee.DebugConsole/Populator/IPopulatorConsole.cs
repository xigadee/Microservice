using System;
using Xigadee;

namespace Test.Xigadee
{
    internal interface IPopulatorConsole:IPopulator
    {
        ServiceStatus Status { get; }

        IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get; }

        /// <summary>
        /// This event can be used to subscribe to status changes.
        /// </summary>
        event EventHandler<StatusChangedEventArgs> StatusChanged;

        string Name { get; }
    }
}