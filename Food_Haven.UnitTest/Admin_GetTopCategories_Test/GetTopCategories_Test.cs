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
using Models.DBContext;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using static NUnit.Framework.Internal.OSPlatform;

namespace Food_Haven.UnitTest.Admin_GetTopCategories_Test
{
    public class GetTopCategories_Test
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
        public async Task GetTopCategories_ReturnsCorrectResult_ForToday()
        {
            // Arrange
            var now = DateTime.Now;
            var categoryId = Guid.NewGuid();

            var categories = new List<Categories>
            {
                new Categories
                {
                    ID = categoryId,
                    Name = "Category 1",
                    Commission = 5,
                    CreatedDate = now.AddMonths(-1)
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ID = Guid.NewGuid(),
                    Name = "Product 1",
                    CategoryID = categoryId
                }
            };

            var productTypes = new List<ProductTypes>
            {
                new Models.ProductTypes
                {
                    ID = Guid.NewGuid(),
                    ProductID = products[0].ID,
                }
            };

            var orders = new List<Order>
            {
                new Order
                {
                    ID = Guid.NewGuid(),
                    Status = "CONFIRMED",
                    CreatedDate = now.Date.AddHours(10)
                }
            };

            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    ID = Guid.NewGuid(),
                    OrderID = orders[0].ID,
                    ProductTypesID = productTypes[0].ID,
                    Quantity = 2,
                    ProductPrice = 100,
                    CommissionPercent = 5
                }
            };

            // Setup for _product.ListAsync
            _productMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);

            // Setup for _variantService.ListAsync
            _variantServiceMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);

            // Setup for _orderDetail.ListAsync
            _orderDetailMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);

            // Setup for _order.ListAsync
            _orderMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), null, null))
                .ReturnsAsync(orders);

            // Setup for _categoryService.ListAsync
            _categoryServiceMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetTopCategories("today", "");

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Convert the data back to JSON then parse as JsonDocument
            string json = System.Text.Json.JsonSerializer.Serialize(jsonResult.Value);
            var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            Assert.AreEqual(1, root.GetArrayLength());

            var categoryResult = root[0];
            Assert.AreEqual(categoryId.ToString(), categoryResult.GetProperty("id").GetString());
            Assert.AreEqual("Category 1", categoryResult.GetProperty("name").GetString());
            Assert.AreEqual(5, categoryResult.GetProperty("commitson").GetInt32());
            Assert.AreEqual(categories[0].CreatedDate?.ToString("dd/MM/yyyy"), categoryResult.GetProperty("createdDate").GetString());
            Assert.AreEqual(200, categoryResult.GetProperty("grossSales").GetDecimal());
            Assert.AreEqual(10, categoryResult.GetProperty("commissionRevenue").GetDecimal());
            Assert.AreEqual(1, categoryResult.GetProperty("orderCount").GetInt32());
        }

        [Test]
        public async Task GetTopCategories_ReturnsEmpty_WhenNoProducts()
        {
            _productMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(new List<Product>());

            var result = await _controller.GetTopCategories("today", "");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value as IEnumerable<object>;
            Assert.IsNotNull(data);
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task GetTopCategories_ReturnsEmpty_WhenNoProductTypes()
        {
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product 1", CategoryID = Guid.NewGuid() }
            };

            _productMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);

            _variantServiceMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes>());

            var result = await _controller.GetTopCategories("today", "");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value as IEnumerable<object>;
            Assert.IsNotNull(data);
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task GetTopCategories_ReturnsEmpty_WhenNoOrderDetails()
        {
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product 1", CategoryID = Guid.NewGuid() }
            };

            var productTypes = new List<ProductTypes>
            {
                new ProductTypes { ID = Guid.NewGuid(), ProductID = products[0].ID }
            };

            _productMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);

            _variantServiceMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);

            _orderDetailMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(new List<OrderDetail>());

            var result = await _controller.GetTopCategories("today", "");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value as IEnumerable<object>;
            Assert.IsNotNull(data);
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task GetTopCategories_ReturnsEmpty_WhenNoOrders()
        {
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product 1", CategoryID = Guid.NewGuid() }
            };

            var productTypes = new List<ProductTypes>
            {
                new ProductTypes { ID = Guid.NewGuid(), ProductID = products[0].ID }
            };

            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    ID = Guid.NewGuid(),
                    OrderID = Guid.NewGuid(),
                    ProductTypesID = productTypes[0].ID,
                    Quantity = 2,
                    ProductPrice = 100,
                    CommissionPercent = 10
                }
            };

            _productMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);

            _variantServiceMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);

            _orderDetailMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);

            _orderMock.Setup(svc =>
                svc.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), null, null))
                .ReturnsAsync(new List<Order>());

            var result = await _controller.GetTopCategories("today", "");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value as IEnumerable<object>;
            Assert.IsNotNull(data);
            Assert.IsEmpty(data);
        }
    }
}