using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using Net.payOS;
using NUnit.Framework;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Food_Haven_TestUnit.UnitTestFollowedStore
{
    public class ManagementFollowedStores_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<IStoreFollowersService> _storeFollowersMock;
        private HomeController _controller;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;
        private Mock<IHubContext<ChatHub>> hubContextMock;
        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
            hubContextMock = new Mock<IHubContext<ChatHub>>();

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var orderDetailMock = new Mock<IOrderDetailService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            var categoryServiceMock = new Mock<ICategoryService>();
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            var emailSenderMock = new Mock<IEmailSender>();
            var cartMock = new Mock<ICartService>();
            var wishlistMock = new Mock<IWishlistServices>();
            var productServiceMock = new Mock<IProductService>();
            var productImgServiceMock = new Mock<IProductImageService>();
            var productVariantMock = new Mock<IProductVariantService>();
            var reviewServiceMock = new Mock<IReviewService>();
            var balanceChangeMock = new Mock<IBalanceChangeService>();
            var ordersServiceMock = new Mock<IOrdersServices>();
            var payOSMock = payOS;
            var voucherMock = new Mock<IVoucherServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            _storeFollowersMock = new Mock<IStoreFollowersService>();
            var recipeSearchMock = new RecipeSearchService("");

            _controller = new HomeController(
                _signInManagerMock.Object,
                orderDetailMock.Object,
                recipeServiceMock.Object,
                _userManagerMock.Object,
                categoryServiceMock.Object,
                storeDetailServiceMock.Object,
                emailSenderMock.Object,
                cartMock.Object,
                wishlistMock.Object,
                productServiceMock.Object,
                productImgServiceMock.Object,
                productVariantMock.Object,
                reviewServiceMock.Object,
                balanceChangeMock.Object,
                ordersServiceMock.Object,
                payOSMock,
                voucherMock.Object,
                storeReportMock.Object,
                _storeFollowersMock.Object,
                recipeSearchMock,
                _expertRecipeServicesMock.Object, // <-- Add this argument
    _recipeViewHistoryServicesMock.Object,
                    hubContextMock.Object

            // <-- Add this argument
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task ManagementFollowedStores_KetNoiThanhCongDanhSachKhongRong()
        {
            // Arrange
            var userId = ("8e91c798-b5e9-4649-994-5d0aaea77f5f");
            var user = new AppUser { Id = userId };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Mock StoreFollowersService
            var storeId = Guid.Parse("0521BE69-FC8D-4C9D-80DC-8D4DEA3B814C");
            var storeFollower = new StoreFollower { UserID = userId, StoreID = storeId };
            var followedStores = new List<StoreFollower> { storeFollower };
            _storeFollowersMock
                .Setup(s => s.ListAsync(
                    It.Is<Expression<Func<StoreFollower, bool>>>(expr => expr.Compile()(storeFollower)),
                    It.IsAny<Func<IQueryable<StoreFollower>, IOrderedQueryable<StoreFollower>>>(),
                    It.IsAny<Func<IQueryable<StoreFollower>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreFollower, object>>>()
                ))
                .ReturnsAsync(followedStores);

            // Mock StoreDetailService
            var storeDetail = new StoreDetails
            {
                ID = storeId,
                Name = "Test Store",
                ImageUrl = "http://example.com/image.jpg",
                Address = "123 Test Street",
                Phone = "123-456-7890",
                ShortDescriptions = "Test description",
                CreatedDate = DateTime.Now
            };
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            storeDetailServiceMock.Setup(s => s.GetAsyncById(storeId)).ReturnsAsync(storeDetail);

            // Cập nhật controller với tất cả các mock
            _controller = new HomeController(
                _signInManagerMock.Object,
                new Mock<IOrderDetailService>().Object,
                new Mock<IRecipeService>().Object,
                _userManagerMock.Object,
                new Mock<ICategoryService>().Object,
                storeDetailServiceMock.Object,
                new Mock<IEmailSender>().Object,
                new Mock<ICartService>().Object,
                new Mock<IWishlistServices>().Object,
                new Mock<IProductService>().Object,
                new Mock<IProductImageService>().Object,
                new Mock<IProductVariantService>().Object,
                new Mock<IReviewService>().Object,
                new Mock<IBalanceChangeService>().Object,
                new Mock<IOrdersServices>().Object,
                new PayOS("client-id", "api-key", "https://callback.url"),
                new Mock<IVoucherServices>().Object,
                new Mock<IStoreReportServices>().Object,
                _storeFollowersMock.Object,
                new RecipeSearchService(""),
                _expertRecipeServicesMock.Object,
                _recipeViewHistoryServicesMock.Object,
                hubContextMock.Object // <-- Add this argument
            );

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.ManagementFollowedStores();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<List<StoreFollowerViewModel>>(viewResult.Model);
            var model = (List<StoreFollowerViewModel>)viewResult.Model;
            Assert.IsNotEmpty(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test Store", model[0].Name);
        }

        [Test]
        public async Task ManagementFollowedStores_KetNoiThanhCongDanhSachRong()
        {
            // Arrange
            var userId = ("8e91c798-bc78-46a9-89a4-5d0aaea77f5f");
            var user = new AppUser { Id = userId };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeFollowersMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<StoreFollower, bool>>>(),
                    It.IsAny<Func<IQueryable<StoreFollower>, IOrderedQueryable<StoreFollower>>>(),
                    It.IsAny<Func<IQueryable<StoreFollower>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreFollower, object>>>()
                ))
                .ReturnsAsync(new List<StoreFollower>());

            // Act
            var result = await _controller.ManagementFollowedStores();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<List<StoreFollowerViewModel>>(viewResult.Model);
            Assert.IsEmpty((List<StoreFollowerViewModel>)viewResult.Model);
        }

        [Test]
        public async Task ManagementFollowedStores_KetNoiThatBaiCoNgoaiLe()
        {
            // Arrange
            var userId = "8e91c798-b5e9-4649-994-5d0aaea77f5f"; // String, cần chuyển sang Guid
            var user = new AppUser { Id = userId }; // Chuyển string sang Guid
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            httpContext.Response.StatusCode = 500; // Mô phỏng thất bại kết nối máy chủ
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeFollowersMock
                .Setup(s => s.ListAsync(
                    It.IsAny<Expression<Func<StoreFollower, bool>>>(),
                    It.IsAny<Func<IQueryable<StoreFollower>, IOrderedQueryable<StoreFollower>>>(),
                    It.IsAny<Func<IQueryable<StoreFollower>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreFollower, object>>>()
                ))
                .ThrowsAsync(new Exception("Cannot connect to server"));

            // Act
            var result = await _controller.ManagementFollowedStores();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result); // Kiểm tra trả về ObjectResult
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode); // Kiểm tra mã trạng thái
            Assert.AreEqual("An error occurred. Please try again later.", objectResult.Value); // Kiểm tra thông báo
        }
    }
}