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
    public class Sha512SignatureHelper: SignatureHelper<SHA512CryptoServiceProvider>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha512SignatureHelper(string salt) : base(salt)
        {
        }
        #endregion

        protected override SHA512CryptoServiceProvider CreateProvider()
        {
            return new SHA512CryptoServiceProvider();
        }
    }
}
