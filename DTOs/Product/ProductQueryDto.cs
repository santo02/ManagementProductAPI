namespace ManagementProduct.DTOs.Product
{
    public class ProductQueryDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
