using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Food_Haven.UnitTest.Admin_ShopComplaintRates_Test
{
    public class ShopComplaintRates_Test
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

            _manageTransaction = manageTransactionMock.Object; // <-- Fix: assign the mock object

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
                _manageTransaction, // <-- Now not null
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
                _expertRecipeServicesMock?.Object,
                hubContextMock.Object
            );
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        [Test]
        public async Task ShopComplaintRates_ReturnsCorrectStats()
        {
            // Arrange
            // 2 shops, mỗi shop 2 sản phẩm, mỗi sản phẩm có 1 ProductTypes, mỗi shop có 2 orderDetail, 1 complaint mỗi shop
            var shop1 = new StoreDetails { ID = Guid.NewGuid(), Name = "Store A" };
            var shop2 = new StoreDetails { ID = Guid.NewGuid(), Name = "Store B" };

            var product1A = new Product { ID = Guid.NewGuid(), StoreID = shop1.ID, Name = "P1A" };
            var product1B = new Product { ID = Guid.NewGuid(), StoreID = shop1.ID, Name = "P1B" };
            var product2A = new Product { ID = Guid.NewGuid(), StoreID = shop2.ID, Name = "P2A" };
            var product2B = new Product { ID = Guid.NewGuid(), StoreID = shop2.ID, Name = "P2B" };

            var pt1A = new ProductTypes { ID = Guid.NewGuid(), ProductID = product1A.ID };
            var pt1B = new ProductTypes { ID = Guid.NewGuid(), ProductID = product1B.ID };
            var pt2A = new ProductTypes { ID = Guid.NewGuid(), ProductID = product2A.ID };
            var pt2B = new ProductTypes { ID = Guid.NewGuid(), ProductID = product2B.ID };

            var od1A = new OrderDetail { ID = Guid.NewGuid(), ProductTypesID = pt1A.ID };
            var od1B = new OrderDetail { ID = Guid.NewGuid(), ProductTypesID = pt1B.ID };
            var od2A = new OrderDetail { ID = Guid.NewGuid(), ProductTypesID = pt2A.ID };
            var od2B = new OrderDetail { ID = Guid.NewGuid(), ProductTypesID = pt2B.ID };

            var complaint1 = new Complaint { ID = Guid.NewGuid(), OrderDetailID = od1A.ID, CreatedDate = DateTime.Now.AddDays(-1) };
            var complaint2 = new Complaint { ID = Guid.NewGuid(), OrderDetailID = od2A.ID, CreatedDate = DateTime.Now.AddDays(-1) };

            _storeServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>(), null, null))
                .ReturnsAsync(new List<StoreDetails> { shop1, shop2 });
            _storeServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<StoreDetails> { shop1, shop2 });

            _productMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(new List<Product> { product1A, product1B, product2A, product2B });
            _productMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<Product> { product1A, product1B, product2A, product2B });

            _variantServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(new List<ProductTypes> { pt1A, pt1B, pt2A, pt2B });
            _variantServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<ProductTypes> { pt1A, pt1B, pt2A, pt2B });

            _orderDetailMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(new List<OrderDetail> { od1A, od1B, od2A, od2B });
            _orderDetailMock.Setup(x => x.ListAsync()).ReturnsAsync(new List<OrderDetail> { od1A, od1B, od2A, od2B });

            _complaintServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Complaint, bool>>>(), null, null))
                .ReturnsAsync(new List<Complaint> { complaint1, complaint2 });

            // Act
            var result = await _controller.ShopComplaintRates("lastmonth");

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            string json = System.Text.Json.JsonSerializer.Serialize(jsonResult.Value);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            Assert.IsTrue(root.GetProperty("success").GetBoolean());
            var data = root.GetProperty("data");

            var series = data.GetProperty("series").EnumerateArray().ToList();
            var percentages = data.GetProperty("percentages").EnumerateArray().ToList();
            var labels = data.GetProperty("labels").EnumerateArray().ToList();
            var total = data.GetProperty("total").GetInt32();

            Assert.AreEqual(2, series.Count);
            Assert.Contains("Store A", labels.Select(x => x.GetString()).ToList());
            Assert.Contains("Store B", labels.Select(x => x.GetString()).ToList());

            // Mỗi shop có 2 đơn, 1 khiếu nại => complaintRate = 50
            Assert.IsTrue(percentages.All(p => Math.Abs(p.GetDouble() - 50.0) < 0.01));
            Assert.IsTrue(series.All(s => s.GetInt32() == 1));
            Assert.AreEqual(4, total);
        }
    }
}
