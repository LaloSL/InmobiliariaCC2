using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioInforme
    {
        private readonly string _connectionString;

        public RepositorioInforme(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public List<InformeInmueble> ObtenerMasReservados(
            int dias = 365)
        {
            var lista = new List<InformeInmueble>();

            using var connection =
                new MySqlConnection(_connectionString);

            var sql = @"
                SELECT
                    i.IdInmueble,
                    i.Direccion,
                    i.PrecioDia,
                    i.Estado,
                    i.IdPropietario,

                    CONCAT(
                        p.Nombre,
                        ' ',
                        p.Apellido
                    ) AS Propietario,

                    COUNT(r.IdReserva) AS CantidadReservas,

                    MAX(r.FechaDesde) AS UltimaReserva

                FROM Inmueble i

                INNER JOIN Propietario p
                    ON p.IdPropietario =
                       i.IdPropietario

                LEFT JOIN Reserva r
                    ON r.IdInmueble =
                       i.IdInmueble
                    AND r.Estado = 1
                    AND r.FechaDesde >=
                        DATE_SUB(
                            CURDATE(),
                            INTERVAL @dias DAY
                        )

                GROUP BY
                    i.IdInmueble,
                    i.Direccion,
                    i.PrecioDia,
                    i.Estado,
                    i.IdPropietario,
                    p.Nombre,
                    p.Apellido

                HAVING COUNT(r.IdReserva) > 0

                ORDER BY
                    CantidadReservas DESC,
                    i.Direccion ASC;
            ";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@dias",
                dias
            );

            connection.Open();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new InformeInmueble
                {
                    IdInmueble =
                        reader.GetInt32("IdInmueble"),

                    Direccion =
                        reader.GetString("Direccion"),

                    PrecioDia =
                        reader.GetDecimal("PrecioDia"),

                    Estado =
                        reader.GetBoolean("Estado"),

                    IdPropietario =
                        reader.GetInt32("IdPropietario"),

                    Propietario =
                        reader.GetString("Propietario"),

                    CantidadReservas =
                        Convert.ToInt32(
                            reader["CantidadReservas"]
                        ),

                    UltimaReserva =
                        reader.IsDBNull(
                            reader.GetOrdinal(
                                "UltimaReserva"
                            )
                        )
                            ? null
                            : reader.GetDateTime(
                                "UltimaReserva"
                            )
                });
            }

            return lista;
        }

        public List<InformeInmueble>
            ObtenerSinReservas(int dias)
        {
            var lista =
                new List<InformeInmueble>();

            using var connection =
                new MySqlConnection(
                    _connectionString
                );

            var sql = @"
                SELECT
                    i.IdInmueble,
                    i.Direccion,
                    i.PrecioDia,
                    i.Estado,
                    i.IdPropietario,

                    CONCAT(
                        p.Nombre,
                        ' ',
                        p.Apellido
                    ) AS Propietario,

                    MAX(r.FechaDesde)
                        AS UltimaReserva

                FROM Inmueble i

                INNER JOIN Propietario p
                    ON p.IdPropietario =
                       i.IdPropietario

                LEFT JOIN Reserva r
                    ON r.IdInmueble =
                       i.IdInmueble
                    AND r.Estado = 1

                GROUP BY
                    i.IdInmueble,
                    i.Direccion,
                    i.PrecioDia,
                    i.Estado,
                    i.IdPropietario,
                    p.Nombre,
                    p.Apellido

                HAVING
                    MAX(r.FechaDesde) IS NULL

                    OR MAX(r.FechaDesde) <
                       DATE_SUB(
                           CURDATE(),
                           INTERVAL @dias DAY
                       )

                ORDER BY
                    i.Direccion ASC;
            ";

            using var command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@dias",
                dias
            );

            connection.Open();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    new InformeInmueble
                    {
                        IdInmueble =
                            reader.GetInt32(
                                "IdInmueble"
                            ),

                        Direccion =
                            reader.GetString(
                                "Direccion"
                            ),

                        PrecioDia =
                            reader.GetDecimal(
                                "PrecioDia"
                            ),

                        Estado =
                            reader.GetBoolean(
                                "Estado"
                            ),

                        IdPropietario =
                            reader.GetInt32(
                                "IdPropietario"
                            ),

                        Propietario =
                            reader.GetString(
                                "Propietario"
                            ),

                        UltimaReserva =
                            reader.IsDBNull(
                                reader.GetOrdinal(
                                    "UltimaReserva"
                                )
                            )
                                ? null
                                : reader.GetDateTime(
                                    "UltimaReserva"
                                )
                    }
                );
            }

            return lista;
        }

        public List<InformeReserva>
            ObtenerReservasVigentes()
        {
            var lista =
                new List<InformeReserva>();

            using var connection =
                new MySqlConnection(
                    _connectionString
                );

            var sql = @"
                SELECT
                    r.IdReserva,

                    CONCAT(
                        iq.Nombre,
                        ' ',
                        iq.Apellido
                    ) AS Inquilino,

                    i.Direccion AS Inmueble,

                    r.FechaDesde,
                    r.FechaHasta,
                    r.MontoDia,
                    r.Estado,

                    DATEDIFF(
                        r.FechaHasta,
                        CURDATE()
                    ) AS DiasRestantes

                FROM Reserva r

                INNER JOIN Inquilino iq
                    ON iq.IdInquilino =
                       r.IdInquilino

                INNER JOIN Inmueble i
                    ON i.IdInmueble =
                       r.IdInmueble

                WHERE
                    r.Estado = 1

                    AND CURDATE()
                        BETWEEN r.FechaDesde
                        AND r.FechaHasta

                ORDER BY
                    r.FechaHasta ASC;
            ";

            using var command =
                new MySqlCommand(
                    sql,
                    connection
                );

            connection.Open();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    new InformeReserva
                    {
                        IdReserva =
                            reader.GetInt32(
                                "IdReserva"
                            ),

                        Inquilino =
                            reader.GetString(
                                "Inquilino"
                            ),

                        Inmueble =
                            reader.GetString(
                                "Inmueble"
                            ),

                        FechaDesde =
                            reader.GetDateTime(
                                "FechaDesde"
                            ),

                        FechaHasta =
                            reader.GetDateTime(
                                "FechaHasta"
                            ),

                        MontoDia =
                            reader.GetDecimal(
                                "MontoDia"
                            ),

                        Estado =
                            reader.GetBoolean(
                                "Estado"
                            ),

                        DiasRestantes =
                            reader.GetInt32(
                                "DiasRestantes"
                            )
                    }
                );
            }

            return lista;
        }

        public List<InformeReserva>
            ObtenerReservasPorFinalizar(int dias)
        {
            var lista =
                new List<InformeReserva>();

            using var connection =
                new MySqlConnection(
                    _connectionString
                );

            var sql = @"
                SELECT
                    r.IdReserva,

                    CONCAT(
                        iq.Nombre,
                        ' ',
                        iq.Apellido
                    ) AS Inquilino,

                    i.Direccion AS Inmueble,

                    r.FechaDesde,
                    r.FechaHasta,
                    r.MontoDia,
                    r.Estado,

                    DATEDIFF(
                        r.FechaHasta,
                        CURDATE()
                    ) AS DiasRestantes

                FROM Reserva r

                INNER JOIN Inquilino iq
                    ON iq.IdInquilino =
                       r.IdInquilino

                INNER JOIN Inmueble i
                    ON i.IdInmueble =
                       r.IdInmueble

                WHERE
                    r.Estado = 1

                    AND r.FechaHasta
                        BETWEEN CURDATE()
                        AND DATE_ADD(
                            CURDATE(),
                            INTERVAL @dias DAY
                        )

                ORDER BY
                    r.FechaHasta ASC;
            ";

            using var command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@dias",
                dias
            );

            connection.Open();

            using var reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    new InformeReserva
                    {
                        IdReserva =
                            reader.GetInt32(
                                "IdReserva"
                            ),

                        Inquilino =
                            reader.GetString(
                                "Inquilino"
                            ),

                        Inmueble =
                            reader.GetString(
                                "Inmueble"
                            ),

                        FechaDesde =
                            reader.GetDateTime(
                                "FechaDesde"
                            ),

                        FechaHasta =
                            reader.GetDateTime(
                                "FechaHasta"
                            ),

                        MontoDia =
                            reader.GetDecimal(
                                "MontoDia"
                            ),

                        Estado =
                            reader.GetBoolean(
                                "Estado"
                            ),

                        DiasRestantes =
                            reader.GetInt32(
                                "DiasRestantes"
                            )
                    }
                );
            }

            return lista;
        }
    }
}