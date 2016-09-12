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

namespace Xigadee
{
    /// <summary>
    /// This helper class provides shortcuts for creation using standard names.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// This method converts a DocumentDb connection in to an account.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="defaultTimeout">The default timeout.</param>
        /// <returns></returns>
        public static Account ToAccount(this DocumentDbConnection conn, TimeSpan? defaultTimeout = null)
        {
            return new Account(conn, defaultTimeout);
        }

        public static Database ToDatabase(this Account account, string databaseName, TimeSpan? defaultTimeout = null, bool create = false)
        {
            var database = account.Resolve(databaseName).Result;
            if (database != null)
                return database;

            if (!create)
                throw new ArgumentException($"DocumentDB database {databaseName} cannot be resolved on server {account.Name}");

            var dbResult = account.Create(databaseName).Result;
            if (!dbResult.IsSuccess)
                throw new ArgumentException($"DocumentDB database {databaseName} cannot be created on server {account.Name}-{dbResult.StatusCode}-{dbResult.StatusMessage}");

            return dbResult.Entity;
        }

        public static Collection ToCollection(this Database database, string collectionName, TimeSpan? defaultTimeout = null, bool create = false)
        {
            var collection = database.Resolve(collectionName, database.Name).Result;
            if (collection != null)
                return collection;

            if (!create)
                throw new ArgumentException($"DocumentDB collection {collectionName} cannot be resolved on database {database.Name} for server {database.Connection.AccountName}");

            var collectionResult = database.Create(collectionName, database.Name).Result;
            if (!collectionResult.IsSuccess)
                throw new ArgumentException($"DocumentDB collection {collectionName} cannot be created on database {database.Name} for server {database.Connection.AccountName}-{collectionResult.StatusCode}-{collectionResult.StatusMessage}");

            return collectionResult.Entity;
        }

        public static CollectionHolder ToCollectionHolder(this DocumentDbConnection conn, string databaseName, string collectionName, TimeSpan? defaultTimeout = null, bool create = true)
        {
            return new CollectionHolder(conn, databaseName, collectionName, defaultTimeout, create);
        }
    }
}
