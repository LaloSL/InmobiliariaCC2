using MySql.Data.MySqlClient;
using InmobiliariaCC2.Models;

namespace InmobiliariaCC2.Repositories
{
    public class RepositorioInmueble
    {
        private readonly string _connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"SELECT i.id_inmueble, i.direccion, i.cupo, i.coordenadas, i.precio_dia, i.estado,
                                   t.id_tipo, t.nombre AS tipo_nombre,
                                   p.id_propietario, p.nombre AS prop_nombre, p.apellido AS prop_apellido
                            FROM inmuebles i
                            INNER JOIN tipos_inmueble t ON i.id_tipo = t.id_tipo
                            INNER JOIN propietarios p ON i.id_propietario = p.id_propietario
                            WHERE i.estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Cupo = reader.GetInt32("cupo"),
                                Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas")) ? null : reader.GetString("coordenadas"),
                                PrecioDia = reader.GetDecimal("precio_dia"),
                                Estado = reader.GetBoolean("estado"),
                                IdTipo = reader.GetInt32("id_tipo"),
                                Tipo = new TipoInmueble { IdTipo = reader.GetInt32("id_tipo"), Nombre = reader.GetString("tipo_nombre") },
                                IdPropietario = reader.GetInt32("id_propietario"),
                                Propietario = new Propietario { IdPropietario = reader.GetInt32("id_propietario"), Nombre = reader.GetString("prop_nombre"), Apellido = reader.GetString("prop_apellido") }
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
                var sql = @"SELECT i.id_inmueble, i.direccion, i.cupo, i.coordenadas, i.precio_dia, i.estado, i.id_tipo, i.id_propietario,
                                   t.nombre AS tipo_nombre,
                                   p.nombre AS prop_nombre, p.apellido AS prop_apellido, p.email AS prop_email
                            FROM inmuebles i
                            INNER JOIN tipos_inmueble t ON i.id_tipo = t.id_tipo
                            INNER JOIN propietarios p ON i.id_propietario = p.id_propietario
                            WHERE i.id_inmueble = @id";

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
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Cupo = reader.GetInt32("cupo"),
                                Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas")) ? null : reader.GetString("coordenadas"),
                                PrecioDia = reader.GetDecimal("precio_dia"),
                                Estado = reader.GetBoolean("estado"),
                                IdTipo = reader.GetInt32("id_tipo"),
                                Tipo = new TipoInmueble { IdTipo = reader.GetInt32("id_tipo"), Nombre = reader.GetString("tipo_nombre") },
                                IdPropietario = reader.GetInt32("id_propietario"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("prop_nombre"),
                                    Apellido = reader.GetString("prop_apellido"),
                                    Email = reader.GetString("prop_email")
                                }
                            };
                        }
                    }
                }
            }
            return inmueble;
        }

        public int Guardar(Inmueble i)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO inmuebles (direccion, cupo, id_tipo, coordenadas, precio_dia, id_propietario) 
                            VALUES (@direccion, @cupo, @idTipo, @coordenadas, @precioDia, @idPropietario);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@cupo", i.Cupo);
                    command.Parameters.AddWithValue("@idTipo", i.IdTipo);
                    command.Parameters.AddWithValue("@coordenadas", (object?)i.Coordenadas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@precioDia", i.PrecioDia);
                    command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);

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

                var sql = @"SELECT i.id_inmueble, i.direccion, i.cupo, i.coordenadas, i.precio_dia,
                           t.nombre AS tipo_nombre,
                           p.nombre AS prop_nombre, p.apellido AS prop_apellido
                    FROM inmuebles i
                    INNER JOIN tipos_inmueble t ON i.id_tipo = t.id_tipo
                    INNER JOIN propietarios p ON i.id_propietario = p.id_propietario
                    WHERE i.estado = 1
                      AND i.id_inmueble NOT IN (
                          SELECT id_inmueble 
                          FROM reservas 
                          WHERE estado = 1 
                            AND (fecha_desde < @hasta AND fecha_hasta > @desde)
                      )";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                Cupo = reader.GetInt32("cupo"),
                                PrecioDia = reader.GetDecimal("precio_dia"),
                                Tipo = new TipoInmueble { Nombre = reader.GetString("tipo_nombre") },
                                Propietario = new Propietario
                                {
                                    Nombre = reader.GetString("prop_nombre"),
                                    Apellido = reader.GetString("prop_apellido")
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