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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Repository.ViewModels;
using System.Linq.Expressions;

namespace Food_Haven.UnitTest.Admin_UpdateVoucher_Test
{
    public class UpdateVoucher_Test
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
        public async Task UpdateVoucher_InvalidModel_ReturnsBadRequest()
        {
            var invalidVoucher = new VoucherViewModel
            {
                Code = "", // Invalid
                DiscountAmount = -1, // Invalid
                DiscountType = "Invalid", // Invalid
                StartDate = "invalid-date", // Invalid
                ExpirationDate = "invalid-date", // Invalid
                Scope = "",
                MaxUsage = -1,
                CurrentUsage = -1,
                MinOrderValue = -1,
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(invalidVoucher) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var value = result.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsTrue(fieldErrors != null && fieldErrors.Count > 0);
        }

        [Test]
        public async Task UpdateVoucher_VoucherNotFound_ReturnsNotFound()
        {
            var voucher = new VoucherViewModel
            {
                ID = Guid.NewGuid(),
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                Scope = "100",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                IsActive = true
            };

            _voucherMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Voucher, bool>>>()))
                .ReturnsAsync((Voucher?)null);

            var result = await _controller.UpdateVoucher(voucher);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task UpdateVoucher_ValidModel_UpdatesVoucher_ReturnsSuccess()
        {
            var voucherId = Guid.NewGuid();
            var voucherVm = new VoucherViewModel
            {
                ID = voucherId,
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ExpirationDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                Scope = "100",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                IsActive = true
            };

            var entity = new Voucher { ID = voucherId };

            _voucherMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Voucher, bool>>>()))
                .ReturnsAsync(entity);

            _voucherMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await _controller.UpdateVoucher(voucherVm) as JsonResult;
            Assert.IsNotNull(result);

            var value = result.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task UpdateVoucher_MultipleInvalidFields_ReturnsAllFieldErrors()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "", // Invalid
                DiscountAmount = 0, // Invalid
                DiscountType = "content", // Invalid
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = -10, // Invalid
                CurrentUsage = -10, // Invalid
                MinOrderValue = -10, // Invalid
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var value = result.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsFalse(success);
            Assert.IsNotNull(fieldErrors);

            Assert.IsTrue(fieldErrors.ContainsKey("code"));
            Assert.IsTrue(fieldErrors.ContainsKey("discountAmount"));
            Assert.IsTrue(fieldErrors.ContainsKey("discountType"));
            Assert.IsFalse(fieldErrors.ContainsKey("startDate")); // Valid date
            Assert.IsFalse(fieldErrors.ContainsKey("expirationDate")); // Valid date
            Assert.IsTrue(fieldErrors.ContainsKey("maxUsage"));
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
            Assert.IsTrue(fieldErrors.ContainsKey("minOrderValue"));
        }

        [Test]
        public async Task UpdateVoucher_CodeIsEmpty_ReturnsCodeError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("code"));
        }

        [Test]
        public async Task UpdateVoucher_DiscountAmountIsZero_ReturnsDiscountAmountError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 0,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("discountAmount"));
        }

        [Test]
        public async Task UpdateVoucher_DiscountTypeIsInvalid_ReturnsDiscountTypeError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "content",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("discountType"));
        }

        [Test]
        public async Task UpdateVoucher_MaxUsageIsNegative_ReturnsMaxUsageError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = -10,
                CurrentUsage = 0,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("maxUsage"));
        }

        [Test]
        public async Task UpdateVoucher_CurrentUsageIsNegative_ReturnsCurrentUsageError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 10,
                CurrentUsage = -10,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task UpdateVoucher_CurrentUsageExceedsMaxUsage_ReturnsCurrentUsageError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 1000,
                CurrentUsage = 1001,
                MinOrderValue = 0,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("currentUsage"));
        }

        [Test]
        public async Task UpdateVoucher_MinOrderValueIsNegative_ReturnsMinOrderValueError()
        {
            var voucherVm = new VoucherViewModel
            {
                Code = "CODE123",
                DiscountAmount = 10,
                DiscountType = "Fixed",
                StartDate = "2025-08-05",
                ExpirationDate = "2025-12-31",
                MaxUsage = 10,
                CurrentUsage = 0,
                MinOrderValue = -10,
                Scope = "",
                IsActive = true
            };

            var result = await _controller.UpdateVoucher(voucherVm) as BadRequestObjectResult;
            var value = result.Value;
            var type = value.GetType();
            var fieldErrors = type.GetProperty("fieldErrors")?.GetValue(value) as IDictionary<string, string>;
            Assert.IsTrue(fieldErrors.ContainsKey("minOrderValue"));
        }
    }
}
