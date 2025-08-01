using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
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

namespace Food_Haven.UnitTest.Admin_RejectWithdraw_Test
{
    public class RejectWithdraw_Test
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
                _roleManagerMock.Object
            );
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task RejectWithdraw_InvalidId_ReturnsJsonError()
        {
            var result = await _controller.AcceptWithdraw("");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value.GetType().GetProperty("success") != null
                ? jsonResult.Value
                : (object)new Dictionary<string, object>(jsonResult.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(jsonResult.Value)));
            bool success = (bool)(data is IDictionary<string, object> dict ? dict["success"] : data.GetType().GetProperty("success").GetValue(data));
            string msg = (string)(data is IDictionary<string, object> dict2 ? dict2["msg"] : data.GetType().GetProperty("msg").GetValue(data));
            Assert.IsFalse(success);
            Assert.AreEqual("Invalid ID!", msg);
        }


        [Test]
        public async Task RejectWithdraw_Success_ReturnsJsonSuccess()
        {
            // Arrange
            var adminUser = new AppUser { UserName = "admin", Id = "admin-id" };
            var withdrawUser = new AppUser { UserName = "user", Id = "user-id" };
            var guid = Guid.NewGuid();
            var withdraw = new BalanceChange { ID = guid, UserID = "user-id", MoneyChange = 1000 };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(adminUser);
            _balanceMock.Setup(b => b.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BalanceChange, bool>>>())).ReturnsAsync(withdraw);
            _userManagerMock.Setup(u => u.FindByIdAsync("user-id")).ReturnsAsync(withdrawUser);
            _balanceMock.Setup(b => b.GetBalance(withdrawUser.Id)).ReturnsAsync(5000);
            _balanceMock.Setup(b => b.UpdateAsync(withdraw)).Returns(Task.CompletedTask);
            _balanceMock.Setup(b => b.SaveChangesAsync()).ReturnsAsync(1);
            var manageTransactionMock = new Mock<ManageTransaction>(new FoodHavenDbContext(new DbContextOptionsBuilder<FoodHavenDbContext>().Options));
            manageTransactionMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) => { await func(); return true; });
            // Replace _manageTransaction with mock
            typeof(AdminController).GetField("_managetrans", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_controller, manageTransactionMock.Object);

            // Act
            var result = await _controller.RejectWithdraw(guid.ToString());
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value.GetType().GetProperty("success") != null
                ? jsonResult.Value
                : (object)new Dictionary<string, object>(jsonResult.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(jsonResult.Value)));
            bool success = (bool)(data is IDictionary<string, object> dict ? dict["success"] : data.GetType().GetProperty("success").GetValue(data));
            string msg = (string)(data is IDictionary<string, object> dict2 ? dict2["msg"] : data.GetType().GetProperty("msg").GetValue(data));
            Assert.IsTrue(success);
            Assert.AreEqual("Confirm withdrawal rejection successful!", msg);
        }
    }
}
