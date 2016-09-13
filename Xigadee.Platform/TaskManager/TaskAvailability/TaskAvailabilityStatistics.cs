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

namespace Xigadee
{
    /// <summary>
    /// This is the statistics class for Task Availability
    /// </summary>
    public class TaskAvailabilityStatistics:StatusBase
    {
        public int TasksMaxConcurrent { get; set; }

        public int SlotsAvailable { get; set; }

        public string[] Levels { get; set; }

        public int Killed { get; set; }

        public long KilledDidReturn { get; set; }

        public int Active { get; set; }


        public string[] Running { get; set; }

        public string[] Reservations { get; set; }
    }
}
