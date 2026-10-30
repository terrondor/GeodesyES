using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeodesyApp
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Scheme> GetAllSchemes()
        {
            List<Scheme> schemes = new List<Scheme>();
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Register";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    Scheme scheme = new Scheme();
                    scheme.Id = (int) reader["id"];
                    scheme.Name = reader["Название"].ToString()!;
                    scheme.View = reader["Вид"].ToString()!;
                    scheme.Volume = (decimal) reader["Обьем"];
                    scheme.Status = reader["Статус"].ToString()!;

                    schemes.Add(scheme);
                }
            }
            return schemes;
        }

        public bool AddScheme(Scheme scheme)
        {
            string query = @"INSERT INTO Register (Название, Вид, Обьем, Статус) 
                 VALUES (@name, @view, @volume, @status)";
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@name", scheme.Name);
                    cmd.Parameters.AddWithValue("@view", scheme.View);
                    cmd.Parameters.AddWithValue("@volume", scheme.Volume);
                    cmd.Parameters.AddWithValue("@status", scheme.Status);

                    int result = cmd.ExecuteNonQuery();

                    return result > 0;
                }


            }
            catch(Exception e)
            {
                Console.WriteLine($"Ошибка при обновлении: {e.Message}");
                return false;
            }

        }

        public bool UpdateStatus(int id, string newStatus)
        {
            string query = "UPDATE Register SET Статус = @status WHERE id = @id";
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);

                    // Добавляем параметры
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", id);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
                return false;
            }
        }
        public bool DeleteScheme(int id)
        {
            string query = "DELETE FROM Register WHERE id = @id";
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                return false;
            }
        }
    }
}
