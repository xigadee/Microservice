using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class StatisticsSummaryLog
    {
        public const string MimeContentType = "udp/statisticssummarylog";

    }

    public class StatisticsSummaryLogUdpSerializer: JsonRawSerializer
    {
        public override string ContentType => StatisticsSummaryLog.MimeContentType;


        public override void Deserialize(SerializationHolder holder)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(SerializationHolder holder)
        {
            throw new NotImplementedException();
        }

        public override bool SupportsContentTypeSerialization(Type entityType)
        {
            return entityType == typeof(StatisticsSummaryLog);
        }
    }
}
