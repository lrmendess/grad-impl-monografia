using AutoMapper;
using SCAP.Models;
using SCAP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Afastamento, AfastamentoViewModel>().ReverseMap();
            CreateMap<Documento, DocumentoViewModel>().ReverseMap();
            CreateMap<Mandato, MandatoViewModel>().ReverseMap();
            CreateMap<Parecer, ParecerViewModel>().ReverseMap();
            CreateMap<Parentesco, ParentescoViewModel>().ReverseMap();
            CreateMap<Mandato, MandatoViewModel>().ReverseMap();

            // O Automapper sobrescreve o Id quando instanciado, portanto devemos ignorá-lo
            CreateMap<Professor, UserViewModel>()
                .ForMember(m => m.UserType, opt => opt.MapFrom(s => 1))
                .ReverseMap()
                .ForMember(m => m.Id, opt => opt.Ignore());

            CreateMap<Secretario, UserViewModel>()
                .ForMember(m => m.UserType, opt => opt.MapFrom(s => 2))
                .ReverseMap()
                .ForMember(m => m.Id, opt => opt.Ignore());
        }
    }
}
