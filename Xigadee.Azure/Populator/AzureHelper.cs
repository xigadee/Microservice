using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Xigadee
{
    public static class AzureHelper
    {
        public readonly static Func<string, string, string> Resolver = (k, v) =>
                {
                    string value = null;

                    try
                    {
                        if (k != null)
                        {
                            value = CloudConfigurationManager.GetSetting(k);

                            if (value == null)
                                value = ConfigurationManager.AppSettings[k];
                        }
}
                    catch (Exception)
                    {
                        // Unable to retrieve from azure
                        return null;
                    }

                    return value;
                };
    }
}
