using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is used when a TestContext setting cannot be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class TestContextSettingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestContextSettingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TestContextSettingException(string message) : base(message)
        {

        }
    }
}
