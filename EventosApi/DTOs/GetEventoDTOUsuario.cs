﻿using EventosApi.Data;
using EventosApi.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EventosApi.DTOs
{
    public class GetEventoDTOUsuario
    {
        public int EventoId { get; set; }
        public string Nombre_Evento { get; set; }

        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }

        public string Ubicacion { get; set; }
        public float Costo { get; set; }
        public int EspaciosDisponibles { get; set; }
        public List<OrganizadorDTOEnEvento> Organizadores { get; set; } = new List<OrganizadorDTOEnEvento>();

    }
}
