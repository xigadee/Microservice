using System.Text;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to provide a consistent interface for initializing an entity from a byte stream or array.
    /// </summary>
    public interface IMessageLoadData
    {
        int Load(string data);

        int Load(string data, Encoding encoding);

        int Load(byte[] buffer, int offset, int count);

        int Load(Stream data);
   }

    public interface IMessageLoadData<TERM>
        where TERM : IMessageTermination
    {
        int Load(TERM terminator, byte[] buffer, int offset, int count);

        int Load(TERM terminator, Stream data);

        int Load(TERM terminator, string data);

        int Load(TERM terminator, string data, Encoding encoding);
    }
    
}
