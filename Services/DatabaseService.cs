using MySql.Data.MySqlClient;
using Cotalog.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Cotalog.Services
{
    public static class DatabaseService
    {
        public static string ConnectionString { get; } = "Server=localhost;Database=ProjectDB;Uid=root;Pwd=qwerty;";

        public static void InitializeDatabase()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            // Создание таблиц
            new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS Users (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    login VARCHAR(50) UNIQUE NOT NULL,
                    password_hash VARCHAR(255) NOT NULL,
                    salt VARCHAR(255) NOT NULL,
                    role VARCHAR(20) NOT NULL DEFAULT 'User',
                    full_name VARCHAR(100) NOT NULL
                )", connection).ExecuteNonQuery();

            new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS Projects (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    title VARCHAR(255) NOT NULL,
                    short_description TEXT,
                    detailed_description TEXT,
                    author_id INT NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    likes INT DEFAULT 0,
                    dislikes INT DEFAULT 0,
                    FOREIGN KEY (author_id) REFERENCES Users(id)
                )", connection).ExecuteNonQuery();

            new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS ProjectFiles (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    project_id INT NOT NULL,
                    file_path TEXT NOT NULL,
                    file_type VARCHAR(20) NOT NULL,
                    FOREIGN KEY (project_id) REFERENCES Projects(id)
                )", connection).ExecuteNonQuery();
        }

        public static User? AuthenticateUser(string login, string password)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            var cmd = new MySqlCommand(
                "SELECT salt, password_hash FROM Users WHERE login = @login",
                connection);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var salt = reader.GetString("salt");
            var storedHash = reader.GetString("password_hash");

            var inputHash = SecurityHelper.HashPassword(password, salt);
            return inputHash == storedHash ? GetUser(login) : null;
        }

        private static User? GetUser(string login)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            var cmd = new MySqlCommand(
                "SELECT id, login, role, full_name FROM Users WHERE login = @login",
                connection);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? new User
            {
                Id = reader.GetInt32("id"),
                Login = reader.GetString("login"),
                Role = reader.GetString("role"),
                FullName = reader.GetString("full_name")
            } : null;
        }

        public static bool SaveProject(Project project)
        {
            try
            {
                using var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                var cmd = new MySqlCommand(
                    "INSERT INTO Projects (title, short_description, detailed_description, author_id) " +
                    "VALUES (@title, @short, @detailed, @authorId); SELECT LAST_INSERT_ID();",
                    connection);

                cmd.Parameters.AddWithValue("@title", project.Title);
                cmd.Parameters.AddWithValue("@short", project.ShortDescription);
                cmd.Parameters.AddWithValue("@detailed", project.DetailedDescription);
                cmd.Parameters.AddWithValue("@authorId", project.AuthorId);

                var projectId = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (var file in project.Files)
                {
                    cmd = new MySqlCommand(
                        "INSERT INTO ProjectFiles (project_id, file_path, file_type) " +
                        "VALUES (@projectId, @path, @type)",
                        connection);

                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@path", file.FilePath);
                    cmd.Parameters.AddWithValue("@type", file.FileType);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                return false;
            }
        }

        public static List<Project> GetProjects()
        {
            var projects = new List<Project>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand(@"
                    SELECT p.id, p.title, p.short_description, p.created_at, 
                           p.likes, p.dislikes, u.full_name as author_name
                    FROM Projects p
                    JOIN Users u ON p.author_id = u.id", connection);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    projects.Add(new Project
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        ShortDescription = reader.GetString("short_description"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        Likes = reader.GetInt32("likes"),
                        Dislikes = reader.GetInt32("dislikes"),
                        AuthorName = reader.GetString("author_name")
                    });
                }
            }
            return projects;
        }
    }
}