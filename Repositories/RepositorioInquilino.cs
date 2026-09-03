using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioInquilino
    {
        private readonly string _connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                IdInquilino,
                                Dni,
                                Nombre,
                                Apellido,
                                Email,
                                Telefono,
                                DireccionOrigen,
                                Estado,
                                FechaCreacion
                            FROM Inquilino
                            WHERE Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),

                                DireccionOrigen =
                                    reader.IsDBNull(reader.GetOrdinal("DireccionOrigen"))
                                    ? null
                                    : reader.GetString("DireccionOrigen"),

                                Estado = reader.GetBoolean("Estado"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion")
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                IdInquilino,
                                Dni,
                                Nombre,
                                Apellido,
                                Email,
                                Telefono,
                                DireccionOrigen,
                                Estado,
                                FechaCreacion
                            FROM Inquilino
                            WHERE IdInquilino = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),

                                DireccionOrigen =
                                    reader.IsDBNull(reader.GetOrdinal("DireccionOrigen"))
                                    ? null
                                    : reader.GetString("DireccionOrigen"),

                                Estado = reader.GetBoolean("Estado"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion")
                            };
                        }
                    }
                }
            }

            return inquilino;
        }

        public int Guardar(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Inquilino
                                (
                                    Dni,
                                    Nombre,
                                    Apellido,
                                    Email,
                                    Telefono,
                                    DireccionOrigen
                                )
                            VALUES
                                (
                                    @dni,
                                    @nombre,
                                    @apellido,
                                    @email,
                                    @telefono,
                                    @direccion
                                );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);

                    command.Parameters.AddWithValue(
                        "@direccion",
                        (object?)inquilino.DireccionOrigen ?? DBNull.Value
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificar(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Inquilino
                            SET
                                Dni = @dni,
                                Nombre = @nombre,
                                Apellido = @apellido,
                                Email = @email,
                                Telefono = @telefono,
                                DireccionOrigen = @direccion
                            WHERE IdInquilino = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.IdInquilino);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);

                    command.Parameters.AddWithValue(
                        "@direccion",
                        (object?)inquilino.DireccionOrigen ?? DBNull.Value
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Inquilino
                            SET Estado = 0
                            WHERE IdInquilino = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}