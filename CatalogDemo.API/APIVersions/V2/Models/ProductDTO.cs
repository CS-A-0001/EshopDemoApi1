namespace CatalogDemo.API.Versions.V2.Models
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUri { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
