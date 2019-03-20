using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Xigadee
{
    /// <summary>
    /// This class holds the incoming parameters for the GET request.
    /// </summary>
    [ModelBinder(typeof(EntityRequestModelBinder))]
    public class EntityRequestModel
    {
        /// <summary>
        /// The incoming Id as a string
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The incoming version id
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// The entity reference type.
        /// </summary>
        public string Reftype { get; set; }
        /// <summary>
        /// The entity reference value.
        /// </summary>
        public string Refvalue { get; set; }

        /// <summary>
        /// Returns true if the class has a valid setting for a request.
        /// </summary>
        public bool IsValid => IsByKey || IsByReference;
        /// <summary>
        /// Set to true if this is a entity by key read.
        /// </summary>
        public bool IsByKey { get; set; }
        /// <summary>
        /// Set to true if this is a entity by reference read.
        /// </summary>
        public bool IsByReference { get; set; }

        /// <summary>
        /// A quick string output of incoming
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsByKey ? Id.ToString() : $"{Reftype}|{Refvalue}";
        }

    }
}
