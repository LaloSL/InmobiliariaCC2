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


        // OBTENER USUARIO POR EMAIL

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
            SELECT
                id_usuario,
                nombre,
                email,
                password_hash,
                rol,
                estado,
                avatar
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
                                Estado = reader.GetBoolean("estado"),

                                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar"))
                                    ? null
                                    : reader.GetString("avatar")
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        // OBTENER USUARIO POR ID

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT
                        id_usuario,
                        nombre,
                        email,
                        password_hash,
                        rol,
                        estado,
                        avatar
                    FROM Usuario
                    WHERE id_usuario = @id
                    LIMIT 1;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }

            return usuario;
        }

        // ACTUALIZAR AVATAR

        public bool ActualizarAvatar(int idUsuario, string rutaAvatar)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE Usuario
                    SET avatar = @avatar
                    WHERE id_usuario = @idUsuario;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@avatar", rutaAvatar);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // MAPEO DEL USUARIO

        private Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("id_usuario"),
                Nombre = reader.GetString("nombre"),
                Email = reader.GetString("email"),
                PasswordHash = reader.GetString("password_hash"),
                Rol = reader.GetString("rol"),
                Estado = reader.GetBoolean("estado"),

                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar"))
                    ? null
                    : reader.GetString("avatar")
            };
        }
        // =========================================================
        // ACTUALIZAR PERFIL
        // =========================================================
        public bool ActualizarPerfil(
            int idUsuario,
            string nombre,
            string email)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
            UPDATE Usuario
            SET
                nombre = @nombre,
                email = @email
            WHERE id_usuario = @idUsuario;
        ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        // =========================================================
        // ACTUALIZAR CONTRASEÑA
        // =========================================================
        public bool ActualizarPassword(
            int idUsuario,
            string passwordHash)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
            UPDATE Usuario
            SET password_hash = @passwordHash
            WHERE id_usuario = @idUsuario;
        ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@passwordHash",
                        passwordHash
                    );

                    command.Parameters.AddWithValue(
                        "@idUsuario",
                        idUsuario
                    );

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }

}