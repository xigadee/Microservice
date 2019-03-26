namespace Xigadee
{
    /// <summary>
    /// This is the base API application context class.
    /// </summary>
    /// <typeparam name="MSCONF">The microservice configuration type.</typeparam>
    /// <typeparam name="MODSEC">The user security module type.</typeparam>
    /// <typeparam name="CONATHZ">The authorization configuration type.</typeparam>
    public abstract class JwtApiStartUpContextBase<MSCONF, MODSEC, CONATHZ> : ApiStartUpContextBase<MSCONF, MODSEC, ConfigAuthenticationJwt, CONATHZ>
        where MSCONF : ConfigApiService, new()
        where MODSEC : IApiUserSecurityModule
        where CONATHZ : ConfigAuthorization, new()
    {

    }
}
