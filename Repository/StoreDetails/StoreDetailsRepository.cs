﻿using Models.DBContext;
using Models;
using Repository.BaseRepository;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            {
                return false;
            }

            store.IsActive = isActive;
            store.ModifiedDate = DateTime.UtcNow;

            // Lấy danh sách sản phẩm của store
            var products = await _context.Products
                .Where(p => p.StoreID == storeId)
                .ToListAsync();

            // Lấy danh sách biến thể sản phẩm của store
            var productIds = products.Select(p => p.ID).ToList();
            var productVariants = await _context.ProductTypes
                .Where(v => productIds.Contains(v.ProductID))
                .ToListAsync();

            if (!isActive) // Nếu store bị khóa
            {
                foreach (var product in products)
                {
                    product.IsActive = false; // Tắt tất cả sản phẩm
                }

                foreach (var variant in productVariants)
                {
                    variant.IsActive = false; // Tắt tất cả biến thể sản phẩm
                }
            }
            else // Nếu store được mở khóa
            {
                foreach (var product in products)
                {
                    product.IsActive = true; // Bật tất cả sản phẩm
                }

                foreach (var variant in productVariants)
                {
                    variant.IsActive = true; // Bật tất cả biến thể sản phẩm
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
    }
}
