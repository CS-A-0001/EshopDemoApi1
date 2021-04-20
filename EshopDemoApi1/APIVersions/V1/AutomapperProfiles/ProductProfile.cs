using AutoMapper;
using CatalogDemo.API.Model;
using CatalogDemo.API.Versions.V1.Models;

namespace CatalogDemo.API.Versions.V1.AutomapperProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>();
        }        
    }
}
