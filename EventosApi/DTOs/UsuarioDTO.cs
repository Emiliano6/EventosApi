﻿using EventosApi.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EventosApi.DTOs
{
    public class UsuarioDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayus]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress]
        public string Correo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Phone]
        public string Telefono { get; set; }

    }
}
