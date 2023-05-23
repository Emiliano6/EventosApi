using EventosApi.Data;
using EventosApi.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using EventosApi.DTOs;

namespace EventosApi.DTOs
{
    public class GetOrganizadorDTO
    {
        public int OrganizadorId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public List<EventoDTOEnOrganizador> Eventos { get; set; } = new List<EventoDTOEnOrganizador>();
    }
}