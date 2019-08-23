using System;
using System.Collections.Generic;
using System.Text;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    /// <summary>
    /// This is the core account entity.
    /// </summary>
    public class Account : UserReferenceBase
    {
        /// <summary>
        /// This is the account type.
        /// </summary>
        [EntityPropertyHint("type")]
        public AccountType Type { get; set; }
        /// <summary>
        /// The optional userId. If the type is Personal, this is mandatory.
        /// </summary>
        [EntityPropertyHint("userid")]
        public override Guid? UserId { get => base.UserId; set => base.UserId = value; }
        /// <summary>
        /// This is the optional organization Id. If the type is Group, this is mandatory.
        /// </summary>
        [EntityPropertyHint("groupid")]
        public Guid? GroupId { get; set; }
    }


    [Flags]
    public enum AccountType : int
    {
        Type0 = 0,
        Type1 = 1,
        Type2 = 2
    }
}
