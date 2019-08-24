﻿namespace Xigadee
{
    /// <summary>
    /// This class is used to generate the SQL for a specific entity.
    /// </summary>
    /// <typeparam name="E">The entity type parameter.</typeparam>
    public class SqlJsonGenerator<E>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="names">This is the optional naming class. If this is null a class will be created with the default settings.</param>
        /// <param name="options">This is the SQL generation options settings.</param>
        public SqlJsonGenerator(SqlStoredProcedureResolver<E> names = null, RepositorySqlJsonOptions options = null)
        {
            Names = names ?? new SqlStoredProcedureResolver<E>();
            Generator = new RepositorySqlJsonGenerator<E>(Names,options);
        }

        /// <summary>
        /// This is the SQL naming class.
        /// </summary>
        public SqlStoredProcedureResolver<E> Names { get; }

        /// <summary>
        /// This is the Sql generator class
        /// </summary>
        public RepositorySqlJsonGenerator<E> Generator { get; }

        /// <summary>
        /// This is a shortcut to the main SQL generation.
        /// </summary>
        public string SqlEntity => Generator.ScriptEntity;

        /// <summary>
        /// This is the SQL script definition without the base extension SQL code.
        /// </summary>
        public string ScriptEntityWithoutExtension => Generator.ScriptEntityWithoutExtension;

        /// <summary>
        /// This is the supported SQL extension script that you can customize for your data uses.
        /// </summary>
        public string ScriptExtensionLogic => Generator.ScriptExtensionLogic;

        /// <summary>
        /// This is the shortcut to the Extension Table SQL.
        /// </summary>
        public string ScriptExtensionTable => Generator.ScriptExtensionTable;
    }
}
