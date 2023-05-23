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
            CreateMap<Evento, EventoDTO>().ReverseMap();

        }
        
    }
}
