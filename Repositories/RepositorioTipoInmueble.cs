using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioTipoInmueble
    {
        private readonly string _connectionString;

        public RepositorioTipoInmueble(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public List<TipoInmueble> ObtenerTodos()
        {
            var lista = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                IdTipo,
                                Nombre,
                                Estado
                            FROM TipoInmueble
                            WHERE Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TipoInmueble
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
                                Nombre = reader.GetString("Nombre"),
                                Estado = reader.GetBoolean("Estado")
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipo = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                IdTipo,
                                Nombre,
                                Estado
                            FROM TipoInmueble
                            WHERE IdTipo = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new TipoInmueble
                            {
                                IdTipo = reader.GetInt32("IdTipo"),
                                Nombre = reader.GetString("Nombre"),
                                Estado = reader.GetBoolean("Estado")
                            };
                        }
                    }
                }
            }

            return tipo;
        }

        public int Guardar(TipoInmueble tipo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO TipoInmueble
                                (Nombre)
                            VALUES
                                (@nombre);";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Editar(TipoInmueble tipo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE TipoInmueble
                            SET Nombre = @nombre
                            WHERE IdTipo = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipo.Nombre);
                    command.Parameters.AddWithValue("@id", tipo.IdTipo);

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int BajaLogica(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE TipoInmueble
                            SET Estado = 0
                            WHERE IdTipo = @id;";

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