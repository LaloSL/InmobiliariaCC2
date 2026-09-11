using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioUsuario
    {
       private readonly string _connectionString;

        public RepositorioUsuario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }
       
        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT id_usuario, nombre, email, password_hash, rol, estado
                    FROM Usuario
                    WHERE email = @email
                    LIMIT 1;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt32("id_usuario"),
                                Nombre = reader.GetString("nombre"),
                                Email = reader.GetString("email"),
                                PasswordHash = reader.GetString("password_hash"),
                                Rol = reader.GetString("rol"),
                                Estado = reader.GetBoolean("estado")
                            };
                        }
                    }
                }
            }

            return usuario;
        }
    }
}