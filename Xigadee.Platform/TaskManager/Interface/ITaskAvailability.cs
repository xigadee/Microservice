using System;

namespace Xigadee
{
    public interface ITaskAvailability
    {
        int Level(int priority);

        int LevelMin { get; }

        int LevelMax { get; }

        bool ReservationMake(Guid id, int priority, int taken);

        bool ReservationRelease(Guid id);

        int GetAvailability(int priority, int slotsAvailable);
    }
}
