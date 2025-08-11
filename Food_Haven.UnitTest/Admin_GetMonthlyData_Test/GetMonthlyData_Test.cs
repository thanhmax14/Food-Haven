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
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Food_Haven.UnitTest.Admin_GetMonthlyData_Test
{
    [TestFixture]
    public class GetMonthlyData_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IIngredientTagService> _ingredientTagServiceMock;
        private Mock<IStoreDetailService> _storeServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IBalanceChangeService> _balanceMock;
        private Mock<ICategoryService> _categoryServiceMock;
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
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            _controller = new AdminController(
                _userManagerMock.Object,
                _typeOfDishServiceMock.Object,
                _ingredientTagServiceMock.Object,
                _storeServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _balanceMock.Object,
                _categoryServiceMock.Object,
                manageTransactionMock.Object,
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
        public async Task GetMonthlyData_ReturnsMonthlySummary_WhenServerIsAvailable()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { ID = Guid.NewGuid(), Status = "CONFIRMED", CreatedDate = new DateTime(2025, 6, 1), TotalPrice = 100 },
                new Order { ID = Guid.NewGuid(), Status = "DELIVERING", CreatedDate = new DateTime(2025, 6, 2), TotalPrice = 200 }
            };
            _orderMock.Setup(o => o.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), null, null)).ReturnsAsync(orders);

            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { OrderID = orders[0].ID, ProductPrice = 50, Quantity = 2, CommissionPercent = 10 },
                new OrderDetail { OrderID = orders[1].ID, ProductPrice = 100, Quantity = 1, CommissionPercent = 5 }
            };
            _orderDetailMock.Setup(od => od.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>(), null, null)).ReturnsAsync(orderDetails);

            _balanceMock.Setup(b => b.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BalanceChange, bool>>>(), null, null)).ReturnsAsync(new List<BalanceChange>());

            // Act
            var result = await _controller.GetMonthlyData("2025-06") as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var value = result.Value?.ToString();
            Assert.IsTrue(value?.Contains("summary") ?? false);
            Assert.IsTrue(value?.Contains("chartData") ?? false);
        }

        [Test]
        public async Task GetMonthlyData_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            _orderMock.Setup(o => o.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), null, null)).ThrowsAsync(new Exception("Test server error"));

            // Act
            var result = await _controller.GetMonthlyData("2025-06") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            var value = result.Value?.ToString();
            Assert.IsTrue(value?.Contains("Server error while processing data") ?? false);
            Assert.IsTrue(value?.Contains("Test server error") ?? false);
        }
    }
}
