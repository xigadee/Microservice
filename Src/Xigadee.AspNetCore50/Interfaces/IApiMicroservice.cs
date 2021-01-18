//namespace Xigadee
//{

//    /// <summary>
//    /// This interface is implemented by applications that use Xigadee in an API or web based application.
//    /// </summary>
//    /// <typeparam name="MODSEC">The type of the security module.</typeparam>
//    /// <typeparam name="CONATEN">The authentication module type.</typeparam>
//    /// <typeparam name="CONATHZ">The authorization module type.</typeparam>
//    public interface IApiMicroservice<MODSEC, CONATEN, CONATHZ>: IApiMicroserviceBase<MODSEC, CONATEN, CONATHZ>, IApiStartupContext
//        where MODSEC : IApiUserSecurityModule
//        where CONATEN : ConfigAuthentication
//        where CONATHZ : ConfigAuthorization
//    {

//    }
//}
