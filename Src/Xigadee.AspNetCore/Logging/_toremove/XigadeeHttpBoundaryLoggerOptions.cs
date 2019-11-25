//using System;
//using Microsoft.AspNetCore.Http;

//namespace Xigadee
//{
//    /// <summary>
//    /// This class contains the options for the boundary logger.
//    /// </summary>
//    public class XigadeeHttpBoundaryLoggerOptions
//    {
//        /// <summary>
//        /// The logging level needed.
//        /// </summary>
//        public ApiBoundaryLoggingFilterLevel Level { get; set; } = ApiBoundaryLoggingFilterLevel.All;
//        /// <summary>
//        /// The correlation key header.
//        /// </summary>
//        public string CorrelationIdKey { get; set; } = "X-CorrelationId";
//        /// <summary>
//        /// Specifies whether the correlation key should be added to the claims principal
//        /// </summary>
//        public bool AddToClaimsPrincipal { get; set; } = true;
//        /// <summary>
//        /// The current Microservice.
//        /// </summary>
//        public IMicroservice Microservice { get; set; }
//        /// <summary>
//        /// A custom filter function for the specific request.
//        /// </summary>
//        public Func<HttpContext, bool> Filter { get; set; }
//    }
//}
