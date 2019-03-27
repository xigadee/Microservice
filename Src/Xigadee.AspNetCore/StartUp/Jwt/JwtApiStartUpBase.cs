//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;

//namespace Xigadee
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="CTX">The type of the tx.</typeparam>
//    /// <typeparam name="MSCONF">The type of the sconf.</typeparam>
//    /// <typeparam name="MODSEC">The type of the odsec.</typeparam>
//    /// <typeparam name="CONATHZ">The type of the onathz.</typeparam>
//    public abstract class JwtApiStartUpBase<CTX, MSCONF, MODSEC, CONATHZ> : ApiStartupBase<CTX, MODSEC, ConfigAuthenticationJwt, CONATHZ>
//        where CTX : class, IApiMicroservice<MODSEC, ConfigAuthenticationJwt, CONATHZ>, new()
//        where MSCONF : ConfigApplication, new()
//        where MODSEC : IApiUserSecurityModule
//        where CONATHZ : ConfigAuthorization, new()
//    {
//        /// <summary>
//        /// Initializes a new instance of the API application class.
//        /// </summary>
//        /// <param name="env">The environment.</param>
//        protected JwtApiStartUpBase(IHostingEnvironment env):base(env)
//        {
//        }
//    }
//}
