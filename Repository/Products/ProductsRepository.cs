using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using NuGet.Protocol.Core.Types;
using Repository.BaseRepository;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repository.Products
{
    public class ProductsRepository : BaseRepository<Models.Product>, IProductsRepository
    {
        private readonly FoodHavenDbContext _context;
        public ProductsRepository(FoodHavenDbContext context) : base(context) {
            _context = context;
        }
        
        public async Task<bool> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Guid> GetCurrentStoreIDAsync(string userId)
        {
            var store = await _context.StoreDetails.FirstOrDefaultAsync(s => s.UserID == userId);
            return store.ID;
        }

        public async Task<List<ProductListViewModel>> GetProductsWithDetailsByStoreIdAsync(Guid storeId)
        {
            // Bước 1: Lấy danh sách sản phẩm (không có ảnh)
            var productEntities = await (from p in _context.Products
                                         join c in _context.Categories on p.CategoryID equals c.ID
                                         join s in _context.StoreDetails on p.StoreID equals s.ID
                                         where p.StoreID == storeId
                                         select new ProductListViewModel
                                         {
                                             ID = p.ID,
                                             Name = p.Name,
                                             ShortDescription = p.ShortDescription,
                                             LongDescription = p.LongDescription,
                                             CreatedDate = p.CreatedDate,
                                             ModifiedDate = p.ModifiedDate,
                                             ManufactureDate = p.ManufactureDate,
                                             IsActive = p.IsActive,
                                             IsOnSale = p.IsOnSale,
                                             CategoryName = c.Name,
                                             StoreName = s.Name,
                                             StoreId = p.StoreID,
                                             CateID = p.CategoryID,
                                             Images = new List<ProductImageViewModel>() // Tạo danh sách rỗng ban đầu
                                         }).ToListAsync();

            // Bước 2: Lấy danh sách ảnh của tất cả sản phẩm
            var productIds = productEntities.Select(p => p.ID).ToList();
            var images = await _context.ProductImages
                .Where(img => productIds.Contains(img.ProductID))
                .OrderByDescending(img => img.IsMain)
                .ThenByDescending(img => img.ID)
                .ToListAsync();

            // Bước 3: Nhóm ảnh theo từng sản phẩm
            var imageDict = images.GroupBy(img => img.ProductID)
                                  .ToDictionary(g => g.Key, g => g
                                      .Take(5)
                                      .Select(img => new ProductImageViewModel
                                      {
                                          ImageUrl = img.ImageUrl,
                                          IsMain = img.IsMain
                                      })
                                      .ToList());

            // Bước 4: Gán danh sách ảnh vào sản phẩm tương ứng
            foreach (var product in productEntities)
            {
                if (imageDict.TryGetValue(product.ID, out var imgList))
                {
                    product.Images = imgList;
                }
            }

            return productEntities;
        }

        public List<ProductIndexViewModel> GetProductsByStoreId(Guid storeId)
        {
            var products = (from p in _context.Products
                            join c in _context.Categories on p.CategoryID equals c.ID
                            join s in _context.StoreDetails on p.StoreID equals s.ID
                            join i in _context.ProductImages.Where(img => img.IsMain)
                                on p.ID equals i.ProductID into imgGroup
                            from img in imgGroup.DefaultIfEmpty() // Lấy ảnh IsMain nếu có
                            where p.StoreID == storeId
                            select new ProductIndexViewModel
                            {
                                ProductId = p.ID,
                                Name = p.Name,
                                ShortDescription = p.ShortDescription,
                                LongDescription = p.LongDescription,
                                CreatedDate = p.CreatedDate,
                                ModifiedDate = p.ModifiedDate,
                                ManufactureDate = p.ManufactureDate,
                                IsActive = p.IsActive,
                                IsOnSale = p.IsOnSale,
                                CateName = c.Name,
                                StoreName = s.Name,
                                ImageUrl = img != null ? img.ImageUrl : "/images/default.png"
                            }).ToList();

            return products;
        }
        public async Task<Guid> CreateProductAsync(Product product, List<Models.ProductImage> images)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (images.Any())
            {
                foreach (var image in images)
                {
                    image.ProductID = product.ID; // Gán ProductID
                    await _context.ProductImages.AddAsync(image);
                }
                await _context.SaveChangesAsync();
            }

            return product.ID;
        }
        public async Task<List<Categories>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync(); // lấy từ DB trước
            return categories
                .OrderBy(c => int.TryParse(c.DisplayOrder, out var order) ? order : int.MaxValue)
                .ToList();
        }
        public async Task<ProductUpdateViewModel> GetProductByIdAsync(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null) return null;

            return new ProductUpdateViewModel
            {
                ProductID = product.ID,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                LongDescription = product.LongDescription,
                ManufactureDate = product.ManufactureDate,
                ModifiedDate = product.ModifiedDate,
                IsActive = product.IsActive,
                IsOnSale = product.IsOnSale,
                CateID = product.CategoryID,
                StoreID = product.StoreID,
                ExistingImages = product.ProductImages.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task UpdateProductAsync(ProductUpdateViewModel model, string webRootPath)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ID == model.ProductID);

            if (product == null) return;

            product.Name = model.Name;
            product.ShortDescription = model.ShortDescription;
            product.LongDescription = model.LongDescription;
            product.ManufactureDate = model.ManufactureDate;
            product.ModifiedDate = DateTime.UtcNow;
            product.IsOnSale = model.IsOnSale;
            product.CategoryID = model.CateID;

            // XÓA ảnh được chọn
            if (model.RemoveImageUrls?.Any() == true)
            {
                var imagesToRemove = product.ProductImages
                    .Where(i => model.RemoveImageUrls.Contains(i.ImageUrl))
                    .ToList();

                foreach (var img in imagesToRemove)
                {
                    var path = Path.Combine(webRootPath, "uploads", Path.GetFileName(img.ImageUrl));
                    if (File.Exists(path)) File.Delete(path);
                }

                _context.ProductImages.RemoveRange(imagesToRemove);
                await _context.SaveChangesAsync();
            }

            // XỬ LÝ ẢNH CHÍNH
            if (model.MainImage != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(model.MainImage.FileName);
                string filePath = Path.Combine(webRootPath, "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MainImage.CopyToAsync(stream);
                }

                // Xóa ảnh chính cũ
                var oldMain = product.ProductImages.FirstOrDefault(i => i.IsMain);
                if (oldMain != null)
                {
                    var oldPath = Path.Combine(webRootPath, "uploads", Path.GetFileName(oldMain.ImageUrl));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                    _context.ProductImages.Remove(oldMain);
                }

                _context.ProductImages.Add(new Models.ProductImage
                {
                    ProductID = product.ID,
                    ImageUrl = "/uploads/" + fileName,
                    IsMain = true
                });
            }
            else if (!string.IsNullOrEmpty(model.ExistingMainImage))
            {
                bool exists = product.ProductImages.Any(p => p.ImageUrl == model.ExistingMainImage && p.IsMain);
                if (!exists)
                {
                    _context.ProductImages.Add(new Models.ProductImage
                    {
                        ProductID = product.ID,
                        ImageUrl = model.ExistingMainImage,
                        IsMain = true
                    });
                }
            }

            // XỬ LÝ ẢNH PHỤ
            var gallerySlots = 4 - product.ProductImages.Count(i => !i.IsMain);
            if (model.GalleryImages?.Any() == true)
            {
                foreach (var file in model.GalleryImages.Take(gallerySlots))
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(webRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.ProductImages.Add(new Models.ProductImage
                    {
                        ProductID = product.ID,
                        ImageUrl = "/uploads/" + fileName,
                        IsMain = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ProductHideShowViewModel> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductHideShowViewModel
            {
                ID = product.ID,
                Name = product.Name,
                IsActive = product.IsActive
            };
        }

        public async Task<bool> UpdateStatusAsync(Guid productId, bool isActive)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsActive = isActive; // Cập nhật trạng thái
            _context.Products.Update(product);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào DB

            return true;
        }
        public List<ProductIndexViewModel> GetProductsByCurrentUser(string userId)
        {
            var store = _context.StoreDetails.FirstOrDefault(s => s.UserID == userId);
            if (store == null)
            {
                return new List<ProductIndexViewModel>(); // Trả về danh sách rỗng nếu user không có store
            }

            var products = (from p in _context.Products
                            join c in _context.Categories on p.CategoryID equals c.ID
                            join i in _context.ProductImages.Where(img => img.IsMain)
                                on p.ID equals i.ProductID into imgGroup
                            from img in imgGroup.DefaultIfEmpty()
                            where p.StoreID == store.ID
                            select new ProductIndexViewModel
                            {
                                ProductId = p.ID,
                                Name = p.Name,
                                ShortDescription = p.ShortDescription,
                                LongDescription = p.LongDescription,
                                CreatedDate = p.CreatedDate,
                                ModifiedDate = p.ModifiedDate,
                                ManufactureDate = p.ManufactureDate,
                                IsActive = p.IsActive,
                                IsOnSale = p.IsOnSale,
                                CateName = c.Name,
                                StoreName = store.Name,
                                StoreId = store.ID, // Gán StoreId
                                StoreIsActive = store.IsActive,
                                ImageUrl = img != null ? img.ImageUrl : "/images/default.png"
                            }).ToList();

            return products;
        }
        public async Task<bool?> IsStoreActiveByProductIdAsync(Guid productId)
        {
            return await (from p in _context.Products
                          join s in _context.StoreDetails on p.StoreID equals s.ID
                          where p.ID == productId
                          select s.IsActive).FirstOrDefaultAsync();
        }
        public async Task<List<string>> GetImageUrlsByProductIdAsync(Guid productId)
        {
            return await _context.ProductImages
                .Where(p => p.ProductID == productId)
                .OrderByDescending(p => p.IsMain)
                .Select(p => p.ImageUrl)
                .ToListAsync();
        }
        public async Task<List<Categories>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
        }
        public async Task<bool> ToggleProductStatus(Guid productId)
        {
            var product = await GetByIdAsync(productId);
            if (product == null) return false;

            bool newStatus = !product.IsActive; // Đảo trạng thái Hide/Show

            // Nếu đang tắt sản phẩm => reset stock về 0 cho toàn bộ biến thể
            if (!newStatus)
            {
                var variants = await _context.ProductTypes
                    .Where(v => v.ProductID == productId)
                    .ToListAsync();

                foreach (var variant in variants)
                {
                    variant.Stock = 0;
                    variant.IsActive = false; // cũng tắt biến thể nếu muốn đồng bộ
                }

                await _context.SaveChangesAsync();
            }

            return await UpdateStatusAsync(productId, newStatus);
        }
    }
}
