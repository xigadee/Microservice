using System;

namespace Xigadee
{
    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public abstract class UserSecurityModuleBase : UserSecurityModuleBase<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
    {

    }

    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public abstract class UserSecurityModuleBase<U> : UserSecurityModuleBase<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
        where U : User
    {

    }

    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public abstract class UserSecurityModuleBase<U,USEC,USES,UEXA,UR,UAT> : ApiModuleBase, IApiUserSecurityModule
        where U : User
        where USEC : UserSecurity
        where USES : UserSession
        where UEXA : UserExternalAction
        where UR : UserRoles
        where UAT : UserAccessToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurityModuleBase{U, USEC, USES, UEXA, UR, UAT}"/> class.
        /// Specifically, this sets the lazy initializers for the base interface.
        /// </summary>
        protected UserSecurityModuleBase()
        {
            //This lazy conversion allows you to use a more derived entity for User, User Security etc.
            //but still use the same generic interface to interact with the base object definition.
            //I will probably regret this code :(
            _lazyUsers = GetLazy<Guid, User, U>(() => RepositoryUsers);
            _lazyUserSecurities = GetLazy<Guid, UserSecurity, USEC>(() => RepositoryUserSecurities);
            _lazyUserSessions = GetLazy<Guid, UserSession, USES>(() => RepositoryUserSessions);
            _lazyUserRoles = GetLazy<Guid, UserRoles, UR>(() => RepositoryUserRoles);
            _lazyUserAccessTokens = GetLazy<Guid, UserAccessToken, UAT>(() => RepositoryUserAccessTokens);
            _lazyUserExternalActions = GetLazy<Guid, UserExternalAction, UEXA>(() => RepositoryUserExternalActions);
        }

        #region Repositories
        /// <summary>
        /// Gets or sets the generic users repository
        /// </summary>
        public virtual RepositoryBase<Guid, U> RepositoryUsers { get; protected set; }
        /// <summary>
        /// Gets or sets the generic user security repository.
        /// </summary>
        public virtual RepositoryBase<Guid, USEC> RepositoryUserSecurities { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user sessions.
        /// </summary>
        public virtual RepositoryBase<Guid, USES> RepositoryUserSessions { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user external actions.
        /// </summary>
        public virtual RepositoryBase<Guid, UEXA> RepositoryUserExternalActions { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user roles.
        /// </summary>
        public virtual RepositoryBase<Guid, UR> RepositoryUserRoles { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user access tokens.
        /// </summary>
        public virtual RepositoryBase<Guid, UAT> RepositoryUserAccessTokens { get; protected set; }
        #endregion

        #region Lazy conversion
        readonly Lazy<IRepositoryAsync<Guid, User>> _lazyUsers;
        readonly Lazy<IRepositoryAsync<Guid, UserSecurity>> _lazyUserSecurities;
        readonly Lazy<IRepositoryAsync<Guid, UserSession>> _lazyUserSessions;
        readonly Lazy<IRepositoryAsync<Guid, UserRoles>> _lazyUserRoles;
        readonly Lazy<IRepositoryAsync<Guid, UserAccessToken>> _lazyUserAccessTokens;
        readonly Lazy<IRepositoryAsync<Guid, UserExternalAction>> _lazyUserExternalActions;
        /// <summary>
        /// Gets the lazy. Enough said. A bit of magical generic plumbing due to the need to map derived entites
        /// to their base implementation.
        /// </summary>
        /// <typeparam name="KL">The type of the l.</typeparam>
        /// <typeparam name="ER">The type of the r.</typeparam>
        /// <typeparam name="EL">The type of the l.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <returns>The lazy initializer.</returns>
        private static Lazy<IRepositoryAsync<KL, ER>> GetLazy<KL, ER, EL>(Func<RepositoryBase<KL, EL>> repository)
            where KL : IEquatable<KL>
            where ER : class
            where EL : ER
        {
            if (typeof(ER) == typeof(EL))
                return new Lazy<IRepositoryAsync<KL, ER>>(() => repository() as IRepositoryAsync<KL, ER>);

            return new Lazy<IRepositoryAsync<KL, ER>>(() => new RepositoryBridge<KL, ER, EL>(repository()));
        } 
        #endregion

        #region IApiUserSecurityModule        
        /// <summary>
        /// Gets the users repository
        /// </summary>
        public virtual IRepositoryAsync<Guid, User> Users => _lazyUsers.Value;

        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSecurity> UserSecurities => _lazyUserSecurities.Value;

        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSession> UserSessions => _lazyUserSessions.Value;

        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserRoles> UserRoles => _lazyUserRoles.Value;

        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserAccessToken> UserAccessTokens => _lazyUserAccessTokens.Value;

        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserExternalAction> UserExternalActions => _lazyUserExternalActions.Value;
        #endregion


    }
}
