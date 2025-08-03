using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeIngredientTagIngredientTagServices;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.TypeOfDishServices;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.StoreDetails;
using Repository.ViewModels;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_RejectSeller_Test
{
    public class RejectSeller_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IIngredientTagService> _ingredientTagServiceMock;
        private Mock<IStoreDetailService> _storeServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<StoreDetailsRepository> _storeRepositoryMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private ManageTransaction _manageTransaction;
        private Mock<IComplaintServices> _complaintServiceMock;
        private Mock<IOrderDetailService> _orderDetailMock;
        private Mock<IOrdersServices> _orderMock;
        private Mock<IProductVariantService> _variantServiceMock;
        private Mock<IComplaintImageServices> _complaintImageMock;
        private Mock<IProductService> _productMock;
        private Mock<IVoucherServices> _voucherMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<IStoreReportServices> _storeReportMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IRecipeIngredientTagIngredientTagSerivce> _recipeIngredientTagServiceMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            _ingredientTagServiceMock = new Mock<IIngredientTagService>();
            _storeServiceMock = new Mock<IStoreDetailService>();
            _mapperMock = new Mock<IMapper>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _balanceMock = new Mock<IBalanceChangeService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var dbContext = new FoodHavenDbContext(options);
            var manageTransactionMock = new Mock<ManageTransaction>(dbContext);
            manageTransactionMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) =>
                {
                    await func();
                    return true;
                });

            _complaintServiceMock = new Mock<IComplaintServices>();
            _orderDetailMock = new Mock<IOrderDetailService>();
            _orderMock = new Mock<IOrdersServices>();
            _variantServiceMock = new Mock<IProductVariantService>();
            _complaintImageMock = new Mock<IComplaintImageServices>();
            _productMock = new Mock<IProductService>();
            _voucherMock = new Mock<IVoucherServices>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _storeReportMock = new Mock<IStoreReportServices>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>(); // Add this line

            _controller = new AdminController(
                _userManagerMock.Object,
                _typeOfDishServiceMock.Object,
                _ingredientTagServiceMock.Object,
                _storeServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _balanceMock.Object,
                _categoryServiceMock.Object,
                _manageTransaction,
                _complaintServiceMock.Object,
                _orderDetailMock.Object,
                _orderMock.Object,
                _variantServiceMock.Object,
                _complaintImageMock.Object,
                _storeServiceMock.Object,
                _productMock.Object,
                _voucherMock.Object,
                _recipeServiceMock.Object,
                _storeReportMock.Object,
                _storeReportMock.Object,
                _productImageServiceMock.Object,
                _recipeIngredientTagServiceMock.Object,
                _roleManagerMock.Object,
                _expertRecipeServicesMock.Object,
                                hubContextMock.Object

            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task RejectSeller_ReturnsSuccess_WhenUserExistsAndUpdateSucceeds()
        {
            // Arrange
            var admin = new AppUser { Id = "admin", UserName = "admin" };
            var user = new AppUser { Id = "user1", Email = "huyy1035@gmail.com" };
            var usersViewModel = new UsersViewModel { Email = "huyy1035@gmail.com", RejectNote = "Not qualified" };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(admin);
            _userManagerMock.Setup(m => m.IsInRoleAsync(admin, "Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(m => m.FindByEmailAsync("huyy1035@gmail.com")).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RejectSeller(usersViewModel);

            object value = null;
            if (result is JsonResult jsonResult)
                value = jsonResult.Value;
            else if (result is ObjectResult objectResult)
                value = objectResult.Value;

            Assert.IsNotNull(value);

            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);

            Assert.IsTrue(success);
            Assert.AreEqual("User rejected as seller", message);
        }

        [Test]
        public async Task RejectSeller_ReturnsUserNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var admin = new AppUser { Id = "admin", UserName = "admin" };
            var usersViewModel = new UsersViewModel { Email = "abc@gmail.com", RejectNote = "Not found" };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(admin);
            _userManagerMock.Setup(m => m.IsInRoleAsync(admin, "Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(m => m.FindByEmailAsync("abc@gmail.com")).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.RejectSeller(usersViewModel);

            object value = null;
            if (result is JsonResult jsonResult)
                value = jsonResult.Value;
            else if (result is ObjectResult objectResult)
                value = objectResult.Value;

            Assert.IsNotNull(value);

            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);

            Assert.IsFalse(success);
            Assert.AreEqual("User not found", message);
        }

        [Test]
        public async Task RejectSeller_ReturnsError_WhenUpdateFails()
        {
            // Arrange
            var admin = new AppUser { Id = "admin", UserName = "admin" };
            var user = new AppUser { Id = "user1", Email = "huyy1035@gmail.com" };
            var usersViewModel = new UsersViewModel { Email = "huyy1035@gmail.com", RejectNote = "Not qualified" };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(admin);
            _userManagerMock.Setup(m => m.IsInRoleAsync(admin, "Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(m => m.FindByEmailAsync("huyy1035@gmail.com")).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _controller.RejectSeller(usersViewModel);

            object value = null;
            if (result is JsonResult jsonResult)
                value = jsonResult.Value;
            else if (result is ObjectResult objectResult)
                value = objectResult.Value;

            Assert.IsNotNull(value);

            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);

            Assert.IsFalse(success);
            Assert.AreEqual("Failed to update user", message);
        }
    }
}
