using EventosApi.Data;
using EventosApi.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EventosApi.DTOs
{
    public class EventoDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayus]
        public string Nombre_Evento { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Capacidad_Maxima { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public float Costo { get; set; }
        public int EspaciosDisponibles { get; set; }

        public EventoDTO(Evento evento)
        {
            Nombre_Evento = evento.Nombre_Evento;
            Descripcion = evento.Descripcion;
            Fecha = evento.Fecha;
            Ubicacion = evento.Ubicacion;
            Capacidad_Maxima = evento.Capacidad_Maxima;
            Costo = evento.Costo;
            EspaciosDisponibles = evento.Capacidad_Maxima;
        }

    }
}
