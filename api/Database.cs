using Microsoft.Data.SqlClient;

namespace api
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Example method to execute a query and return results as a list of dictionaries
        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, object>>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            return results;
        }

        // Example method to execute a non-query command (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return await cmd.ExecuteNonQueryAsync();
        }

    }
}
