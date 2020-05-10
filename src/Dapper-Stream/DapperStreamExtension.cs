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
            QueryInto(connection, stream,
                                  sql, new DapperStreamOptions() { DefaultOutput = defaultOutput },
                                  param, transaction, buffered, commandTimeout, commandType);
        }

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
        /// <param name="options">The output text with will be written into the stream if command returns no results.</param>
        public static void QueryInto(this IDbConnection connection, Stream stream, 
                                        string sql, DapperStreamOptions options, object param = null, IDbTransaction transaction = null,
                                        bool buffered = false, int? commandTimeout = null, CommandType? commandType = null)
        {
            bool empty = true;
            if(options.Prefix != null)
                stream.Write(System.Text.Encoding.UTF8.GetBytes(options.Prefix), 0, options.Prefix.Length);

            foreach (var s in connection.Query<string>(sql, param, transaction, buffered, commandTimeout, commandType))
            {
                empty = false;
                byte[] c = System.Text.Encoding.UTF8.GetBytes(s);
                stream.Write(c, 0, c.Length);
            }
            if (empty && options.DefaultOutput != null)
                stream.Write(System.Text.Encoding.UTF8.GetBytes(options.DefaultOutput), 0, options.DefaultOutput.Length);
            
            if (options.Suffix != null)
                stream.Write(System.Text.Encoding.UTF8.GetBytes(options.Suffix), 0, options.Suffix.Length);
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
                                        string query, DapperStreamOptions options, object param = null, IDbTransaction transaction = null,
                                        bool buffered = false, int? commandTimeout = null, CommandType? commandType = null)
        {
            bool empty = true;
            var result = connection.QueryAsync<string>(query, param, transaction, commandTimeout, commandType).Result;
            await StreamAsync(stream, options, empty, result);
        }

        public async static Task QueryAsyncInto(this IDbConnection connection, Stream stream,
                                        string query, object param = null, IDbTransaction transaction = null,
                                        bool buffered = false, int? commandTimeout = null, CommandType? commandType = null, string defaultOutput = "[]")
        {
            bool empty = true;
            var result = connection.QueryAsync<string>(query, param, transaction, commandTimeout, commandType).Result;
            await StreamAsync(stream, new DapperStreamOptions() { DefaultOutput = defaultOutput }, empty, result);
        }

        private static async Task StreamAsync(Stream stream, DapperStreamOptions options, bool empty, System.Collections.Generic.IEnumerable<string> result)
        {
            if (options.Prefix != null)
            {
                var Prefix = System.Text.Encoding.UTF8.GetBytes(options.Prefix);
                await stream.WriteAsync(Prefix, 0, Prefix.Length);
            }
            foreach (var s in result)
            {
                empty = false;
                byte[] c = System.Text.Encoding.UTF8.GetBytes(s);
                await stream.WriteAsync(c, 0, c.Length);
            }
            if (empty && options.DefaultOutput != null)
            {
                var def = System.Text.Encoding.UTF8.GetBytes(options.DefaultOutput);
                await stream.WriteAsync(def, 0, def.Length);
            }
            if (options.Suffix != null)
            {
                var sufix = System.Text.Encoding.UTF8.GetBytes(options.Suffix);
                await stream.WriteAsync(sufix, 0, sufix.Length);
            }
        }
    }

    public class DapperStreamOptions
    {
        public string Prefix { get; set; }
        public string DefaultOutput { get; set; }
        public string Suffix { get; set; }
    }
}
