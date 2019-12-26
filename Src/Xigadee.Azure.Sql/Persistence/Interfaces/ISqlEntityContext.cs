using System.Data.SqlClient;

namespace Xigadee
{
    /// <summary>
    /// This interface holds the SQL specific properties for the context.
    /// </summary>
    public interface ISqlEntityContext
    {
        /// <summary>
        /// This is the current Sql command
        /// </summary>
        SqlCommand Command { get; set; }
        /// <summary>
        /// This is the stored procedure name.
        /// </summary>
        string SpName { get; }
    }
}