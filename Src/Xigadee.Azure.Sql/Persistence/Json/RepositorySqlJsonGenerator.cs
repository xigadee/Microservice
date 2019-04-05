using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
