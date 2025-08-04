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

namespace Food_Haven.UnitTest.Admin_GetAllVoucher_Test
{
    public class GetAllVoucher_Test
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
        public void GetAllVoucher_ReturnsJsonResult_WithVoucherList()
        {
            // Arrange
            var voucherMock = new Mock<IVoucherServices>();
            var vouchers = new List<Voucher>
            {
                new Voucher
                {
                    ID = Guid.NewGuid(),
                    Code = "ABC123",
                    DiscountAmount = 10,
                    DiscountType = "Fixed",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    StartDate = DateTime.Now.AddDays(-1),
                    ExpirationDate = DateTime.Now.AddDays(10),
                    MaxUsage = 100,
                    CurrentUsage = 10,
                    MinOrderValue = 50,
                    IsActive = true,
                    IsGlobal = true,
                    MaxDiscountAmount = 20
                }
            };
            voucherMock.Setup(x => x.GetAll()).Returns(vouchers.AsQueryable());

            // Create all required mocks for AdminController constructor
            var userManagerMock = new Mock<UserManager<AppUser>>(new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);
            var typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            var ingredientTagServiceMock = new Mock<IIngredientTagService>();
            var storeServiceMock = new Mock<IStoreDetailService>();
            var mapperMock = new Mock<IMapper>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var balanceMock = new Mock<IBalanceChangeService>();
            var categoryServiceMock = new Mock<ICategoryService>();
            var manageTransactionMock = new Mock<ManageTransaction>(new Mock<FoodHavenDbContext>(new DbContextOptions<FoodHavenDbContext>()).Object);
            var complaintServiceMock = new Mock<IComplaintServices>();
            var orderDetailMock = new Mock<IOrderDetailService>();
            var orderMock = new Mock<IOrdersServices>();
            var variantServiceMock = new Mock<IProductVariantService>();
            var complaintImageMock = new Mock<IComplaintImageServices>();
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            var productMock = new Mock<IProductService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            var storeRepoMock = new Mock<IStoreReportServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var productImageServiceMock = new Mock<IProductImageService>();
            var recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
            var expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            var controller = new AdminController(
                userManagerMock.Object,
                typeOfDishServiceMock.Object,
                ingredientTagServiceMock.Object,
                storeServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                balanceMock.Object,
                categoryServiceMock.Object,
                manageTransactionMock.Object,
                complaintServiceMock.Object,
                orderDetailMock.Object,
                orderMock.Object,
                variantServiceMock.Object,
                complaintImageMock.Object,
                storeDetailServiceMock.Object,
                productMock.Object,
                voucherMock.Object,
                recipeServiceMock.Object,
                storeRepoMock.Object,
                storeReportMock.Object,
                productImageServiceMock.Object,
                recipeIngredientTagServiceMock.Object,
                roleManagerMock.Object,
                expertRecipeServicesMock.Object,
                hubContextMock.Object
            );

            // Act
            var result = controller.GetAllVoucher() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var data = result.Value as IEnumerable<object>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Cast<object>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAllVoucher_ReturnsJsonResult_WithEmptyList()
        {
            // Arrange
            var voucherMock = new Mock<IVoucherServices>();
            var vouchers = new List<Voucher>();
            voucherMock.Setup(x => x.GetAll()).Returns(vouchers.AsQueryable());

            // Create all required mocks for AdminController constructor
            var userManagerMock = new Mock<UserManager<AppUser>>(new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);
            var typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            var ingredientTagServiceMock = new Mock<IIngredientTagService>();
            var storeServiceMock = new Mock<IStoreDetailService>();
            var mapperMock = new Mock<IMapper>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var balanceMock = new Mock<IBalanceChangeService>();
            var categoryServiceMock = new Mock<ICategoryService>();
            var manageTransactionMock = new Mock<ManageTransaction>(new Mock<FoodHavenDbContext>(new DbContextOptions<FoodHavenDbContext>()).Object);
            var complaintServiceMock = new Mock<IComplaintServices>();
            var orderDetailMock = new Mock<IOrderDetailService>();
            var orderMock = new Mock<IOrdersServices>();
            var variantServiceMock = new Mock<IProductVariantService>();
            var complaintImageMock = new Mock<IComplaintImageServices>();
            var storeDetailServiceMock = new Mock<IStoreDetailService>();
            var productMock = new Mock<IProductService>();
            var recipeServiceMock = new Mock<IRecipeService>();
            var storeRepoMock = new Mock<IStoreReportServices>();
            var storeReportMock = new Mock<IStoreReportServices>();
            var productImageServiceMock = new Mock<IProductImageService>();
            var recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
            var expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            var controller = new AdminController(
                userManagerMock.Object,
                typeOfDishServiceMock.Object,
                ingredientTagServiceMock.Object,
                storeServiceMock.Object,
                mapperMock.Object,
                webHostEnvironmentMock.Object,
                balanceMock.Object,
                categoryServiceMock.Object,
                manageTransactionMock.Object,
                complaintServiceMock.Object,
                orderDetailMock.Object,
                orderMock.Object,
                variantServiceMock.Object,
                complaintImageMock.Object,
                storeDetailServiceMock.Object,
                productMock.Object,
                voucherMock.Object,
                recipeServiceMock.Object,
                storeRepoMock.Object,
                storeReportMock.Object,
                productImageServiceMock.Object,
                recipeIngredientTagServiceMock.Object,
                roleManagerMock.Object,
                expertRecipeServicesMock.Object,
                hubContextMock.Object
            );

            // Act
            var result = controller.GetAllVoucher() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var data = result.Value as IEnumerable<object>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Cast<object>().Count(), Is.EqualTo(0));
        }
    }
}
