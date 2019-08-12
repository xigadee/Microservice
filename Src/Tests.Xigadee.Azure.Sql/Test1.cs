using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    public class Test1 : EntityAuditableBase
    {
        /// <summary>
        /// This is the entity key for a read by reference request.
        /// </summary>
        public const string EntityKeyReferenceId = "referenceid";
        /// <summary>
        /// This method formats the reference id.
        /// </summary>
        /// <param name="UserId">The user id.</param>
        /// <param name="AccountId">The account id.</param>
        /// <returns>Returns the uniquely formatted string.</returns>
        public static string ReferenceFormat(Guid UserId, Guid AccountId)
            => $"U{UserId:N}.{AccountId:N}".ToUpperInvariant();

        /// <summary>
        /// This is the account id.
        /// </summary>
        [EntityPropertyHint("accountid")]
        public Guid AccountId { get; set; }
        /// <summary>
        /// This is the user id.
        /// </summary>
        [EntityPropertyHint("userid")]
        public Guid UserId { get; set; }
        /// <summary>
        /// This is the unique reference Id for this account.
        /// </summary>
        [JsonIgnore]
        [EntityReferenceHint(EntityKeyReferenceId)]
        public string ReferenceId => ReferenceFormat(UserId, AccountId);
    }
}
