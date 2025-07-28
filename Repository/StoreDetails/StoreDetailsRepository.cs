using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Repository.BaseRepository;
using Repository.ViewModels;

namespace Repository.StoreDetails
{
    public class StoreDetailsRepository : BaseRepository<Models.StoreDetails>, IStoreDetailsRepository
    {
        private readonly FoodHavenDbContext _context;
        public StoreDetailsRepository(FoodHavenDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> IsUserSellerAsync(string userId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                                _context.Roles.Any(r => r.Id == ur.RoleId && r.Name.ToLower() == "seller"));
        }

        public async Task<bool> AddStoreAsync(Models.StoreDetails store)
        {
            try
            {
                await _context.StoreDetails.AddAsync(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<IEnumerable<Models.StoreDetails>> GetAllStoresAsync()
        {
            return await _context.StoreDetails.ToListAsync();
        }

        public async Task<Models.StoreDetails?> GetStoreByIdAsync(Guid storeId)
        {
            return await _context.StoreDetails.FirstOrDefaultAsync(s => s.ID == storeId);
        }
        public async Task<Models.StoreDetails> GetByIdAsync(Guid id)
        {
            return await _context.StoreDetails.FindAsync(id);
        }

        public async Task UpdateAsync(Models.StoreDetails storeDetails)
        {
            _context.StoreDetails.Update(storeDetails);
            await _context.SaveChangesAsync();
        }
        public async Task<List<StoreViewModel>> GetInactiveStoresAsync()
        {
            var stores = await _context.StoreDetails
                .Where(s => (s.Status ?? "").ToLower() != "pending") // chỉ lấy các store có Status là "Pending" (không phân biệt hoa thường)
                .Join(
                    _context.Users,
                    s => s.UserID,
                    u => u.Id,
                    (s, u) => new StoreViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        CreatedDate = s.CreatedDate,
                        ModifiedDate = s.ModifiedDate,
                        ShortDescriptions = s.ShortDescriptions ?? "No description available",
                        LongDescriptions = s.LongDescriptions ?? "No description available",
                        Address = s.Address,
                        Phone = s.Phone,
                        Img = !string.IsNullOrEmpty(s.ImageUrl) ? s.ImageUrl : "default-store.png",
                        IsActive = s.IsActive,
                        Status = s.Status ?? "Pending",
                        UserName = u.UserName,
                        UserID = s.UserID
                    }
                )
                .ToListAsync();

            return stores;
        }

        public async Task<List<StoreViewModel>> GetStoreRegistrationRequestsAsync()
        {
            var stores = await _context.StoreDetails
                .OrderBy(s => s.Status != "PENDING") // PENDING lên trước
                .ThenBy(s => s.Status != "REJECTED") // REJECTED tiếp theo
                .ThenBy(s => s.CreatedDate)          // Ngày tạo sớm nhất lên trước
                .Join(
                    _context.Users,
                    s => s.UserID,
                    u => u.Id,
                    (s, u) => new StoreViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        CreatedDate = s.CreatedDate,
                        ModifiedDate = s.ModifiedDate,
                        ShortDescriptions = s.ShortDescriptions ?? "No description available",
                        LongDescriptions = s.LongDescriptions ?? "No description available",
                        Address = s.Address,
                        Phone = s.Phone,
                        Img = !string.IsNullOrEmpty(s.ImageUrl) ? s.ImageUrl : "default-store.png",
                        IsActive = s.IsActive,
                        Status = s.Status ?? "PENDING",
                        UserName = u.UserName,
                        UserID = s.UserID
                    }
                )
                .ToListAsync();

            return stores;
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateStoreAsync(Models.StoreDetails store)
        {
            try
            {
                _context.StoreDetails.Update(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                Console.WriteLine($"Error updating store: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateStoreIsActiveAsync(Guid storeId, bool isActive)
        {
            var store = await _context.StoreDetails.FindAsync(storeId);
            if (store == null)
                return false;

            store.IsActive = isActive;
            store.ModifiedDate = DateTime.UtcNow;

            var products = await _context.Products
                .Where(p => p.StoreID == storeId)
                .ToListAsync();

            var productIds = products.Select(p => p.ID).ToList();

            var productVariants = await _context.ProductTypes
                .Where(v => productIds.Contains(v.ProductID))
                .ToListAsync();

            if (!isActive)
            {
                // 🔒 Khóa cửa hàng → vô hiệu hóa toàn bộ + cập nhật trạng thái banned
                foreach (var product in products)
                {
                    if (product.IsActive == true)
                    {
                        product.IsProductBanned = false;
                    }
                    else
                    {
                        product.IsProductBanned = true;
                    }

                    product.IsActive = false;
                }

                foreach (var variant in productVariants)
                {
                    if (variant.IsActive == true)
                    {
                        variant.IsProductTypeBanned = false;
                    }
                    else
                    {
                        variant.IsProductTypeBanned = true;
                    }

                    variant.IsActive = false;
                    variant.Stock = 0;
                }
            }
            else
            {
                // 🔓 Mở lại cửa hàng → khôi phục trạng thái nếu KHÔNG bị banned
                foreach (var product in products)
                {
                    if (product.IsProductBanned == false && product.IsActive == false)
                    {
                        product.IsActive = true;
                    }
                }

                foreach (var variant in productVariants)
                {
                    if (variant.IsProductTypeBanned == false && variant.IsActive == false)
                    {
                        variant.IsActive = true;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<List<Models.StoreDetails>> GetStoresAsync()
        {
            return await _context.StoreDetails.ToListAsync();
        }
        public async Task<bool> AcceptStoreAsync(Guid id)
        {
            var store = await _context.StoreDetails.FindAsync(id);
            if (store == null)
                return false;

            store.Status = "APPROVED";
            store.ModifiedDate = DateTime.UtcNow; // Cập nhật ngày sửa đổi

            _context.StoreDetails.Update(store);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> RejectStoreAsync(Guid id)
        {
            var store = await _context.StoreDetails.FindAsync(id);
            if (store == null)
                return false;

            store.Status = "REJECTED";
            store.ModifiedDate = DateTime.UtcNow; // Cập nhật ngày sửa đổi

            _context.StoreDetails.Update(store);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<StoreViewModel>> GetStoresByUserIdAsync(string? userId)
        {
            return await _context.StoreDetails
                .Where(s => s.UserID == userId)
                .Select(s => new StoreViewModel
                {
                    ID = s.ID,
                    UserID = s.UserID,
                    Name = s.Name,
                    CreatedDate = s.CreatedDate.Date,
                    ModifiedDate = s.ModifiedDate, // Kiểm tra null trước khi lấy ngày
                    LongDescriptions = s.LongDescriptions,
                    ShortDescriptions = s.ShortDescriptions,
                    Address = s.Address,
                    Phone = s.Phone,
                    Img = s.ImageUrl,
                    Status = s.Status,
                    IsActive = s.IsActive
                }).ToListAsync();
        }
        public async Task<Models.StoreDetails> GetByUserId(string userId)
        {
            return await _context.StoreDetails.FirstOrDefaultAsync(s => s.UserID == userId)
                   ?? throw new Exception("Store not found");
        }
        public async Task<bool> IsStoreActiveAsync(Guid storeId)
        {
            var store = await _context.StoreDetails.FindAsync(storeId);
            return store?.IsActive ?? false;
        }
        public Models.StoreDetails GetStoreByUserId(string userId)
        {
            return _context.StoreDetails.FirstOrDefault(s => s.UserID == userId);
        }
        public async Task<bool> IsStoreActiveByUserIdAsync(string userId)
        {
            var store = await GetStoreByUserIdAsync(userId);
            return store?.IsActive ?? false;
        }
        public async Task<Models.StoreDetails> GetStoreByUserIdAsync(string userId)
        {
            return await _context.StoreDetails.FirstOrDefaultAsync(s => s.UserID == userId);
        }
        public async Task<ViewStoreDetailViewModel> GetStoreDetailWithOwnerAsync(Guid storeId)
        {
            var result = await (from s in _context.StoreDetails
                                join u in _context.Users on s.UserID equals u.Id
                                where s.ID == storeId
                                select new ViewStoreDetailViewModel
                                {
                                    ID = s.ID,
                                    StoreName = s.Name,
                                    StoreOwner = u.FirstName + " " + u.LastName,
                                    CreatedDate = s.CreatedDate,
                                    ModifiedDate = s.ModifiedDate,
                                    ShortDescriptions = s.ShortDescriptions,
                                    LongDescriptions = s.LongDescriptions,
                                    Address = s.Address,
                                    Phone = s.Phone,
                                    ImageUrl = s.ImageUrl,
                                    Status = s.Status,
                                    IsActive = s.IsActive,
                                    RejectNote = s.RejectNote
                                }).FirstOrDefaultAsync();

            return result!;
        }
    }
}
