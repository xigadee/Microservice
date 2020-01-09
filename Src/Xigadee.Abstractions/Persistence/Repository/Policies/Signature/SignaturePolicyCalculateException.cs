using System;
using System.Runtime.Serialization;

namespace Xigadee
{
    [Serializable]
    public class SignaturePolicyCalculateException : Exception
    {
        private Exception ex;


        public SignaturePolicyCalculateException(string message, Exception ex):base(message,ex)
        {
            this.ex = ex;
        }

    }
}