using AutoMapper;

using Core.Domain.Entities;
using API.Models.DTOs.Responses;
using API.Models.DTOs.Requests;

namespace API.Models.Mappings
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
