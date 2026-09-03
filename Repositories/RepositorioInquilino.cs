using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioInquilino
    {
        private readonly string _connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT id_inquilino, dni, nombre, apellido, email, telefono, direccion_origen, estado, fecha_creacion FROM inquilinos WHERE estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("id_inquilino"),
                                Dni = reader.GetString("dni"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Email = reader.GetString("email"),
                                Telefono = reader.GetString("telefono"),
                                DireccionOrigen = reader.IsDBNull(reader.GetOrdinal("direccion_origen")) ? null : reader.GetString("direccion_origen"),
                                Estado = reader.GetBoolean("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion")
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
                var sql = "SELECT id_inquilino, dni, nombre, apellido, email, telefono, direccion_origen, estado, fecha_creacion FROM inquilinos WHERE id_inquilino = @id";
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
                                IdInquilino = reader.GetInt32("id_inquilino"),
                                Dni = reader.GetString("dni"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Email = reader.GetString("email"),
                                Telefono = reader.GetString("telefono"),
                                DireccionOrigen = reader.IsDBNull(reader.GetOrdinal("direccion_origen")) ? null : reader.GetString("direccion_origen"),
                                Estado = reader.GetBoolean("estado"),
                                FechaCreacion = reader.GetDateTime("fecha_creacion")
                            };
                        }
                    }
                }
            }
            return inquilino;
        }

        public int Guardar(Inquilino i)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO inquilinos (dni, nombre, apellido, email, telefono, direccion_origen) 
                            VALUES (@dni, @nombre, @apellido, @email, @telefono, @direccion);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@direccion", (object?)i.DireccionOrigen ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificar(Inquilino i)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE inquilinos 
                            SET dni = @dni, nombre = @nombre, apellido = @apellido, email = @email, telefono = @telefono, direccion_origen = @direccion 
                            WHERE id_inquilino = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", i.IdInquilino);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@direccion", (object?)i.DireccionOrigen ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "UPDATE inquilinos SET estado = 0 WHERE id_inquilino = @id;";
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