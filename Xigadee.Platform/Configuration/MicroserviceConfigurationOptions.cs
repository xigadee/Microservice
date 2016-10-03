//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Xigadee
//{
//    /// <summary>
//    /// This class is used to set the default options for a Microservice.
//    /// </summary>
//    public class MicroserviceConfigurationOptions
//    {
//        /// <summary>
//        /// This is the default configuration for a Microservice.
//        /// </summary>
//        public MicroserviceConfigurationOptions()
//        {
//            TransitCountMax = 20;
//            //ProcessingTimeMax = TimeSpan.FromSeconds(30);

//            ProcessingTimeTaskDecrease = TimeSpan.FromSeconds(2);
//            ProcessingTimeTaskDecreasePercentage = 0.05F;
//            ProcessingTimeTaskIncrease = TimeSpan.FromMilliseconds(250);
//            ProcessingTimeTaskIncreasePercentage = 0.15F;

//            UnhandledMessagesIgnore = true;

//            StatusLogFrequency = TimeSpan.FromSeconds(15);

//            ConcurrentRequestsMin = Environment.ProcessorCount * 2;
//            ConcurrentRequestsMax = Environment.ProcessorCount * 16;

//            OverloadProcessTimeInMs = 5000;

//            OverloadProcessLimitMin = 2;
//            OverloadProcessLimitMax = ConcurrentRequestsMax /10;

//            ProcessorTargetLevelPercentage = 75;

//            ProcessKillOverrunGracePeriod = TimeSpan.FromSeconds(15);

//            SupportAutotune = false;
//        }

//        /// <summary>
//        /// This specifies whether autotune should be supported.
//        /// </summary>
//        public bool SupportAutotune { get; set; }
//        /// <summary>
//        /// This is the time that a process is marked as killed after it has been marked as cancelled.
//        /// </summary>
//        public TimeSpan ProcessKillOverrunGracePeriod { get; set; }
//        /// <summary>
//        /// This is maximum target percentage usuage limit.
//        /// </summary>
//        public int ProcessorTargetLevelPercentage { get; set; }
//        /// <summary>
//        /// This is the maximum number overload processes permitted.
//        /// </summary>
//        public int OverloadProcessLimitMax { get; set; }
//        /// <summary>
//        /// This is the minimum number of overload processors available.
//        /// </summary>
//        public int OverloadProcessLimitMin { get; set; }
//        /// <summary>
//        /// This is the maximum time that an overload process task can run.
//        /// </summary>
//        public int OverloadProcessTimeInMs { get; set; }
//        /// <summary>
//        /// This is the maximum number of concurrent requests.
//        /// </summary>
//        public int ConcurrentRequestsMax { get; set; }
//        /// <summary>
//        /// This is the minimum number of concurrent requests.
//        /// </summary>
//        public int ConcurrentRequestsMin { get; set; }
//        /// <summary>
//        /// This is the maximum number of hops allowed for a message before it raises an error and is stopped.
//        /// </summary>
//        public int TransitCountMax { get; set; }
//        /// <summary>
//        /// This is the default maximum processing time for a request after which it should be cancelled.
//        /// </summary>
//        public TimeSpan DefaultProcessingTimeMax { get; set; }
//        /// <summary>
//        /// This is the target time after which if exceeded the number of concurrent tasks should be decreased.
//        /// </summary>
//        public TimeSpan? ProcessingTimeTaskDecrease { get; set; }

//        public float ProcessingTimeTaskDecreasePercentage { get; set; }
//        /// <summary>
//        /// This is the target time after which the number of concurrent tasks should be decreased.
//        /// </summary>
//        public TimeSpan? ProcessingTimeTaskIncrease { get; set; }

//        public float ProcessingTimeTaskIncreasePercentage { get; set; }
//        /// <summary>
//        /// This is the frequency that status log entries are generated
//        /// </summary>
//        public TimeSpan StatusLogFrequency { get; set; }
//        /// <summary>
//        /// This flag ignores unhandled messages and marks then as complete from the queue.
//        /// </summary>
//        public bool UnhandledMessagesIgnore { get; set; }
//    }
//}
