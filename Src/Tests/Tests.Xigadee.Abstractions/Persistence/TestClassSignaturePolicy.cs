using System;
using System.Collections.Generic;
using System.Text;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestClassSignaturePolicy : ISignaturePolicy
    {
        ISignaturePolicy _childPolicy = null;

        readonly Guid Namespace = new Guid("{7F09C1CF-4CDB-45EB-BA3B-E9F3610635C0}");

        public int? SignatureVersion => 1;

        public bool VerificationPassedWithoutSignature { get; set; } = false;

        public string Calculate(object entity, int? versionid = null)
        {
            if (!Supports(entity.GetType()))
                return null;

            var e = entity as TestClass;

            return $"{e.Id.ToString("N")}:{e.VersionId.ToString("N")}:{e.Name}".ToLowerInvariant();
        }

        private string HashGenerate(TestClass e) =>
            GuidHelper.Create(Namespace, $"{e.Id.ToString("N")}:{e.VersionId.ToString("N")}:{e.Name}".ToLowerInvariant()).ToString("N").ToUpperInvariant();

        public bool Verify(object entity, string signature)
        {
            if (VerificationPassedWithoutSignature && string.IsNullOrWhiteSpace(signature))
                return true;

            if (Supports(entity.GetType()))
            {
                var hash = Calculate(entity);
                return hash == signature;
            }

            return false;
        }

        public bool Supports(Type entityType) => entityType == typeof(TestClass)
            || entityType.IsSubclassOf(typeof(TestClass));

        public void RegisterChildPolicy(ISignaturePolicy childPolicy) => _childPolicy = childPolicy;
    }

}
