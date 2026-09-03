using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioPropietario
    {
        private readonly string _connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }


        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                IdPropietario,
                                Dni,
                                Nombre,
                                Apellido,
                                Email,
                                Telefono,
                                Direccion,
                                Estado,
                                FechaCreacion
                            FROM Propietario
                            WHERE Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion"))
                                    ? null
                                    : reader.GetString("Direccion"),
                                Estado = reader.GetBoolean("Estado"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion")
                            });
                        }
                    }
                }
            }

            return lista;
        }


        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT 
                                IdPropietario,
                                Dni,
                                Nombre,
                                Apellido,
                                Email,
                                Telefono,
                                Direccion,
                                Estado,
                                FechaCreacion
                            FROM Propietario
                            WHERE IdPropietario = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion"))
                                    ? null
                                    : reader.GetString("Direccion"),
                                Estado = reader.GetBoolean("Estado"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion")
                            };
                        }
                    }
                }
            }

            return propietario;
        }


        public int Guardar(Propietario propietario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Propietario
                                (Dni, Nombre, Apellido, Email, Telefono, Direccion)
                            VALUES
                                (@dni, @nombre, @apellido, @email, @telefono, @direccion);";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@email", propietario.Email);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono);

                    command.Parameters.AddWithValue(
                        "@direccion",
                        (object?)propietario.Direccion ?? DBNull.Value
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }


        public int Modificar(Propietario propietario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Propietario
                            SET
                                Dni = @dni,
                                Nombre = @nombre,
                                Apellido = @apellido,
                                Email = @email,
                                Telefono = @telefono,
                                Direccion = @direccion
                            WHERE IdPropietario = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id",
                        propietario.IdPropietario
                    );

                    command.Parameters.AddWithValue(
                        "@dni",
                        propietario.Dni
                    );

                    command.Parameters.AddWithValue(
                        "@nombre",
                        propietario.Nombre
                    );

                    command.Parameters.AddWithValue(
                        "@apellido",
                        propietario.Apellido
                    );

                    command.Parameters.AddWithValue(
                        "@email",
                        propietario.Email
                    );

                    command.Parameters.AddWithValue(
                        "@telefono",
                        propietario.Telefono
                    );

                    command.Parameters.AddWithValue(
                        "@direccion",
                        (object?)propietario.Direccion ?? DBNull.Value
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
                var sql = @"UPDATE Propietario
                            SET Estado = 0
                            WHERE IdPropietario = @id;";

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
