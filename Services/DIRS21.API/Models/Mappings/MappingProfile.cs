using AutoMapper;

using Core.Domain.Entities;
using DIRS21.API.Models.DTOs.Responses;
using DIRS21.API.Models.DTOs.Requests;

namespace DIRS21.API.Models.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            CreateMap<Client, MinifiedClientDto>()
            .ForMember(destinationMember => destinationMember.ApiKey, options => options.MapFrom(src => src.Id));
            CreateMap<Client, ClientDto>();
            CreateMap<Product, ProductDto>();

        }

    }
   
}
