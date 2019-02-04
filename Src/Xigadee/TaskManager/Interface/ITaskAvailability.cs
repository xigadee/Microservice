using System;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to share the Microservice availability statistics.
    /// </summary>
    public interface ITaskAvailability
    {
        int Level(int priority);

        int LevelMin { get; }

        int LevelMax { get; }

        bool ReservationMake(Guid id, int priority, int taken);

        bool ReservationRelease(Guid id);

        int ReservationsAvailable(int priority);
    }
}
