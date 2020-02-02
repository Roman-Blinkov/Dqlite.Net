﻿using System.Threading;
using System.Threading.Tasks;

namespace Dqlite.Net
{
    public static class DqliteConnectionExtensions
    {
        public static Task<DqliteDataReader> ExecuteReaderAsync(this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
            => ExecuteReaderAsync(connection, commandText, parameters, CancellationToken.None);
        
        public static async Task<DqliteDataReader> ExecuteReaderAsync(
            this DqliteConnection connection,
            string commandText,
            DqliteParameter[] parameters,
            CancellationToken cancellationToken = default(CancellationToken))   
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return await command.ExecuteReaderAsync(cancellationToken);
            }
        }

        public static Task<int> ExecuteNonQueryAsync(
            this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
            => ExecuteNonQueryAsync(connection, commandText, parameters, CancellationToken.None);

        public static async Task<int> ExecuteNonQueryAsync(
            this DqliteConnection connection,
            string commandText,
            DqliteParameter[] parameters,
            CancellationToken cancellationToken = default(CancellationToken))   
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public static Task<T> ExecuteScalarAsync<T>(
            this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
            => ExecuteScalarAsync<T>(connection, commandText, parameters, CancellationToken.None);

        public static async Task<T> ExecuteScalarAsync<T>(
            this DqliteConnection connection,
            string commandText,
            DqliteParameter[] parameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return (T) await command.ExecuteScalarAsync(cancellationToken);
            }
        }

        public static DqliteDataReader ExecuteReader(
            this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return command.ExecuteReader();
            }
        }

        public static int ExecuteNonQuery(
            this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return command.ExecuteNonQuery();
            }
        }

        public static T ExecuteScalar<T>(
            this DqliteConnection connection,
            string commandText,
            params DqliteParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddRange(parameters);

                return (T)command.ExecuteScalar();
            }
        }
    }
}
