using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to consolidate a set of SQL generation scripts and their customized options together.
    /// </summary>
    public abstract class SqlGeneratorCollection
    {
        /// <summary>
        /// This method will populate the sql generator automatically, if not already done. It is called by the enumerator before it returns the collection.
        /// </summary>
        public virtual void Populate()
        {
            GetType()
                .GetCustomAttributes<PrepopulateEntitiesAttribute>()
                .Where(a => a.EntityTypes != null)
                .SelectMany(a => a.EntityTypes)
                .ForEach(t => PopulateSqlJsonGenerator(t));
        }

        public Dictionary<Type, RepositorySqlJsonOptions> Options { get; } = new Dictionary<Type, RepositorySqlJsonOptions>();

        public Dictionary<Type, SqlStoredProcedureResolver> Resolvers { get; } = new Dictionary<Type, SqlStoredProcedureResolver>();

        public Dictionary<Type, SqlFileExtractOptions> FileExtractOptions { get; } = new Dictionary<Type, SqlFileExtractOptions>();

        public Dictionary<Type, SqlJsonGenerator> SqlGenerators { get; } = new Dictionary<Type, SqlJsonGenerator>();

        public RepositorySqlJsonOptions DefaultRepositorySqlJsonOptions { get; set; } = new RepositorySqlJsonOptions(true);

        public SqlFileExtractOptions DefaultSqlFileExtractOptions { get; set; }

        protected RepositorySqlJsonOptions GetOptions<E>() => GetOptions(typeof(E));
        protected RepositorySqlJsonOptions GetOptions(Type type) => Options.ContainsKey(type) ? Options[type] : DefaultRepositorySqlJsonOptions;

        protected SqlStoredProcedureResolver GetResolver<E>() => GetResolver(typeof(E));
        protected SqlStoredProcedureResolver GetResolver(Type type) => Resolvers.ContainsKey(type) ? Resolvers[type] : new SqlStoredProcedureResolver(type.Name);

        protected SqlFileExtractOptions GetFileExtractOptions<E>() => GetFileExtractOptions(typeof(E));
        protected SqlFileExtractOptions GetFileExtractOptions(Type type) => FileExtractOptions.ContainsKey(type) ? FileExtractOptions[type] : DefaultSqlFileExtractOptions;

        /// <summary>
        /// This method can be used to populate specific Sql Generators
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="names"></param>
        /// <param name="options"></param>
        /// <param name="fileExtractOptions"></param>
        /// <returns>Returns the SqlJsonGenerator</returns>
        public virtual SqlJsonGenerator PopulateSqlJsonGenerator<E>(SqlStoredProcedureResolver<E> names = null, RepositorySqlJsonOptions options = null, SqlFileExtractOptions fileExtractOptions = null)
            => PopulateSqlJsonGenerator(typeof(E), names, options, fileExtractOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="names"></param>
        /// <param name="options"></param>
        /// <param name="fileExtractOptions"></param>
        /// <returns></returns>
        public virtual SqlJsonGenerator PopulateSqlJsonGenerator(Type type, SqlStoredProcedureResolver names = null, RepositorySqlJsonOptions options = null, SqlFileExtractOptions fileExtractOptions = null)
        {
            if (!SqlGenerators.ContainsKey(type))
                SqlGenerators[type] = new SqlJsonGenerator(names ?? GetResolver(type), options ?? GetOptions(type), fileExtractOptions ?? GetFileExtractOptions(type));

            return SqlGenerators[type];
        }

        /// <summary>
        /// This method creates the Sql generator.
        /// </summary>
        /// <typeparam name="E">The generator type.</typeparam>
        /// <returns>Returns the SqlJsonGenerator</returns>
        public virtual SqlJsonGenerator Get<E>(SqlStoredProcedureResolver names = null, RepositorySqlJsonOptions options = null, SqlFileExtractOptions fileExtractOptions = null) => 
            PopulateSqlJsonGenerator(typeof(E), names, options, fileExtractOptions);

        /// <summary>
        /// This is the collection of Sql generators.
        /// </summary>
        /// <returns>Returns the SqlJsonGenerator collection</returns>
        public virtual IEnumerable<SqlJsonGenerator> Generators()
        {
            Populate();

            return SqlGenerators.Values;
        }

    }
}
