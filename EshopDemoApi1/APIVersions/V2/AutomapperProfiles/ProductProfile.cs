using AutoMapper;
using CatalogDemo.API.Model;
using CatalogDemo.API.Versions.V2.Models;

namespace CatalogDemo.API.Versions.V2.AutomapperProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>();
        }        
    }
}
