using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
  public class RepositorioSolicitudReserva
  {
    private readonly string _connectionString;

    public RepositorioSolicitudReserva(IConfiguration configuration)
    {
      _connectionString = configuration.GetConnectionString("CadenaSQL")
          ?? throw new InvalidOperationException(
              "No se encontró la cadena de conexión CadenaSQL."
          );
    }


    // =====================================================
    // OBTENER TODAS LAS SOLICITUDES
    // =====================================================

    public List<SolicitudReserva> ObtenerTodas()
    {
      var lista = new List<SolicitudReserva>();

      using (var connection = new MySqlConnection(_connectionString))
      {
        var sql = @"
                    SELECT
                        sr.IdSolicitud,
                        sr.Nombre,
                        sr.Apellido,
                        sr.Dni,
                        sr.Email,
                        sr.Telefono,
                        sr.IdInmueble,
                        sr.FechaDesde,
                        sr.FechaHasta,
                        sr.FechaSolicitud,
                        sr.Estado,

                        i.Direccion,
                        i.PrecioDia,
                        i.Cupo

                    FROM SolicitudReserva sr

                    INNER JOIN Inmueble i
                        ON sr.IdInmueble = i.IdInmueble

                    ORDER BY
                        CASE
                            WHEN sr.Estado = 'Pendiente' THEN 0
                            ELSE 1
                        END,
                        sr.FechaSolicitud DESC;";

        using (var command = new MySqlCommand(sql, connection))
        {
          connection.Open();

          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              lista.Add(new SolicitudReserva
              {
                IdSolicitud =
                      reader.GetInt32("IdSolicitud"),

                Nombre =
                      reader.GetString("Nombre"),

                Apellido =
                      reader.GetString("Apellido"),

                Dni =
                      reader.GetString("Dni"),

                Email =
                      reader.GetString("Email"),

                Telefono =
                      reader.GetString("Telefono"),

                IdInmueble =
                      reader.GetInt32("IdInmueble"),

                FechaDesde =
                      reader.GetDateTime("FechaDesde"),

                FechaHasta =
                      reader.GetDateTime("FechaHasta"),

                FechaSolicitud =
                      reader.GetDateTime("FechaSolicitud"),

                Estado =
                      reader.GetString("Estado"),

                Inmueble = new Inmueble
                {
                  IdInmueble =
                          reader.GetInt32("IdInmueble"),

                  Direccion =
                          reader.GetString("Direccion"),

                  PrecioDia =
                          reader.GetDecimal("PrecioDia"),

                  Cupo =
                          reader.GetInt32("Cupo")
                }
              });
            }
          }
        }
      }

      return lista;
    }


    // =====================================================
    // GUARDAR UNA NUEVA SOLICITUD
    // =====================================================

    public int Guardar(SolicitudReserva solicitud)
    {
      using (var connection = new MySqlConnection(_connectionString))
      {
        var sql = @"
                    INSERT INTO SolicitudReserva
                    (
                        Nombre,
                        Apellido,
                        Dni,
                        Email,
                        Telefono,
                        IdInmueble,
                        FechaDesde,
                        FechaHasta,
                        Estado
                    )
                    VALUES
                    (
                        @nombre,
                        @apellido,
                        @dni,
                        @email,
                        @telefono,
                        @idInmueble,
                        @fechaDesde,
                        @fechaHasta,
                        @estado
                    );";

        using (var command = new MySqlCommand(sql, connection))
        {
          command.Parameters.AddWithValue(
              "@nombre",
              solicitud.Nombre
          );

          command.Parameters.AddWithValue(
              "@apellido",
              solicitud.Apellido
          );

          command.Parameters.AddWithValue(
              "@dni",
              solicitud.Dni
          );

          command.Parameters.AddWithValue(
              "@email",
              solicitud.Email
          );

          command.Parameters.AddWithValue(
              "@telefono",
              solicitud.Telefono
          );

          command.Parameters.AddWithValue(
              "@idInmueble",
              solicitud.IdInmueble
          );

          command.Parameters.AddWithValue(
              "@fechaDesde",
              solicitud.FechaDesde
          );

          command.Parameters.AddWithValue(
              "@fechaHasta",
              solicitud.FechaHasta
          );

          command.Parameters.AddWithValue(
              "@estado",
              solicitud.Estado
          );

          connection.Open();

          command.ExecuteNonQuery();

          return (int)command.LastInsertedId;
        }
      }
    }

    // =====================================================
    // OBTENER SOLICITUD POR ID
    // =====================================================

    public SolicitudReserva? ObtenerPorId(int id)
    {
      SolicitudReserva? solicitud = null;

      using (var connection = new MySqlConnection(_connectionString))
      {
        var sql = @"
            SELECT
                sr.IdSolicitud,
                sr.Nombre,
                sr.Apellido,
                sr.Dni,
                sr.Email,
                sr.Telefono,
                sr.IdInmueble,
                sr.FechaDesde,
                sr.FechaHasta,
                sr.FechaSolicitud,
                sr.Estado,

                i.Direccion,
                i.PrecioDia,
                i.Cupo

            FROM SolicitudReserva sr

            INNER JOIN Inmueble i
                ON sr.IdInmueble = i.IdInmueble

            WHERE sr.IdSolicitud = @id;";

        using (var command = new MySqlCommand(sql, connection))
        {
          command.Parameters.AddWithValue("@id", id);

          connection.Open();

          using (var reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              solicitud = new SolicitudReserva
              {
                IdSolicitud =
                      reader.GetInt32("IdSolicitud"),

                Nombre =
                      reader.GetString("Nombre"),

                Apellido =
                      reader.GetString("Apellido"),

                Dni =
                      reader.GetString("Dni"),

                Email =
                      reader.GetString("Email"),

                Telefono =
                      reader.GetString("Telefono"),

                IdInmueble =
                      reader.GetInt32("IdInmueble"),

                FechaDesde =
                      reader.GetDateTime("FechaDesde"),

                FechaHasta =
                      reader.GetDateTime("FechaHasta"),

                FechaSolicitud =
                      reader.GetDateTime("FechaSolicitud"),

                Estado =
                      reader.GetString("Estado"),

                Inmueble = new Inmueble
                {
                  IdInmueble =
                          reader.GetInt32("IdInmueble"),

                  Direccion =
                          reader.GetString("Direccion"),

                  PrecioDia =
                          reader.GetDecimal("PrecioDia"),

                  Cupo =
                          reader.GetInt32("Cupo")
                }
              };
            }
          }
        }
      }

      return solicitud;
    }


    // =====================================================
    // CAMBIAR ESTADO DE LA SOLICITUD
    // =====================================================

    public int CambiarEstado(
        int idSolicitud,
        string nuevoEstado)
    {
      using (var connection = new MySqlConnection(_connectionString))
      {
        var sql = @"
            UPDATE SolicitudReserva
            SET Estado = @estado
            WHERE IdSolicitud = @idSolicitud;";

        using (var command = new MySqlCommand(sql, connection))
        {
          command.Parameters.AddWithValue(
              "@estado",
              nuevoEstado
          );

          command.Parameters.AddWithValue(
              "@idSolicitud",
              idSolicitud
          );

          connection.Open();

          return command.ExecuteNonQuery();
        }
      }
    }
  }
}