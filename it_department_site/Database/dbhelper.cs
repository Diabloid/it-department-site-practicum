using System;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using it_department_site;

namespace dbhelper
{

    public class Dbhelper
    {
        private readonly string _connectionString;

        public Dbhelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Виконання SELECT-запиту і повернення результату у DataTable
        public DataTable ExecuteQuery(string query)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Виконання INSERT, UPDATE, DELETE запитів
        public int ExecuteNonQuery(string query)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }
        // Параметризований SELECT
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var adapter = new MySqlDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // Параметризований INSERT/UPDATE/DELETE
        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return command.ExecuteNonQuery();
        }

    }
}