using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioPago
    {
        private readonly string _connectionString;

        public RepositorioPago(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión CadenaSQL."
                );
        }

        public List<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT
                        p.IdPago,
                        p.IdReserva,
                        p.Concepto,
                        p.Monto,
                        p.FechaPago,
                        p.MedioPago,
                        p.Observacion,
                        p.Estado
                    FROM Pago p
                    ORDER BY p.FechaPago DESC, p.IdPago DESC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPago(reader));
                        }
                    }
                }
            }

            return lista;
        }



        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT
                        p.IdPago,
                        p.IdReserva,
                        p.Concepto,
                        p.Monto,
                        p.FechaPago,
                        p.MedioPago,
                        p.Observacion,
                        p.Estado
                    FROM Pago p
                    WHERE p.IdPago = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = MapearPago(reader);
                        }
                    }
                }
            }

            return pago;
        }


        public List<Pago> ObtenerPorReserva(int idReserva)
        {
            var lista = new List<Pago>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT
                        p.IdPago,
                        p.IdReserva,
                        p.Concepto,
                        p.Monto,
                        p.FechaPago,
                        p.MedioPago,
                        p.Observacion,
                        p.Estado
                    FROM Pago p
                    WHERE p.IdReserva = @idReserva
                    ORDER BY p.FechaPago ASC, p.IdPago ASC;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@idReserva",
                        idReserva
                    );

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPago(reader));
                        }
                    }
                }
            }

            return lista;
        }


        public int Guardar(Pago pago)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    INSERT INTO Pago
                    (
                        IdReserva,
                        Concepto,
                        Monto,
                        FechaPago,
                        MedioPago,
                        Observacion,
                        Estado
                    )
                    VALUES
                    (
                        @idReserva,
                        @concepto,
                        @monto,
                        @fechaPago,
                        @medioPago,
                        @observacion,
                        @estado
                    );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@idReserva",
                        pago.IdReserva
                    );

                    command.Parameters.AddWithValue(
                        "@concepto",
                        pago.Concepto
                    );

                    command.Parameters.AddWithValue(
                        "@monto",
                        pago.Monto
                    );

                    command.Parameters.AddWithValue(
                        "@fechaPago",
                        pago.FechaPago
                    );

                    command.Parameters.AddWithValue(
                        "@medioPago",
                        pago.MedioPago
                    );

                    command.Parameters.AddWithValue(
                        "@observacion",
                        (object?)pago.Observacion ?? DBNull.Value
                    );

                    command.Parameters.AddWithValue(
                        "@estado",
                        pago.Estado
                    );

                    connection.Open();

                    command.ExecuteNonQuery();

                    return (int)command.LastInsertedId;
                }
            }
        }

        public int ModificarConcepto(int idPago, string concepto)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE Pago
                    SET Concepto = @concepto
                    WHERE IdPago = @idPago;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@concepto",
                        concepto
                    );

                    command.Parameters.AddWithValue(
                        "@idPago",
                        idPago
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }


        public decimal ObtenerTotalPagado(int idReserva)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    SELECT COALESCE(SUM(Monto), 0)
                    FROM Pago
                    WHERE IdReserva = @idReserva
                      AND Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@idReserva",
                        idReserva
                    );

                    connection.Open();

                    var resultado = command.ExecuteScalar();

                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        return 0m;
                    }

                    return Convert.ToDecimal(resultado);
                }
            }
        }


        public int Anular(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"
                    UPDATE Pago
                    SET Estado = 0
                    WHERE IdPago = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id",
                        id
                    );

                    connection.Open();

                    return command.ExecuteNonQuery();
                }
            }
        }

        private Pago MapearPago(MySqlDataReader reader)
        {
            return new Pago
            {
                IdPago =
                    reader.GetInt32("IdPago"),

                IdReserva =
                    reader.GetInt32("IdReserva"),

                Concepto =
                    reader.GetString("Concepto"),

                Monto =
                    reader.GetDecimal("Monto"),

                FechaPago =
                    reader.GetDateTime("FechaPago"),

                MedioPago =
                    reader.GetString("MedioPago"),

                Observacion =
                    reader.IsDBNull(
                        reader.GetOrdinal("Observacion")
                    )
                    ? null
                    : reader.GetString("Observacion"),

                Estado =
                    reader.GetBoolean("Estado")
            };
        }
    }
}