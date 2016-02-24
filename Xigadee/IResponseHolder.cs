using System;
using System.Collections.Generic;

namespace Xigadee
{
    public interface IResponseHolder
    {
        string Content { get; set; }
        Exception Ex { get; set; }
        Dictionary<string, string> Fields { get; set; }
        string Id { get; set; }
        string VersionId { get; set; }
        bool IsSuccess { get; set; }
        bool IsTimeout { get; set; }
        int StatusCode { get; }
    }
}