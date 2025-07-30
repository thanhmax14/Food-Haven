using AutoMapper;
using BusinessLogic.Hash;
using BusinessLogic.Mapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.FavoriteFavoriteRecipes;
using BusinessLogic.Services.Message;
using BusinessLogic.Services.MessageImages;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.RecipeReviewReviews;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using Repository.BalanceChange;
using Repository.MessageImages;
using Repository.Messages;
using Repository.OrdersRepository;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLogic.EmailServices.EmailService;

namespace BusinessLogic.Config
{
    public static class CustomServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IStoreDetailService, StoreDetailService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBalanceChangeService, BalanceChangeService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWishlistServices, WishlistServices>();
            services.AddScoped<ManageTransaction>();
            services.AddScoped<IOrdersServices, OrderServices>();
            services.AddHttpContextAccessor();
            services.AddScoped<BusinessLogic.Services.Categorys.CategoryService>();
            services.AddScoped<BusinessLogic.Services.Products.ProductService>();
            services.AddScoped<IOrderDetailService, OrderDetailServices>();
            services.AddScoped<IVoucherServices, VoucherService>();
            services.AddScoped<IComplaintImageServices, ComplaintImageServices>();
            services.AddScoped<IComplaintServices, ComplaintServices>();
            services.AddScoped<IMessageImageService, MessageImageService>();
            services.AddScoped<IMessageService, MessageServices>();
            services.AddScoped<IRecipeReviewService, RecipeReviewService>();
            services.AddScoped<IStoreReportServices, StoreReportServices>();
            services.AddScoped<IFavoriteRecipeService, FavoriteRecipeService>();
            services.AddScoped<IStoreFollowersService, StoreFollowersService>();
            services.AddScoped<IExpertRecipeServices, ExpertRecipeServices>();
            services.AddScoped<IRecipeViewHistoryServices, RecipeViewHistoryServices>();



            var mailSettings = new MailSettings
            {
                Mail = EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("NYi04pWx3H/MLeHzkOqQ78vAlxuJaJHo", "Supersic")),
                DisplayName = "ShopMMO",
                Password = EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("urfqXaNYsrfCDm5ip3Gqr4MNdUzxIBve", "Supersic")),
                Host = "smtp.gmail.com",
                Port = 587
            };
            services.Configure<MailSettings>(options =>
            {
                options.Mail = mailSettings.Mail;
                options.DisplayName = mailSettings.DisplayName;
                options.Password = mailSettings.Password;
                options.Host = mailSettings.Host;
                options.Port = mailSettings.Port;
            });
            services.AddTransient<IEmailSender, SendMailService>();


            PayOS payOS = new PayOS(EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("Lg/z97vcCzxz82hxxmXUPgfr8DkvUbFKwBTpsfmyTe8LvvjZONb4LA==", "Supersic")) ?? throw new Exception("Cannot find environment"),
                 EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("Vh59AG+ltkNthRB245UzJDz3ecAEYVQY06N3Qjjs92qfJj8IQq2+pg==", "Supersic")) ?? throw new Exception("Cannot find environment"),
                EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("LIEz2fnXXmwwZIcZLrNPuevmxV1LCvZvKTznQHNHrkpCphxT2ys1JtRXNU0owvMei0dgN899rvph291QHOw6YsgZQTNkEu/E", "Supersic")) ?? throw new Exception("Cannot find environment"));
            services.AddSingleton(payOS);
            services.AddAutoMapper(typeof(MappingProfile));


        }
    }
}
