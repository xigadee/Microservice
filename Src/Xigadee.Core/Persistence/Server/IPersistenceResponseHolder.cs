using System;

namespace IMGroup.Microservice
{
    public interface IPersistenceResponseHolder
    {
        Exception Ex { get; set; }

        bool IsSuccess { get; set; }

        bool IsTimeout { get; set; }

        bool IsThrottled { get; set; }

        int StatusCode { get; set; }

        byte[] Content { get; set; }

        string VersionId { get; set; }
    }
}