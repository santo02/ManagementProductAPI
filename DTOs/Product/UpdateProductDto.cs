namespace ManagementProduct.DTOs.Product
{
    public class UpdateProductDto
    {
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public List<string>? Images { get; set; }
    }
}
