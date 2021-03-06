﻿namespace Xigadee
{
    /// <summary>
    /// This is the direction of the command request.
    /// </summary>
    public enum CommandHarnessTrafficDirection
    {
        /// <summary>
        /// The request was generated externally in to the command.
        /// </summary>
        Request,
        /// <summary>
        /// The request was generated by the command in response to a request
        /// </summary>
        Response,
        /// <summary>
        /// The request was generated in the command and is outgoing.
        /// </summary>
        Outgoing
    }
}
