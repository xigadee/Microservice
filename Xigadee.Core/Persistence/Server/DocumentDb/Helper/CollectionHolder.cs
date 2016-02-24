#region using
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the documentDb collection holder.
    /// </summary>
    public class CollectionHolder
    {
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="accountName">This is the documentdb id</param>
        /// <param name="base64key">This is the base64 encoded access key</param>
        /// <param name="databaseName">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="collectionName">The is the collection name. If the collection does it exist it will be created.</param>

        public CollectionHolder(string accountName, string base64key, string databaseName, string collectionName, TimeSpan? defaultTimeout = null, bool create = true) :
            this(DocumentDbConnection.ToConnection(accountName, base64key), databaseName, collectionName, defaultTimeout, create)
        {
        }

        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="account">This is the documentdb id</param>
        /// <param name="base64key">This is the base64 encoded access key</param>
        /// <param name="databaseName">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="collectionName">The is the collection name. If the collection does it exist it will be created.</param>
        public CollectionHolder(DocumentDbConnection connection, string databaseName, string collectionName, TimeSpan? defaultTimeout = null, bool create = true)
        {
            Account = connection.ToAccount(defaultTimeout);

            Database = Account.ToDatabase(databaseName, create: create);

            Collection = Database.ToCollection(collectionName, create: create);
        } 
        #endregion

        /// <summary>
        /// The DocumentDb account.
        /// </summary>
        public Account Account { get; protected set; }
        /// <summary>
        /// The DocumentDb database.
        /// </summary>
        public Database Database { get; protected set; }
        /// <summary>
        /// The DocumentDb collection.
        /// </summary>
        public Collection Collection { get; protected set; }
    }
}
