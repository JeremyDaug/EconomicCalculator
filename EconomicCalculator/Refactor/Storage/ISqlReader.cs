using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Refactor.Storage
{
    /// <summary>
    /// Sql reading interface.
    /// </summary>
    public interface ISqlReader
    {
        /// <summary>
        /// Loads data into the class from an sql reader.
        /// </summary>
        /// <param name="reader">The Connection to the Database.</param>
        void LoadFromSql(SqlConnection connection);
    }
}
