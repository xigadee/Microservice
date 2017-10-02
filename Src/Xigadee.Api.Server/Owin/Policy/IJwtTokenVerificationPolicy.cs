namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that verify the token.
    /// </summary>
    public interface IJwtTokenVerificationPolicy
    {
        /// <summary>
        /// This method validates the tokenParameter and returns the token.
        /// </summary>
        /// <param name="tokenParameter">The token string parameter.</param>
        /// <returns></returns>
        JwtToken Validate(string tokenParameter);
        /// <summary>
        /// This property specifies whether the filter should deny access by default.
        /// </summary>
        bool DenyByDefault { get; set; } 
    }
}