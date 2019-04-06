using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to generate the SQL Schema for the JSON based tables and stored procedures.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositorySqlJsonGenerator<E>
    {
        private SqlStoredProcedureResolver<E> _resolver;

        public RepositorySqlJsonGenerator(SqlStoredProcedureResolver<E> resolver)
        {
            _resolver = resolver;

        }

        public string TableEntity => ProcessTemplate("Tables.Table.sql");

        public string TableHistory => ProcessTemplate("Tables.TableHistory.sql");

        public string TableProperty => ProcessTemplate("Tables.TableProperty.sql");

        public string TablePropertyKey => ProcessTemplate("Tables.TablePropertyKey.sql");

        public string TableReference => ProcessTemplate("Tables.TableReference.sql");

        public string TableReferenceKey => ProcessTemplate("Tables.TableReferenceKey.sql");

        public string ProcessTemplate(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using (Stream stream = assembly.GetManifestResourceStream($"Xigadee.Persistence.Json.SqlTemplates.{resourceName}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return ReplaceTokens(result);
            }
        }

        public string ReplaceTokens(string script)
        {
            return script;
        }
    }
}
