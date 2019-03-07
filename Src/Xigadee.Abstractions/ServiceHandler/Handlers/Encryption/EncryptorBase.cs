using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public abstract class EncryptorBase : ServiceHandlerBase, IServiceHandlerEncryption
    {
        public byte[] Decrypt(byte[] input)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(byte[] input)
        {
            throw new NotImplementedException();
        }
    }
}
