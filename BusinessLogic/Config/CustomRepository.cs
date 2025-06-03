
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.Reviews;
using Microsoft.Extensions.DependencyInjection;
using Repository.BalanceChange;
using Repository.Carts;
using Repository.ProductImage;
using Repository.Products;
using Repository.ProductVariants;
using Repository.Reviews;
using Repository.StoreDetails;
using Microsoft.Extensions.DependencyInjection;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Wishlists;
using Repository.OrdersRepository;
using Repository.OrdeDetails;
using Repository.Vouchers;
using Repository.Complaints;
using Repository.ComplaintImages;
using BusinessLogic.Services.RecipeServices;
using Repository.RecipeRepository;
using Repository.IngredientTagRepositorys;
using BusinessLogic.Services.IngredientTagServices;
using Repository.TypeOfDishRepositoties;
using BusinessLogic.Services.TypeOfDishServices;

namespace BusinessLogic.Config
{
    public static class CustomRepository
    {
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<IStoreDetailsRepository, StoreDetailsRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IBalanceChangeRepository, BalanceChangeRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<Repository.StoreDetails.StoreDetailsRepository>();
            services.AddScoped<Repository.Categorys.CategoryRepository>();
            services.AddScoped<Repository.Products.ProductsRepository>();
            services.AddScoped<Repository.ProductImage.ProductImageRepository>();
            services.AddScoped<Repository.ProductVariants.ProductVariantRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<Repository.OrdeDetails.OrderDetailRepository>();
            services.AddScoped<Repository.OrdersRepository.OrdersRepository>();
            services.AddScoped<Repository.BalanceChange.BalanceChangeRepository>();
            services.AddScoped<IVouchersRepository, VouchersRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IComplaintImageRepository, ComplaintImageRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IIngredientTagRepository, IngredientTagRepository>();
            services.AddScoped<IIngredientTagService, IngredientTagService>();
            services.AddScoped<ITypeOfDishRepository, TypeOfDishRepository>();
            services.AddScoped<ITypeOfDishService, TypeOfDishService>();
        }



    }
}
