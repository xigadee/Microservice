using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Xigadee
{
    /// <summary>
    /// This extension pipeline is used by the Web Api pipeline.
    /// </summary>
    public class WebApiMicroservicePipeline: MicroservicePipeline
    {
        #region Declarations
        /// <summary>
        /// Eat me!
        /// </summary>
        protected MicroservicePipeline mInnerPipeline;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipeline"></param>
        public WebApiMicroservicePipeline(MicroservicePipeline pipeline, HttpConfiguration config) : base(pipeline.Service, pipeline.Configuration)
        {
            mInnerPipeline = pipeline;
            ApiConfig = config ?? new HttpConfiguration();
        } 
        #endregion

        #region ApiConfig
        /// <summary>
        /// This is the http configuration class used for the Web Api instance.
        /// </summary>
        public HttpConfiguration ApiConfig { get; protected set; }
        #endregion

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }
    }
}
