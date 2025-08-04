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
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Repository.BalanceChange;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_CreateVoucher_Test
{
    public class CreateVoucher_Test
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
            var manageTransactionMock = new Mock<ManageTransaction>(dbContext); // truyền instance
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
                _storeServiceMock.Object, // storeDetailService
                _productMock.Object,
                _voucherMock.Object,
                _recipeServiceMock.Object,
                _storeReportMock.Object, // storeRepo
                _storeReportMock.Object, // storeReport
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
        public async Task CreateVoucher_Valid_ReturnsSuccessJson()
        {
            // Arrange
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _voucherMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>()))
                .ReturnsAsync((Voucher)null);
            _voucherMock.Setup(x => x.AddAsync(It.IsAny<Voucher>())).Returns(Task.CompletedTask);
            _voucherMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd"),
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };

            // Act
            var result = await _controller.CreateVoucher(voucherVm);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var value = json.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task CreateVoucher_CodeEmpty_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("code"));
        }

        [Test]
        public async Task CreateVoucher_DiscountAmountZero_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 0,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("discountAmount"));
        }

        [Test]
        public async Task CreateVoucher_DiscountTypeInvalid_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "content",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("discountType"));
        }

        [Test]
        public async Task CreateVoucher_StartDateAfterExpirationDate_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-12-31",
                ExpirationDate = "2025-08-05",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("startDate"));
        }

        [Test]
        public async Task CreateVoucher_MaxUsageNegative_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = -10,
                CurrentUsage = 0,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("maxUsage"));
        }

        [Test]
        public async Task CreateVoucher_CurrentUsageNegative_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 100,
                CurrentUsage = -10,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task CreateVoucher_CurrentUsageExceedsMaxUsage_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 1000,
                CurrentUsage = 1001,
                MinOrderValue = 50,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task CreateVoucher_MinOrderValueNegative_ReturnsError()
        {
            var user = new AppUser { UserName = "admin" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);
            var voucherVm = new VoucherViewModel
            {
                Code = "NEWCODE",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 100,
                CurrentUsage = 0,
                MinOrderValue = -10,
                Scope = "100",
                IsActive = true
            };
            var result = await _controller.CreateVoucher(voucherVm);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            var value = badRequest.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors.ContainsKey("minOrderValue"));
        }
    }
}
