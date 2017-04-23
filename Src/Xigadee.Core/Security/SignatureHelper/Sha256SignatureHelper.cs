using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha256SignatureHelper: SignatureHelper<SHA256CryptoServiceProvider>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha256SignatureHelper(string salt) : base(salt)
        {
        }
        #endregion

        protected override SHA256CryptoServiceProvider CreateProvider()
        {
            return new SHA256CryptoServiceProvider();
        }
    }
}
