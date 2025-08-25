namespace DataLibrary;

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

public class DataAccess : IDataAccess
{
    //für sql benchmarks
    /*
    SET PROFILING = 1;
    SELECT * FROM worker WHERE id= 1;
    SHOW PROFILE FOR QUERY 1;
    */

    public async Task<List<T>> LoadDataAsync<T, U>(
        string sql,
        U parameters,
        string connectionString
    )
    {
        // ist kein stream, benutzt aber using um die Verbindung zu schließen und um Dispose() aufzurufen
        // es verwendet Netzwerkressourcen, daher ist es wichtig, die Verbindung zu schließen
        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var rows = await connection.QueryAsync<T>(sql, parameters);
            return rows.ToList();
        }
    }

    // ! muss hier gewartet werden, sonst kann es sein zu verbindungsproblemen kommen
    public async Task SaveDataAsync<T>(string sql, T parameters, string connectionString)
    {
        // ist kein stream, benutzt aber using um die Verbindung zu schließen und um Dispose() aufzurufen
        // es verwendet Netzwerkressourcen, daher ist es wichtig, die Verbindung zu schließen

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, parameters);
        }
    }

    public async Task<int> SaveDataAndGetIdAsync<T>(
        string sql,
        T parameters,
        string connectionString
    )
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var newId = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return newId;
        }
    }
}

public interface IDataAccess
{
    Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters, string connectionString);
    Task SaveDataAsync<T>(string sql, T parameters, string connectionString);
    Task<int> SaveDataAndGetIdAsync<T>(string sql, T parameters, string connectionString);
}
