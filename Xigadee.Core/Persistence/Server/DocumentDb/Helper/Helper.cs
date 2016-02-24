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
                throw new ArgumentException(string.Format("DocumentDB database {0} cannot be resolved on server {1}", databaseName, account.Name));

            var dbResult = account.Create(databaseName).Result;
            if (!dbResult.IsSuccess)
                throw new ArgumentException(string.Format("DocumentDB database {0} cannot be created on server {1}", databaseName, account.Name));

            return dbResult.Entity;
        }

        public static Collection ToCollection(this Database database, string collectionName, TimeSpan? defaultTimeout = null, bool create = false)
        {
            var collection = database.Resolve(collectionName, database.Name).Result;
            if (collection != null)
                return collection;

            if (!create)
                throw new ArgumentException(string.Format("DocumentDB collection {0} cannot be resolved on database {1} for server {2}"
                    , collectionName, database.Name, database.Connection.AccountName));

            var collectionResult = database.Create(collectionName, database.Name).Result;
            if (!collectionResult.IsSuccess)
                throw new ArgumentException(string.Format("DocumentDB collection {0} cannot be created on database {1} for server {2}"
                    , collectionName, database.Name, database.Connection.AccountName));

            return collectionResult.Entity;
        }

        public static CollectionHolder ToCollectionHolder(this DocumentDbConnection conn, string databaseName, string collectionName, TimeSpan? defaultTimeout = null, bool create = true)
        {
            return new CollectionHolder(conn, databaseName, collectionName, defaultTimeout, create);
        }
    }
}
