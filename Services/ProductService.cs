using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IRepository<Product> productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogDebug("Getting all products");
            var products = await _productRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {ProductCount} products", products.Count());
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            var product = await _productRepository.FirstOrDefaultAsync(p => p.SKU == sku);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.FindAsync(p => p.CategoryId == categoryId && p.IsActive);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.FindAsync(p =>
                p.IsActive &&
                (p.Name.Contains(searchTerm) ||
                 p.Description!.Contains(searchTerm) ||
                 p.SKU.Contains(searchTerm)));
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            _logger.LogDebug("Creating new product with SKU: {SKU}", createProductDto.SKU);

            var product = _mapper.Map<Product>(createProductDto);
            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created product {ProductId} with SKU: {SKU}", product.Id, product.SKU);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            _logger.LogDebug("Updating product {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for update", id);
                return null;
            }

            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated product {ProductId}", id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStockAsync(int productId, int newStock)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            product.StockQuantity = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            var products = await _productRepository.FindAsync(p =>
                p.IsActive && p.StockQuantity <= p.MinStockLevel);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync()
        {
            var products = await _productRepository.FindAsync(p => p.IsActive && p.IsFeatured);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
