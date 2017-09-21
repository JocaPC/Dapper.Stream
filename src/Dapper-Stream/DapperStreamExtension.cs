using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Dapper
{

    /// <summary>
    /// Dapper Extension that enables pushing query results into a stream.
    /// </summary>
    public static class DapperStreamExtension
    {
        /// <summary>
        /// Streams results of the query into a stream.
        /// </summary>
        /// <param name="connection">The connection that will be extended with this method.</param>
        /// <param name="stream">The output stream where the results will be written.</param>
        /// <param name="sql">The T-Sql text for this command.</param>
        /// <param name="parameters">The parameters for this command.</param>
        /// <param name="transaction">The transaction for this command to participate in.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="commandTimeout">The timeout (in seconds) for this command.</param>
        /// <param name="commandType">The <see cref="CommandType"/> for this command.</param>
        /// <param name="defaultOutput">The output text with will be written into the stream if command returns no results.</param>
        public static void QueryInto(this IDbConnection connection, Stream stream,
                                        string sql, object param = null, IDbTransaction transaction = null,
                                        bool buffered = false, int? commandTimeout = null, CommandType? commandType = null,
                                        string defaultOutput = "[]")
        {
            bool empty = true;
            foreach (var s in connection.Query<string>(sql, param, transaction, buffered, commandTimeout, commandType))
            {
                empty = false;
                stream.Write(System.Text.Encoding.UTF8.GetBytes(s), 0, s.Length);
            }
            if (empty)
                stream.Write(System.Text.Encoding.UTF8.GetBytes(defaultOutput), 0, defaultOutput.Length);
        }

        /// <summary>
        /// Asynchronously streams results of the query into a stream.
        /// </summary>
        /// <param name="connection">The connection that will be extended with this method.</param>
        /// <param name="stream">The output stream where the results will be written.</param>
        /// <param name="sql">The T-Sql text for this command.</param>
        /// <param name="parameters">The parameters for this command.</param>
        /// <param name="transaction">The transaction for this command to participate in.</param>
        /// <param name="buffered">Whether to buffer the results in memory.</param>
        /// <param name="commandTimeout">The timeout (in seconds) for this command.</param>
        /// <param name="commandType">The <see cref="CommandType"/> for this command.</param>
        /// <param name="defaultOutput">The output text with will be written into the stream if command returns no results.</param>
        public async static Task QueryAsyncInto(this IDbConnection connection, Stream stream,
                                        string query, object param = null, IDbTransaction transaction = null,
                                        bool buffered = false, int? commandTimeout = null, CommandType? commandType = null, string defaultOutput = "[]")
        {
            bool empty = true;
            foreach (var s in connection.QueryAsync<string>(query, param, transaction, commandTimeout, commandType).Result)
            {
                empty = false;
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(s), 0, s.Length);
            }
            if (empty)
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(defaultOutput), 0, defaultOutput.Length);
        }
    }
}
