namespace InmobiliariaCC2.Models
{
    public class InformeInmueble
    {
        public int IdInmueble { get; set; }
        public string Direccion { get; set; } = "";
        public decimal PrecioDia { get; set; }
        public bool Estado { get; set; }

        public int IdPropietario { get; set; }
        public string Propietario { get; set; } = "";

        public int CantidadReservas { get; set; }

        public DateTime? UltimaReserva { get; set; }
    }

    public class InformeReserva
    {
        public int IdReserva { get; set; }

        public string Inquilino { get; set; } = "";
        public string Inmueble { get; set; } = "";

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public decimal MontoDia { get; set; }

        public bool Estado { get; set; }

        public int DiasRestantes { get; set; }
    }
}