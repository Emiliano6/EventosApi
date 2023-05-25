using EventosApi.Data;
using EventosApi.DTOs;
using AutoMapper;
using System;

namespace EventosApi.Mapper
{
    public class MapperCode : Profile    
    {
        public MapperCode() 
        {
            CreateMap<EventoDTO, Evento>();
            CreateMap<Evento, GetEventoDTOUsuario>().ForMember(orga =>orga.Organizadores, opt => opt.MapFrom(src => src.Organizadores));
            CreateMap<OrganizadorDTO, Organizador>();
            CreateMap<Organizador, GetOrganizadorDTO>().ForMember(e =>e.Eventos, opt => opt.MapFrom(src => src.Eventos));
            CreateMap<Organizador, OrganizadorDTOEnEvento>();
            CreateMap<Evento, EventoDTOEnOrganizador>();
            CreateMap<PromocionDTO, Promocion>();
            CreateMap<Promocion, GetPromocionDTO>();
            CreateMap<Usuario, GetUsuarioDTO>();
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Evento, EventoDTOEnHistorial>();
        }
        
    }
}
