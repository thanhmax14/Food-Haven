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
using Microsoft.AspNetCore.Mvc;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Food_Haven.UnitTest.Admin_Detailcomplant_Test
{
    public class Detailcomplant_Test
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
        public async Task Detailcomplant_ReturnsViewResult_WithValidModel_WhenAdminLoggedIn()
        {
            // Arrange
            var adminUser = new AppUser { Id = "admin", UserName = "admin" };
            var complaintId = Guid.NewGuid();
            var orderDetailId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var productTypeId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            var complaint = new Complaint
            {
                ID = complaintId,
                OrderDetailID = orderDetailId,
                Status = "Pending",
                AdminReply = "Admin reply",
                DateAdminReply = DateTime.Now,
                Reply = "Seller reply",
                ReplyDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Description = "Complaint description",
                IsReportAdmin = true,
                AdminReportStatus = "Pending"
            };
            var orderDetail = new OrderDetail
            {
                ID = orderDetailId,
                OrderID = orderId,
                ProductTypesID = productTypeId
            };
            var order = new Order
            {
                ID = orderId,
                UserID = adminUser.Id,
                OrderTracking = "ORD123"
            };
            var productType = new ProductTypes
            {
                ID = productTypeId,
                ProductID = productId,
                Name = "TypeA"
            };
            var product = new Product
            {
                ID = productId,
                StoreID = storeId,
                Name = "ProductA"
            };
            var store = new StoreDetails
            {
                ID = storeId,
                Name = "ShopA"
            };
            var complaintImages = new List<ComplaintImage>
            {
                new ComplaintImage { ComplaintID = complaintId, ImageUrl = "http://image.url/1.jpg" }
            };

            // Mock UserManager
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(adminUser);
            _complaintServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>())).ReturnsAsync(complaint);
            _orderDetailMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _orderMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _variantServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(productType);
            _productMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _userManagerMock.Setup(x => x.FindByIdAsync(order.UserID)).ReturnsAsync(adminUser);
            _storeServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            _complaintImageMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ComplaintImage, bool>>>(), null, null)).ReturnsAsync(complaintImages);

            // Act
            var result = await _controller.Detailcomplant(complaintId);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<ComplantDetailViewmodels>(viewResult.Model);
            var model = viewResult.Model as ComplantDetailViewmodels;
            Assert.AreEqual("Pending", model.Status);
            Assert.AreEqual("Admin reply", model.AdminrReply);
            Assert.AreEqual("Seller reply", model.SellerReply);
            Assert.AreEqual("Complaint description", model.Description);
            Assert.AreEqual("ShopA", model.NameShop);
            Assert.AreEqual("ProductA", model.ProductName);
            Assert.AreEqual("TypeA", model.ProductType);
            Assert.AreEqual("admin", model.UserName);
            Assert.AreEqual("ORD123", model.OrderTracking);
            Assert.IsTrue(model.IsreportAdmin);
            Assert.AreEqual("Pending", model.statusAdmin);
            Assert.IsTrue(model.image.Any(i => i == "http://image.url/1.jpg"));
        }
    }
}
