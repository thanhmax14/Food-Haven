using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.Categorys;
using Repository.ProductImage;
using Repository.Products;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductsRepository _repository;
        private readonly IMapper _mapper;
        private readonly CategoryRepository _categoryRepository;
        private readonly ProductImageRepository _productImageRepository;
        private readonly ProductsRepository _repositorys;
        private readonly IWebHostEnvironment _env;
        private readonly FoodHavenDbContext _context;
        public ProductService(IProductsRepository repository, FoodHavenDbContext context, IMapper mapper, CategoryRepository categoryRepository, ProductImageRepository productImageRepository, ProductsRepository repositorys, IWebHostEnvironment env)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _repositorys = repositorys;
            _env = env;
        }

        public IQueryable<Product> GetAll() => _repository.GetAll();

        public Product GetById(Guid id) => _repository.GetById(id);

        public async Task<Product> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Product Find(Expression<Func<Product, bool>> match) => _repository.Find(match);

        public async Task<Product> FindAsync(Expression<Func<Product, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Product entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Product entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Product entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Product>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Product>> ListAsync(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<bool> CreateProductAsync(ProductViewModel model, string userId, List<ProductImageViewModel> images)
        {
            var storeId = await GetCurrentStoreIDAsync(userId);
            if (storeId == null)
                return false;

            var product = new Product
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                CreatedDate = DateTime.Now,
                ModifiedDate = null,
                ManufactureDate = model.ManufactureDate,
                IsActive = model.IsActive,
                IsOnSale = model.IsOnSale,
                CategoryID = model.CateID,
                StoreID = storeId
            };

            var addProductResult = await _repositorys.AddAsync(product);
            if (!addProductResult)
                return false;

            if (images != null && images.Any())
            {
                var productImages = images.Select(img => new ProductImage
                {
                    ID = Guid.NewGuid(),
                    ProductID = product.ID,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList();

                var addImagesResult = await _productImageRepository.AddRangeAsync(productImages);
                return addImagesResult;
            }

            return true;
        }

        public Task<Guid> GetCurrentStoreIDAsync(string userId)
        {
            return _repositorys.GetCurrentStoreIDAsync(userId);
        }

        public async Task<List<ProductListViewModel>> GetAllProductsAsync(Guid storeId)
        {
            return await _repositorys.GetProductsWithDetailsByStoreIdAsync(storeId);
        }
        public List<ProductIndexViewModel> GetProductsByStoreId(Guid storeId)
        {
            return _repositorys.GetProductsByStoreId(storeId);
        }
        public async Task<Guid> CreateProductAsync(ProductViewModel model)
        {
            var product = new Product
            {
                ID = Guid.NewGuid(),
                Name = model.Name.Trim(),
                ShortDescription = model.ShortDescription.Trim(),
                LongDescription = model.LongDescription.Trim(),
                CreatedDate = DateTime.Now,
                ManufactureDate = model.ManufactureDate,
                IsActive = model.IsActive,
                IsOnSale = model.IsOnSale,
                CategoryID = model.CateID,
                StoreID = model.StoreID
            };

            List<ProductImage> images = new List<ProductImage>();
            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Xử lý ảnh chính
            if (model.MainImage != null && model.MainImage.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.MainImage.FileName)}";
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MainImage.CopyToAsync(stream);
                }

                images.Add(new ProductImage
                {
                    ID = Guid.NewGuid(),
                    ImageUrl = $"/uploads/{fileName}",
                    IsMain = true,
                    ProductID = product.ID
                });
            }

            // Xử lý gallery
            if (model.GalleryImages != null && model.GalleryImages.Count > 0)
            {
                foreach (var img in model.GalleryImages)
                {
                    if (img != null && img.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        images.Add(new ProductImage
                        {
                            ID = Guid.NewGuid(),
                            ImageUrl = $"/uploads/{fileName}",
                            IsMain = false,
                            ProductID = product.ID
                        });
                    }
                }
            }

            return await _repositorys.CreateProductAsync(product, images);
        }


        public async Task<List<Categories>> GetCategoriesAsync()
        {
            return await _repositorys.GetCategoriesAsync();
        }

        public async Task<ProductUpdateViewModel> GetProductByIdAsync(Guid productId)
        {
            return await _repositorys.GetProductByIdAsync(productId);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(ProductUpdateViewModel model, string webRootPath)
        {
            return await _repositorys.UpdateProductAsync(model, webRootPath);
        }

        public async Task<bool> ToggleProductStatus(Guid productId)
        {
            return await _repositorys.ToggleProductStatus(productId);
        }

        public List<ProductIndexViewModel> GetProductsByCurrentUser(string userId)
        {
            return _repositorys.GetProductsByCurrentUser(userId);
        }
        public async Task<bool?> IsStoreActiveByProductIdAsync(Guid productId)
        {
            return await _repositorys.IsStoreActiveByProductIdAsync(productId);
        }

        public Task<List<string>> GetImageUrlsByProductIdAsync(Guid productId)
        {
            return _repositorys.GetImageUrlsByProductIdAsync(productId);
        }
        public async Task<List<Categories>> GetActiveCategoriesAsync()
        {
            return await _repositorys.GetActiveCategoriesAsync();
        }

        public async Task<Product> FindWithStoreAndUserAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.StoreDetails)
                .ThenInclude(sd => sd.AppUser)
                .FirstOrDefaultAsync(p => p.ID == id);
        }
        public async Task<List<string>> GetGalleryImageUrlsByProductIdAsync(Guid productId)
        {
            return await _repositorys.GetGalleryImageUrlsByProductIdAsync(productId);
        }
        public async Task<string> GetMainImageUrlsByProductIdAsync(Guid productId)
        {
            return await _repositorys.GetMainImageUrlByProductIdAsync(productId);
        }
        public async Task<bool> IsProductNameTakenAsync(string name, Guid currentProductId, Guid storeId)
        {
            var products = _repositorys.GetAll(); // IQueryable

            return products.Any(p =>
                p.StoreID == storeId && // kiểm tra cùng store
                p.ID != currentProductId && // không phải chính product đang edit
                p.Name.Trim().ToLower() == name.Trim().ToLower()); // tên trùng
        }

        public async Task<bool> IsDuplicateProductNameAsync(string name, Guid storeId)
        {
            return await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.StoreID == storeId);
        }

        public async Task<bool> IsTypeNameTakenAsync(Guid productId, string size)
        {
            return await _repositorys.IsTypeNameTakenAsync(productId, size);
        }

    }
}
