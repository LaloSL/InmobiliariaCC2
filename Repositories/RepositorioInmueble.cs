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
                                t.IdTipo,
                                t.Nombre AS TipoNombre,
                                p.IdPropietario,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t
                                ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p
                                ON i.IdPropietario = p.IdPropietario
                            WHERE i.Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),

                                Coordenadas = reader.IsDBNull(
                                    reader.GetOrdinal("Coordenadas"))
                                    ? null
                                    : reader.GetString("Coordenadas"),

                                PrecioDia = reader.GetDecimal("PrecioDia"),
                                Estado = reader.GetBoolean("Estado"),

                                IdTipo = reader.GetInt32("IdTipo"),

                                Tipo = new TipoInmueble
                                {
                                    IdTipo = reader.GetInt32("IdTipo"),
                                    Nombre = reader.GetString("TipoNombre")
                                },

                                IdPropietario =
                                    reader.GetInt32("IdPropietario"),

                                Propietario = new Propietario
                                {
                                    IdPropietario =
                                        reader.GetInt32("IdPropietario"),

                                    Nombre =
                                        reader.GetString("PropietarioNombre"),

                                    Apellido =
                                        reader.GetString("PropietarioApellido")
                                }
                            });
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
                                i.IdTipo,
                                i.IdPropietario,
                                t.Nombre AS TipoNombre,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido,
                                p.Email AS PropietarioEmail
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t
                                ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p
                                ON i.IdPropietario = p.IdPropietario
                            WHERE i.IdInmueble = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble =
                                    reader.GetInt32("IdInmueble"),

                                Direccion =
                                    reader.GetString("Direccion"),

                                Cupo =
                                    reader.GetInt32("Cupo"),

                                Coordenadas = reader.IsDBNull(
                                    reader.GetOrdinal("Coordenadas"))
                                    ? null
                                    : reader.GetString("Coordenadas"),

                                PrecioDia =
                                    reader.GetDecimal("PrecioDia"),

                                Estado =
                                    reader.GetBoolean("Estado"),

                                IdTipo =
                                    reader.GetInt32("IdTipo"),

                                Tipo = new TipoInmueble
                                {
                                    IdTipo =
                                        reader.GetInt32("IdTipo"),

                                    Nombre =
                                        reader.GetString("TipoNombre")
                                },

                                IdPropietario =
                                    reader.GetInt32("IdPropietario"),

                                Propietario = new Propietario
                                {
                                    IdPropietario =
                                        reader.GetInt32("IdPropietario"),

                                    Nombre =
                                        reader.GetString("PropietarioNombre"),

                                    Apellido =
                                        reader.GetString("PropietarioApellido"),

                                    Email =
                                        reader.GetString("PropietarioEmail")
                                }
                            };
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
                                    IdPropietario
                                )
                            VALUES
                                (
                                    @direccion,
                                    @cupo,
                                    @idTipo,
                                    @coordenadas,
                                    @precioDia,
                                    @idPropietario
                                );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@direccion",
                        inmueble.Direccion
                    );

                    command.Parameters.AddWithValue(
                        "@cupo",
                        inmueble.Cupo
                    );

                    command.Parameters.AddWithValue(
                        "@idTipo",
                        inmueble.IdTipo
                    );

                    command.Parameters.AddWithValue(
                        "@coordenadas",
                        (object?)inmueble.Coordenadas ?? DBNull.Value
                    );

                    command.Parameters.AddWithValue(
                        "@precioDia",
                        inmueble.PrecioDia
                    );

                    command.Parameters.AddWithValue(
                        "@idPropietario",
                        inmueble.IdPropietario
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public List<Inmueble> BuscarDisponibles(
            DateTime desde,
            DateTime hasta)
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
                                t.Nombre AS TipoNombre,
                                p.Nombre AS PropietarioNombre,
                                p.Apellido AS PropietarioApellido
                            FROM Inmueble i
                            INNER JOIN TipoInmueble t
                                ON i.IdTipo = t.IdTipo
                            INNER JOIN Propietario p
                                ON i.IdPropietario = p.IdPropietario
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
                    command.Parameters.AddWithValue(
                        "@desde",
                        desde
                    );

                    command.Parameters.AddWithValue(
                        "@hasta",
                        hasta
                    );

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                IdInmueble =
                                    reader.GetInt32("IdInmueble"),

                                Direccion =
                                    reader.GetString("Direccion"),

                                Cupo =
                                    reader.GetInt32("Cupo"),

                                Coordenadas = reader.IsDBNull(
                                    reader.GetOrdinal("Coordenadas"))
                                    ? null
                                    : reader.GetString("Coordenadas"),

                                PrecioDia =
                                    reader.GetDecimal("PrecioDia"),

                                Tipo = new TipoInmueble
                                {
                                    Nombre =
                                        reader.GetString("TipoNombre")
                                },

                                Propietario = new Propietario
                                {
                                    Nombre =
                                        reader.GetString("PropietarioNombre"),

                                    Apellido =
                                        reader.GetString("PropietarioApellido")
                                }
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}