using ManagementProduct.DTOs.Product;
using ManagementProduct.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ManagementProduct.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController: ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        => Ok(await _productService.GetAllAsync(query));


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Produk dengan ID {id} tidak ditemukan." });
            return Ok(product);
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting("write")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Validasi gagal.",
                    errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var username = User.FindFirstValue(ClaimTypes.Name)!;
            var product = await _productService.CreateAsync(dto, userId, username);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [EnableRateLimiting("write")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var username = User.FindFirstValue(ClaimTypes.Name)!;
            var (found, product) = await _productService.UpdateAsync(id, dto, userId, username);
            if (!found)
                return NotFound(new { message = $"Produk dengan ID {id} tidak ditemukan." });
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [EnableRateLimiting("write")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Produk dengan ID {id} tidak ditemukan." });
            return Ok(new { message = $"Produk dengan ID {id} berhasil dihapus." });
        }

    }

}
