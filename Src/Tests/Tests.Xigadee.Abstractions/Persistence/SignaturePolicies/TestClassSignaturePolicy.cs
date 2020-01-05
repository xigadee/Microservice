﻿using System;
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

        public string Calculate(object entity, int? versionid = null)
        {
            if (Supports(entity.GetType()))
            {
                return HashGenerate(entity as TestMemoryPersistenceCheck.TestClass);
            }

            return null;
        }

        private string HashGenerate(TestMemoryPersistenceCheck.TestClass e) =>
            GuidHelper.Create(Namespace, $"{e.Id.ToString("N")}:{e.VersionId.ToString("N")}:{e.Name}".ToLowerInvariant()).ToString("N").ToUpperInvariant();

        public bool Verify(object entity, string signature)
        {
            if (Supports(entity.GetType()))
            {
                var hash = Calculate(entity);
                return hash == signature;
            }

            return false;
        }

        public bool Supports(Type entityType) => entityType == typeof(TestMemoryPersistenceCheck.TestClass)
            || entityType.IsSubclassOf(typeof(TestMemoryPersistenceCheck.TestClass));

        public void RegisterChildPolicy(ISignaturePolicy childPolicy) => _childPolicy = childPolicy;
    }

}
