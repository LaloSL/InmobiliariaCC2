using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;


namespace InmobiliariaCC2.Repositories
{
    public class RepositorioInmueble
    {
        private readonly string _connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                i.IdInmueble,
                                i.Direccion,
                                i.Cupo,
                                i.Coordenadas,
                                i.PrecioDia,
                                i.Estado,
                                i.Foto,
                                t.IdTipo,
                                t.Nombre AS TipoNombre,
                                p.IdPropietario,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p ON i.IdPropietario = p.IdPropietario
                            WHERE i.Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearInmuebleCompleto(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                i.IdInmueble,
                                i.Direccion,
                                i.Cupo,
                                i.Coordenadas,
                                i.PrecioDia,
                                i.Estado,
                                i.Foto,
                                i.IdTipo,
                                i.IdPropietario,
                                t.Nombre AS TipoNombre,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido,
                                p.Email AS PropietarioEmail
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p ON i.IdPropietario = p.IdPropietario
                            WHERE i.IdInmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = MapearInmuebleCompleto(reader, incluirEmail: true);
                        }
                    }
                }
            }

            return inmueble;
        }

        public int Guardar(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Inmueble
                                (
                                    Direccion,
                                    Cupo,
                                    IdTipo,
                                    Coordenadas,
                                    PrecioDia,
                                    IdPropietario,
                                    Estado,
                                    Foto
                                )
                            VALUES
                                (
                                    @direccion,
                                    @cupo,
                                    @idTipo,
                                    @coordenadas,
                                    @precioDia,
                                    @idPropietario,
                                    1,
                                    @foto
                                );
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@idTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@coordenadas", (object?)inmueble.Coordenadas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@precioDia", inmueble.PrecioDia);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@foto", (object?)inmueble.Foto ?? DBNull.Value);

                    connection.Open();

                    var insertedId = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = insertedId;
                    return insertedId;
                }
            }
        }

        public int Actualizar(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Inmueble SET
                                Direccion = @direccion,
                                Cupo = @cupo,
                                IdTipo = @idTipo,
                                Coordenadas = @coordenadas,
                                PrecioDia = @precioDia,
                                IdPropietario = @idPropietario,
                                Foto = @foto
                            WHERE IdInmueble = @idInmueble;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", inmueble.IdInmueble);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@idTipo", inmueble.IdTipo);
                    command.Parameters.AddWithValue("@coordenadas", (object?)inmueble.Coordenadas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@precioDia", inmueble.PrecioDia);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@foto", (object?)inmueble.Foto ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Inmueble SET Estado = 0 WHERE IdInmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public List<Inmueble> BuscarDisponibles(DateTime desde, DateTime hasta)
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                i.IdInmueble,
                                i.Direccion,
                                i.Cupo,
                                i.Coordenadas,
                                i.PrecioDia,
                                i.Estado,
                                i.Foto,
                                t.IdTipo,
                                t.Nombre AS TipoNombre,
                                p.IdPropietario,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p ON i.IdPropietario = p.IdPropietario
                            WHERE i.Estado = 1
                              AND i.IdInmueble NOT IN
                              (
                                  SELECT IdInmueble
                                  FROM Reserva
                                  WHERE Estado = 1
                                    AND (
                                        FechaDesde < @hasta
                                        AND FechaHasta > @desde
                                    )
                              );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearInmuebleCompleto(reader));
                        }
                    }
                }
            }

            return lista;
        }

        #region Métodos Auxiliares de Mapeo
        private static Inmueble MapearInmuebleCompleto(MySqlDataReader reader, bool incluirEmail = false)
        {
            var inmueble = new Inmueble
            {
                IdInmueble = reader.GetInt32("IdInmueble"),
                Direccion = reader.GetString("Direccion"),
                Cupo = reader.GetInt32("Cupo"),
                Coordenadas = reader.IsDBNull(reader.GetOrdinal("Coordenadas"))
                    ? null
                    : reader.GetString("Coordenadas"),
                PrecioDia = reader.GetDecimal("PrecioDia"),
                Estado = reader.GetBoolean("Estado"),
                Foto = reader.IsDBNull(reader.GetOrdinal("Foto"))
                    ? null
                    : reader.GetString("Foto"),
                IdTipo = reader.GetInt32("IdTipo"),
                Tipo = new TipoInmueble
                {
                    IdTipo = reader.GetInt32("IdTipo"),
                    Nombre = reader.GetString("TipoNombre")
                },
                IdPropietario = reader.GetInt32("IdPropietario"),
                Propietario = new Propietario
                {
                    IdPropietario = reader.GetInt32("IdPropietario"),
                    Nombre = reader.GetString("PropietarioNombre"),
                    Apellido = reader.GetString("PropietarioApellido")
                }
            };

            if (incluirEmail && !reader.IsDBNull(reader.GetOrdinal("PropietarioEmail")))
            {
                inmueble.Propietario.Email = reader.GetString("PropietarioEmail");
            }

            return inmueble;
        }
        #endregion
    }
}