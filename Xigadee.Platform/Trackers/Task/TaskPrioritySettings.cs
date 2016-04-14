using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the priority settings.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class TaskManagerPrioritySettings
    {
        internal TaskManagerPrioritySettings(int level)
        {
            Level = level;
        }

        public readonly int Level;

        public long Count;

        public int Active;

        public int BulkHeadReservation;

        public string Debug
        {
            get
            {
                return $"Level={Level} Hits={Count} Active={Active} Reserved={BulkHeadReservation}";
            }
        }
    }
}
