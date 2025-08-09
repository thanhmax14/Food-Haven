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
using Models;
using Moq;
using Net.payOS;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_GetAllCategory_Test
{
    public class GetAllCategory_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private HomeController _controller;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null
            );

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");
            var orderDetailMock = new Mock<IOrderDetailService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            _categoryServiceMock = new Mock<ICategoryService>();
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
            var voucherMock = new Mock<IVoucherServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var storeFollowersMock = new Mock<IStoreFollowersService>();
            var recipeSearchMock = new RecipeSearchService("");
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            // ✅ Khởi tạo các mock bị thiếu
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();

            _controller = new HomeController(
                _signInManagerMock.Object,
                orderDetailMock.Object,
                recipeServiceMock.Object,
                _userManagerMock.Object,
                _categoryServiceMock.Object,
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
                payOS,
                voucherMock.Object,
                storeReportMock.Object,
                storeFollowersMock.Object,
                recipeSearchMock,
                _expertRecipeServicesMock.Object,        // đã khởi tạo
                _recipeViewHistoryServicesMock.Object,   // đã khởi tạo
                hubContextMock.Object
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
        public async Task TC01_ReturnsViewWithFilteredList()
        {
            // Arrange
            var searchName = "milk";
            var categories = new List<Categories>
    {
        new Categories { ID =Guid.NewGuid(), Name = "Milk Tea", CreatedDate = DateTime.Now, Commission = 0.1f, ImageUrl = "img1.jpg", ModifiedDate = DateTime.Now }
    };

            _categoryServiceMock.Setup(x => x.ListAsync(
     It.IsAny<Expression<Func<Categories, bool>>>(),
     It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>(),
     null))
     .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllCategory(searchName);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<CategoryViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Milk Tea", model[0].Name);
        }
        [Test]
        public async Task TC02_GetAllCategory_WithSearchName_ReturnsViewWithData()
        {
            // Arrange
            var categories = new List<Categories>
    {
        new Categories { ID = Guid.NewGuid(), Name = "Tea", CreatedDate = DateTime.Now, Commission = 0.1f, ImageUrl = "img.jpg", ModifiedDate = DateTime.Now }
    };

            _categoryServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Categories, bool>>>(),
                It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>(),
                null)).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllCategory("tea");

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<CategoryViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Tea", model[0].Name);
        }
        [Test]
        public async Task TC03_GetAllCategory_ThrowsException_ReturnsEmptyList()
        {
            // Arrange
            _categoryServiceMock.Setup(x => x.ListAsync(
                null,
                It.IsAny<Func<IQueryable<Categories>, IOrderedQueryable<Categories>>>(),
                null)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllCategory(null);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<CategoryViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count); // empty list fallback
        }

    }
}
