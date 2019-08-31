using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This structure processes the content type field and extracts the media type
    /// and the parameters.
    /// </summary>
    public class ContentType
    {
        public string MediaType = null;
        public Dictionary<string, string> Parameters = null;

        public ContentType(string data)
        {
            string[] items = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            MediaType = items[0].TrimStart().ToLowerInvariant();

            if (items.Length <= 1)
                return;

            Parameters = new Dictionary<string, string>(items.Length - 1);
            int count = 1;
            while (count < items.Length)
            {
                string item = items[count];
                int pos = item.IndexOf('=');
                if (pos == -1)
                    Parameters.Add(item.ToLowerInvariant().TrimStart(), null);
                else
                    Parameters.Add(
                        item.Substring(0, pos).ToLowerInvariant().TrimStart()
                        , item.Substring(pos + 1));

                count++;
            }
        }

        public string Parameter(string id)
        {
            if (Parameters == null || !Parameters.ContainsKey(id))
                return null;

            return Parameters[id];
        }
    }
}
