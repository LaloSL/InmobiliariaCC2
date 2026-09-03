using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioReserva
    {
        private readonly string _connectionString;

        public RepositorioReserva(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public bool InmuebleOcupado(
            int idInmueble,
            DateTime desde,
            DateTime hasta,
            int? idReservaExcluir = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT COUNT(*)
                            FROM Reserva
                            WHERE IdInmueble = @idInmueble
                              AND Estado = 1
                              AND (
                                  @idReservaExcluir IS NULL
                                  OR IdReserva != @idReservaExcluir
                              )
                              AND (
                                  FechaDesde < @hasta
                                  AND FechaHasta > @desde
                              );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@idInmueble",
                        idInmueble
                    );

                    command.Parameters.AddWithValue(
                        "@desde",
                        desde
                    );

                    command.Parameters.AddWithValue(
                        "@hasta",
                        hasta
                    );

                    command.Parameters.AddWithValue(
                        "@idReservaExcluir",
                        (object?)idReservaExcluir ?? DBNull.Value
                    );

                    connection.Open();

                    var resultado = command.ExecuteScalar();

                    long cantidad = Convert.ToInt64(resultado);

                    return cantidad > 0;
                }
            }
        }

        public List<Reserva> ObtenerTodas()
        {
            var lista = new List<Reserva>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                r.IdReserva,
                                r.MontoDia,
                                r.FechaDesde,
                                r.FechaHasta,
                                r.FechaTerminacionAnticipada,
                                r.Multa,
                                r.Estado,

                                i.Nombre AS InquilinoNombre,
                                i.Apellido AS InquilinoApellido,

                                inm.Direccion AS InmuebleDireccion

                            FROM Reserva r

                            INNER JOIN Inquilino i
                                ON r.IdInquilino = i.IdInquilino

                            INNER JOIN Inmueble inm
                                ON r.IdInmueble = inm.IdInmueble

                            ORDER BY r.FechaDesde DESC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reserva
                            {
                                IdReserva =
                                    reader.GetInt32("IdReserva"),

                                MontoDia =
                                    reader.GetDecimal("MontoDia"),

                                FechaDesde =
                                    reader.GetDateTime("FechaDesde"),

                                FechaHasta =
                                    reader.GetDateTime("FechaHasta"),

                                FechaTerminacionAnticipada =
                                    reader.IsDBNull(
                                        reader.GetOrdinal("FechaTerminacionAnticipada"))
                                        ? null
                                        : reader.GetDateTime(
                                            "FechaTerminacionAnticipada"),

                                Multa =
                                    reader.GetDecimal("Multa"),

                                Estado =
                                    reader.GetBoolean("Estado"),

                                Inquilino = new Inquilino
                                {
                                    Nombre =
                                        reader.GetString("InquilinoNombre"),

                                    Apellido =
                                        reader.GetString("InquilinoApellido")
                                },

                                Inmueble = new Inmueble
                                {
                                    Direccion =
                                        reader.GetString("InmuebleDireccion")
                                }
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public int Guardar(Reserva reserva)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Reserva
                                (
                                    IdInquilino,
                                    IdInmueble,
                                    MontoDia,
                                    FechaDesde,
                                    FechaHasta
                                )
                            VALUES
                                (
                                    @idInquilino,
                                    @idInmueble,
                                    @montoDia,
                                    @fechaDesde,
                                    @fechaHasta
                                );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@idInquilino",
                        reserva.IdInquilino
                    );

                    command.Parameters.AddWithValue(
                        "@idInmueble",
                        reserva.IdInmueble
                    );

                    command.Parameters.AddWithValue(
                        "@montoDia",
                        reserva.MontoDia
                    );

                    command.Parameters.AddWithValue(
                        "@fechaDesde",
                        reserva.FechaDesde
                    );

                    command.Parameters.AddWithValue(
                        "@fechaHasta",
                        reserva.FechaHasta
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        public Reserva? ObtenerPorIdConDetalles(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT
                                r.IdReserva,
                                r.MontoDia,
                                r.FechaDesde,
                                r.FechaHasta,
                                r.FechaTerminacionAnticipada,
                                r.Multa,
                                r.Estado,

                                i.IdInquilino,
                                i.Nombre AS InquilinoNombre,
                                i.Apellido AS InquilinoApellido,
                                i.Email AS InquilinoEmail,

                                inm.IdInmueble,
                                inm.Direccion AS InmuebleDireccion,
                                inm.PrecioDia

                            FROM Reserva r

                            INNER JOIN Inquilino i
                                ON r.IdInquilino = i.IdInquilino

                            INNER JOIN Inmueble inm
                                ON r.IdInmueble = inm.IdInmueble

                            WHERE r.IdReserva = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reserva = new Reserva
                            {
                                IdReserva =
                                    reader.GetInt32("IdReserva"),

                                MontoDia =
                                    reader.GetDecimal("MontoDia"),

                                FechaDesde =
                                    reader.GetDateTime("FechaDesde"),

                                FechaHasta =
                                    reader.GetDateTime("FechaHasta"),

                                FechaTerminacionAnticipada =
                                    reader.IsDBNull(
                                        reader.GetOrdinal("FechaTerminacionAnticipada"))
                                        ? null
                                        : reader.GetDateTime(
                                            "FechaTerminacionAnticipada"),

                                Multa =
                                    reader.GetDecimal("Multa"),

                                Estado =
                                    reader.GetBoolean("Estado"),

                                IdInquilino =
                                    reader.GetInt32("IdInquilino"),

                                Inquilino = new Inquilino
                                {
                                    IdInquilino =
                                        reader.GetInt32("IdInquilino"),

                                    Nombre =
                                        reader.GetString("InquilinoNombre"),

                                    Apellido =
                                        reader.GetString("InquilinoApellido"),

                                    Email =
                                        reader.GetString("InquilinoEmail")
                                },

                                IdInmueble =
                                    reader.GetInt32("IdInmueble"),

                                Inmueble = new Inmueble
                                {
                                    IdInmueble =
                                        reader.GetInt32("IdInmueble"),

                                    Direccion =
                                        reader.GetString("InmuebleDireccion"),

                                    PrecioDia =
                                        reader.GetDecimal("PrecioDia")
                                }
                            };
                        }
                    }
                }
            }

            return reserva;
        }

        public int FinalizarAnticipadamente(
            int idReserva,
            DateTime fechaTerminacion,
            decimal multa)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"UPDATE Reserva
                            SET
                                FechaTerminacionAnticipada = @fechaTerminacion,
                                Multa = @multa,
                                Estado = 0
                            WHERE IdReserva = @idReserva;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@fechaTerminacion",
                        fechaTerminacion
                    );

                    command.Parameters.AddWithValue(
                        "@multa",
                        multa
                    );

                    command.Parameters.AddWithValue(
                        "@idReserva",
                        idReserva
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}