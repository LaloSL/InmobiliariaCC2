using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioPropietario
    {
        private readonly string _connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT id_propietario, dni, nombre, apellido, email, telefono, direccion, estado, fecha_creacion FROM propietarios WHERE estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32("id_propietario"),
                                Dni = reader.GetString("dni"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Email = reader.GetString("email"),
                                Telefono = reader.GetString("telefono"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                                Estado = reader.GetBoolean("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion")
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
                var sql = "SELECT id_propietario, dni, nombre, apellido, email, telefono, direccion, estado, fecha_creacion FROM propietarios WHERE id_propietario = @id";
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
                                IdPropietario = reader.GetInt32("id_propietario"),
                                Dni = reader.GetString("dni"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Email = reader.GetString("email"),
                                Telefono = reader.GetString("telefono"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                                Estado = reader.GetBoolean("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion")
                            };
                        }
                    }
                }
            }
            return propietario;
        }

        public int Guardar(Propietario p)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO propietarios (dni, nombre, apellido, email, telefono, direccion) 
                            VALUES (@dni, @nombre, @apellido, @email, @telefono, @direccion);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@direccion", (object?)p.Direccion ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificar(Propietario p)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE propietarios 
                            SET dni = @dni, nombre = @nombre, apellido = @apellido, email = @email, telefono = @telefono, direccion = @direccion 
                            WHERE id_propietario = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", p.IdPropietario);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@direccion", (object?)p.Direccion ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE propietarios SET estado = 0 WHERE id_propietario = @id;";
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