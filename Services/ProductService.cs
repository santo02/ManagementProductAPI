using ManagementProduct.Data;
using ManagementProduct.DTOs.Product;
using ManagementProduct.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ManagementProduct.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;

        public ProductService(AppDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }
        private static ProductDto ToDto(Product p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Price = p.Price,
            Description = p.Description,
            Category = p.Category,
            Images = JsonSerializer.Deserialize<List<string>>(p.ImagesJson) ?? new(),
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            CreatedById = p.CreatedById,
            UpdatedAt = p.UpdatedAt,
            UpdatedBy = p.UpdatedBy,
            UpdatedById = p.UpdatedById
        };

        public async Task<object> GetAllAsync(ProductQueryDto query)
        {
            var cacheKey = $"products_{query.Search}_{query.Category}_{query.Page}_{query.Limit}";
            if (_cache.TryGetValue(cacheKey, out object? cached)) return cached!;

            var q = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(p => p.Title.Contains(query.Search));

            if (!string.IsNullOrWhiteSpace(query.Category))
                q = q.Where(p => p.Category == query.Category);

            var total = await q.CountAsync();
            var items = await q
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            var result = new
            {
                total,
                page = query.Page,
                limit = query.Limit,
                totalPages = (int)Math.Ceiling((double)total / query.Limit),
                data = items.Select(ToDto)
            };

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
            return result;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"product_{id}";
            if (_cache.TryGetValue(cacheKey, out ProductDto? cached)) return cached;

            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;

            var dto = ToDto(product);
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, string userId, string username)
        {
            var product = new Product
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description,
                Category = dto.Category,
                ImagesJson = JsonSerializer.Serialize(dto.Images),
                CreatedBy = username,
                CreatedById = userId,
                UpdatedBy = username,
                UpdatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return ToDto(product);
        }

        public async Task<(bool found, ProductDto? dto)> UpdateAsync(
        int id, UpdateProductDto dto, string userId, string username)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return (false, null);

            if (dto.Title != null) product.Title = dto.Title;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Category != null) product.Category = dto.Category;
            if (dto.Images != null) product.ImagesJson = JsonSerializer.Serialize(dto.Images);

            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = username;
            product.UpdatedById = userId;

            await _db.SaveChangesAsync();
            _cache.Remove($"product_{id}");
            return (true, ToDto(product));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            _cache.Remove($"product_{id}");
            return true;
        }
    }

}
