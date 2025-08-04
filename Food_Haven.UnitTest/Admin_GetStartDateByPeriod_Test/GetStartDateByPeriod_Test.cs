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
using System;
using Food_Haven.Web.Controllers;

namespace Food_Haven.UnitTest.Admin_GetStartDateByPeriod_Test
{
    public class GetStartDateByPeriod_Test
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

        [TestFixture]
        public class GetStartDateByPeriod_Tests
        {
            private AdminController _controller;

            [SetUp]
            public void Setup()
            {
                var userManagerMock = new Mock<UserManager<AppUser>>(new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);
                var typeOfDishServiceMock = new Mock<ITypeOfDishService>();
                var ingredientTagServiceMock = new Mock<IIngredientTagService>();
                var storeServiceMock = new Mock<IStoreDetailService>();
                var mapperMock = new Mock<IMapper>();
                var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
                var balanceMock = new Mock<IBalanceChangeService>();
                var categoryServiceMock = new Mock<ICategoryService>();
                var options = new DbContextOptionsBuilder<FoodHavenDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestDb")
                    .Options;
                var dbContext = new FoodHavenDbContext(options);
                var manageTransactionMock = new Mock<ManageTransaction>(dbContext);
                var complaintServiceMock = new Mock<IComplaintServices>();
                var orderDetailMock = new Mock<IOrderDetailService>();
                var orderMock = new Mock<IOrdersServices>();
                var variantServiceMock = new Mock<IProductVariantService>();
                var complaintImageMock = new Mock<IComplaintImageServices>();
                var productMock = new Mock<IProductService>();
                var voucherMock = new Mock<IVoucherServices>();
                var recipeServiceMock = new Mock<IRecipeService>();
                var storeReportMock = new Mock<IStoreReportServices>();
                var productImageServiceMock = new Mock<IProductImageService>();
                var recipeIngredientTagServiceMock = new Mock<IRecipeIngredientTagIngredientTagSerivce>();
                var roleManagerMock = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
                var expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
                var hubContextMock = new Mock<IHubContext<ChatHub>>();

                _controller = new AdminController(
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
                    storeServiceMock.Object,
                    productMock.Object,
                    voucherMock.Object,
                    recipeServiceMock.Object,
                    storeReportMock.Object,
                    storeReportMock.Object,
                    productImageServiceMock.Object,
                    recipeIngredientTagServiceMock.Object,
                    roleManagerMock.Object,
                    expertRecipeServicesMock.Object,
                    hubContextMock.Object
                );
            }

            [TearDown]
            public void TearDown()
            {
                _controller?.Dispose();
            }

            [TestCase("today")]
            [TestCase("")]
            public void GetStartDateByPeriod_TodayOrEmpty_ReturnsToday(string input)
            {
                var now = DateTime.Now.Date;
                var result = InvokeGetStartDateByPeriod(input);
                Assert.That(result, Is.EqualTo(now).Within(TimeSpan.FromSeconds(1)));
            }

           

            [Test]
            public void GetStartDateByPeriod_Yesterday_ReturnsYesterday()
            {
                var expected = DateTime.Now.Date.AddDays(-1);
                var result = InvokeGetStartDateByPeriod("yesterday");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            [Test]
            public void GetStartDateByPeriod_Last7Days_Returns7DaysAgo()
            {
                var expected = DateTime.Now.Date.AddDays(-7);
                var result = InvokeGetStartDateByPeriod("last7days");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            [Test]
            public void GetStartDateByPeriod_Last30Days_Returns30DaysAgo()
            {
                var expected = DateTime.Now.Date.AddDays(-30);
                var result = InvokeGetStartDateByPeriod("last30days");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            [Test]
            public void GetStartDateByPeriod_ThisMonth_ReturnsFirstDayOfMonth()
            {
                var now = DateTime.Now;
                var expected = new DateTime(now.Year, now.Month, 1);
                var result = InvokeGetStartDateByPeriod("thismonth");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            [Test]
            public void GetStartDateByPeriod_LastMonth_ReturnsFirstDayOfLastMonth()
            {
                var now = DateTime.Now;
                var expected = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                var result = InvokeGetStartDateByPeriod("lastmonth");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            [Test]
            public void GetStartDateByPeriod_AllTime_ReturnsMinValue()
            {
                var result = InvokeGetStartDateByPeriod("alltime");
                Assert.AreEqual(DateTime.MinValue, result);
            }

            [Test]
            public void GetStartDateByPeriod_Unknown_ReturnsToday()
            {
                var expected = DateTime.Now.Date;
                var result = InvokeGetStartDateByPeriod("unknown");
                Assert.That(result, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
            }

            private DateTime InvokeGetStartDateByPeriod(string period)
            {
                var method = typeof(AdminController).GetMethod("GetStartDateByPeriod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (DateTime)method.Invoke(_controller, new object[] { period });
            }
        }
    }
}
