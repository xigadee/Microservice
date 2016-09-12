#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to log incoming and outgoing payload message metadata to a SQL database
    /// </summary>
    public class BoundaryLoggerSql: IBoundaryLogger
    {
        #region Declarations
        private readonly string mSqlConnection;
        private readonly string mServiceName;
        private readonly string mServiceId;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor. You should set the SQL connection string here.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection string for the database.</param>
        /// <param name="commandName">This is the default stored procedure name [External].[BoundaryLog] if not set.</param>
        public BoundaryLoggerSql(
              string sqlConnection
            , string serviceName
            , string serviceId
            )
        {
            if (sqlConnection == null)
                throw new ArgumentNullException("SqlBoundaryLogger: sqlConnection cannot be null.");

            mSqlConnection = sqlConnection;
            mServiceName = serviceName;
            mServiceId = serviceId;
        }
        #endregion

        /// <summary>
        /// This method converts the payload and metadata in to a SQL call.
        /// </summary>
        /// <param name="direction">The message direction.</param>
        /// <param name="payload">The payload to serialize.</param>
        /// <param name="ex">Any exception.</param>
        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            if (payload == null)
                return;

            try
            {
                using (SqlConnection cn = new SqlConnection(mSqlConnection))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("[External].[BoundaryLog]");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = cn;

                    cmd.Parameters.Add("@ServiceName", SqlDbType.VarChar, 50).Value = mServiceName;
                    cmd.Parameters.Add("@ServiceId", SqlDbType.VarChar, 50).Value = mServiceId;
                    cmd.Parameters.Add("@PayloadId", SqlDbType.UniqueIdentifier).Value = payload.Id;
                    cmd.Parameters.Add("@Direction", SqlDbType.Bit).Value = direction == ChannelDirection.Outgoing;

                    var message = payload.Message;
                    if (message != null)
                    {
                        cmd.Parameters.Add("@ChannelId", SqlDbType.VarChar, 50).Value = message.ChannelId;
                        cmd.Parameters.Add("@ChannelPriority", SqlDbType.Int).Value = message.ChannelPriority;
                        cmd.Parameters.Add("@MessageType", SqlDbType.VarChar, 50).Value = message.MessageType;
                        cmd.Parameters.Add("@ActionType", SqlDbType.VarChar, 50).Value = message.ActionType;

                        cmd.Parameters.Add("@OriginatorKey", SqlDbType.VarChar, 250).Value = message.OriginatorKey;
                        cmd.Parameters.Add("@OriginatorServiceId", SqlDbType.VarChar, 250).Value = message.OriginatorServiceId;
                        cmd.Parameters.Add("@OriginatorUTC", SqlDbType.DateTime2).Value = message.OriginatorUTC;

                        cmd.Parameters.Add("@CorrelationKey", SqlDbType.VarChar, 250).Value = message.CorrelationKey;
                        cmd.Parameters.Add("@CorrelationServiceId", SqlDbType.VarChar, 250).Value = message.CorrelationServiceId;
                        if (message.CorrelationUTC.HasValue)
                            cmd.Parameters.Add("@CorrelationUTC", SqlDbType.DateTime2).Value = message.CorrelationUTC.Value;

                        cmd.Parameters.Add("@ResponseChannelId", SqlDbType.VarChar, 250).Value = message.ResponseChannelId;
                        cmd.Parameters.Add("@ResponseChannelPriority", SqlDbType.Int).Value = message.ResponseChannelPriority;

                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 250).Value = message.Status;
                        cmd.Parameters.Add("@StatusDescription", SqlDbType.VarChar, 250).Value = message.StatusDescription;

                        if (batchId.HasValue)
                            cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId.Value;
                    }

                    int sqlResult = cmd.ExecuteNonQueryAsync().Result;
                }
            }
            catch (Exception)
            {
                //We don't really care here. This is only indicative. Move on, nothing to see here.
            }
        }



        public Guid BatchPoll(int requested, int returned, string channelId)
        {
            Guid batchId = Guid.NewGuid();

            try
            {
                using (SqlConnection cn = new SqlConnection(mSqlConnection))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("[External].[BoundaryPoll]");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = cn;

                    cmd.Parameters.Add("@ServiceName", SqlDbType.VarChar, 50).Value = mServiceName;
                    cmd.Parameters.Add("@ServiceId", SqlDbType.VarChar, 50).Value = mServiceId;
                    cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;

                    cmd.Parameters.Add("@Requested", SqlDbType.Int).Value = requested;
                    cmd.Parameters.Add("@Returned", SqlDbType.Int).Value = returned;

                    cmd.Parameters.Add("@ChannelId", SqlDbType.VarChar, 100).Value = channelId;

                    int sqlResult = cmd.ExecuteNonQueryAsync().Result;
                }
            }
            catch (Exception)
            {
                //We don't really care here. This is only indicative. Move on, nothing to see here.
            }

            return batchId;
        }


    }
}
