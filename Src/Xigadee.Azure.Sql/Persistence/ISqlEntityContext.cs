using System.Data.SqlClient;

namespace Xigadee
{
    public interface ISqlEntityContext
    {
        SqlCommand Command { get; set; }
        string SpName { get; }
    }
}