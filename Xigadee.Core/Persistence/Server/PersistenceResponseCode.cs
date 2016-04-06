namespace Xigadee
{
    /// <summary>
    /// This is the set of standard response codes for Persistence responses.
    /// You should set these codes when coding a response.
    /// </summary>
    public enum PersistenceResponseCode: int
    {
        Success_200 = 200,
        Created_201 = 201,
        NotFound_404 = 404,
        Conflict_412 = 412,
        UnhandledException_500 = 500,
        NotImplemented_501 = 501
    }
}
