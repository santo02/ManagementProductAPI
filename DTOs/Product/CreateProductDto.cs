using System.ComponentModel.DataAnnotations;

namespace ManagementProduct.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Title wajib diisi.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price wajib diisi.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price harus lebih dari 0.")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Category wajib diisi.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Images wajib diisi minimal 1.")]
        [MinLength(1, ErrorMessage = "Images minimal harus 1 URL.")]
        public List<string> Images { get; set; } = new();
    }
}
