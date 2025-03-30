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
    }
}