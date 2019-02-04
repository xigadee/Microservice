//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Xigadee
//{
//    /// <summary>
//    /// The security container class contains all the components to secure the incoming messaging for a Microservice, 
//    /// and to ensure that incoming message requests have the correct permissions necessary to be processed.
//    /// </summary>
//    public partial class SecurityContainer: ServiceContainerBase<SecurityContainer.Statistics, SecurityContainer.Policy>
//        , ISecurityService
//        , IRequireDataCollector, IRequireServiceOriginator
//    {
//        #region Declarations
//        /// <summary>
//        /// These are the handlers used to encrypt and decrypt blob payloads
//        /// </summary>
//        private Dictionary<string, IServiceHandlerEncryption> mEncryptionHandlers;
//        /// <summary>
//        /// These are the handlers used to authenticate the incoming payloads, and
//        /// sign the outgoing payloads.
//        /// </summary>
//        private Dictionary<string, IServiceHandlerAuthentication> mAuthenticationHandlers;
//        #endregion        
//        #region Constructor
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="policy">The security policy.</param>
//        public SecurityContainer(SecurityContainer.Policy policy) : base(policy)
//        {
//            mEncryptionHandlers = new Dictionary<string, IServiceHandlerEncryption>();
//            mAuthenticationHandlers = new Dictionary<string, IServiceHandlerAuthentication>();
//        }
//        #endregion

//        #region StatisticsRecalculate(SecurityContainerStatistics statistics)
//        /// <summary>
//        /// This method updates the security statistics.
//        /// </summary>
//        /// <param name="statistics">The statistics.</param>
//        protected override void StatisticsRecalculate(SecurityContainer.Statistics statistics)
//        {
//            base.StatisticsRecalculate(statistics);

//            try
//            {
//                statistics.AuthenticationHandlers = mAuthenticationHandlers?.Select((h) => $"{h.Key}: {h.Value.Name}").ToArray();
//                statistics.EncryptionHandlers = mEncryptionHandlers?.Select((h) => $"{h.Key}: {h.Value.Name}").ToArray(); ;
//            }
//            catch (Exception)
//            {

//            }
//        } 
//        #endregion

//        //Authentication
//        #region HasAuthenticationHandler(string identifier)
//        /// <summary>
//        /// This method returns true if the authentication handler can be found.
//        /// </summary>
//        /// <param name="identifier">The identifier.</param>
//        /// <returns></returns>
//        public bool HasAuthenticationHandler(string identifier)
//        {
//            return mAuthenticationHandlers.ContainsKey(identifier);
//        }
//        #endregion
//        #region RegisterAuthenticationHandler(string identifier, IEncryptionHandler handler)
//        /// <summary>
//        /// This method registers an authentication handler with the collection.
//        /// </summary>
//        /// <param name="identifier">The identifier for the handler.</param>
//        /// <param name="handler">The handler to register.</param>
//        public void RegisterAuthenticationHandler(string identifier, IServiceHandlerAuthentication handler)
//        {
//            if (string.IsNullOrEmpty(identifier))
//                throw new ArgumentNullException("identifier");

//            if (handler == null)
//                throw new ArgumentNullException("handler");

//            if (mAuthenticationHandlers.ContainsKey(identifier))
//                throw new AuthenticationHandlerAlreadyExistsException(identifier);

//            try
//            {
//                mAuthenticationHandlers.Add(identifier, handler);
//            }
//            catch (Exception ex)
//            {
//                Collector?.LogException($"{nameof(RegisterAuthenticationHandler)} unexpected error.", ex);
//                throw;
//            }
//        }
//        #endregion

//        //Encryption
//        #region HasEncryptionHandler(string identifier)
//        /// <summary>
//        /// This method returns true if the encryption handler can be found.
//        /// </summary>
//        /// <param name="identifier">The identifier.</param>
//        /// <returns></returns>
//        public bool HasEncryptionHandler(string identifier)
//        {
//            return mEncryptionHandlers.ContainsKey(identifier);
//        }
//        #endregion
//        #region RegisterEncryptionHandler(string identifier, IEncryptionHandler handler)
//        /// <summary>
//        /// This method registers an encryption handler with the collection.
//        /// </summary>
//        /// <param name="identifier">The identifier for the handler.</param>
//        /// <param name="handler">The handler to register.</param>
//        public void RegisterEncryptionHandler(string identifier, IServiceHandlerEncryption handler)
//        {
//            if (string.IsNullOrEmpty(identifier))
//                throw new ArgumentNullException("identifier");

//            if (handler == null)
//                throw new ArgumentNullException("handler");

//            if (mEncryptionHandlers.ContainsKey(identifier))
//                throw new EncryptionHandlerAlreadyExistsException(identifier);

//            try
//            {
//                mEncryptionHandlers.Add(identifier, handler);
//            }
//            catch (Exception ex)
//            {
//                Collector?.LogException($"{nameof(RegisterEncryptionHandler)} unexpected error.", ex);
//                throw;
//            }
//        }
//        #endregion

//        #region Start/Stop
//        /// <summary>
//        /// This method starts the service. It sets the authentication handlers config.
//        /// </summary>
//        protected override void StartInternal()
//        {
//            //Set the originators
//            mAuthenticationHandlers.Values.ForEach((a) =>
//            {
//                a.OriginatorId = OriginatorId;
//                a.Collector = Collector;
//            });
//        }
//        /// <summary>
//        /// This method stops the service.
//        /// </summary>
//        protected override void StopInternal()
//        {

//        }
//        #endregion

//        #region Encrypt(EncryptionHandlerId handler, byte[] input)
//        /// <summary>
//        /// Encrypts the specified blob using the handler provided..
//        /// </summary>
//        /// <param name="handler">The handler.</param>
//        /// <param name="input">The input blob.</param>
//        /// <returns>The encrypted output blob.</returns>
//        public byte[] Encrypt(EncryptionHandlerId handler, byte[] input)
//        {
//            EncryptionValidate(handler);

//            return mEncryptionHandlers[handler.Id].Encrypt(input);
//        } 
//        #endregion
//        #region Decrypt(EncryptionHandlerId handler, byte[] input)
//        /// <summary>
//        /// Decrypts the specified blob using the handler provided..
//        /// </summary>
//        /// <param name="handler">The handler.</param>
//        /// <param name="input">The input blob.</param>
//        /// <returns>Returns the unencrypted blob.</returns>
//        public byte[] Decrypt(EncryptionHandlerId handler, byte[] input)
//        {
//            EncryptionValidate(handler);

//            return mEncryptionHandlers[handler.Id].Decrypt(input);
//        } 
//        #endregion
//        #region EncryptionValidate(EncryptionHandlerId handler, bool throwErrors = true)
//        /// <summary>
//        /// Validates that the required handler is supported.
//        /// </summary>
//        /// <param name="handler">The handler identifier.</param>
//        /// <param name="throwErrors">if set to <c>true</c> [throw errors].</param>
//        /// <returns>Returns true if the encryption handler is supported.</returns>
//        /// <exception cref="EncryptionHandlerNotResolvedException">This exception is thrown if the handler is not present and throwErrors is set to true.</exception>
//        public bool EncryptionValidate(EncryptionHandlerId handler, bool throwErrors = true)
//        {
//            if (!mEncryptionHandlers.ContainsKey(handler.Id))
//                if (throwErrors)
//                    throw new EncryptionHandlerNotResolvedException(handler.Id);
//                else
//                    return false;

//            return true;
//        } 
//        #endregion

//        #region Collector
//        /// <summary>
//        /// This is the data collector used for logging.
//        /// </summary>
//        public IDataCollection Collector{get; set;}
//        #endregion
//        #region OriginatorId
//        /// <summary>
//        /// This is the system information.
//        /// </summary>
//        public MicroserviceId OriginatorId{get; set;}
//        #endregion

//        #region Class -> Policy
//        /// <summary>
//        /// This is the policy container for the security container.
//        /// </summary>
//        public class Policy: PolicyBase
//        {

//        }
//        #endregion
//        #region Class -> Statistics
//        /// <summary>
//        /// This class holds a reference to the statistics.
//        /// </summary>
//        public class Statistics: StatusBase
//        {
//            /// <summary>
//            /// This is a list of the supported authentication handlers.
//            /// </summary>
//            public string[] AuthenticationHandlers { get; set; }
//            /// <summary>
//            /// This is a list of the supported encryption handlers.
//            /// </summary>
//            public string[] EncryptionHandlers { get; set; }
//        } 
//        #endregion
//    }
//}
