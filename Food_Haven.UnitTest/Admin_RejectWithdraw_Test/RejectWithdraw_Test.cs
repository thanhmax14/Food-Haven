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
using System.Security.Claims;
using System.Threading.Tasks;

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
        private Mock<IHubContext<ChatHub>> _hubContextMock;

        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo đầy đủ tất cả các mock
            var userStore = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
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
                .Returns<Func<Task>>(async func =>
                {
                    await func();
                    return true;
                });
            _manageTransaction = manageTransactionMock.Object;

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
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            // Khởi tạo controller với tất cả mock
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
                _hubContextMock.Object
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
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new AppUser { Id = "fake-user" });

            _balanceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BalanceChange, bool>>>()))
                .ReturnsAsync((BalanceChange)null);

            // Act
            var result = await _controller.RejectWithdraw("");
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var value = jsonResult.Value;
            if (value is IDictionary<string, object> dict)
            {
                Assert.IsFalse((bool)dict["success"]);
                Assert.AreEqual("Invalid ID!", dict["msg"]);
            }
            else
            {
                var type = value?.GetType();
                var successProp = type?.GetProperty("success");
                var msgProp = type?.GetProperty("msg");

                Assert.IsNotNull(successProp);
                Assert.IsNotNull(msgProp);

                Assert.IsFalse((bool)successProp.GetValue(value));
                Assert.AreEqual("Invalid ID!", msgProp.GetValue(value)?.ToString());
            }
        }

        [Test]
        public async Task RejectWithdraw_Success_ReturnsJsonSuccess()
        {
            var adminUser = new AppUser { UserName = "admin", Id = "admin-id" };
            var withdrawUser = new AppUser { UserName = "user", Id = "user-id" };
            var guid = Guid.NewGuid();
            var withdraw = new BalanceChange { ID = guid, UserID = "user-id", MoneyChange = 1000 };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(adminUser);
            _balanceMock.Setup(b => b.FindAsync(It.IsAny<Expression<Func<BalanceChange, bool>>>())).ReturnsAsync(withdraw);
            _userManagerMock.Setup(u => u.FindByIdAsync("user-id")).ReturnsAsync(withdrawUser);
            _balanceMock.Setup(b => b.GetBalance(withdrawUser.Id)).ReturnsAsync(5000);
            _balanceMock.Setup(b => b.UpdateAsync(withdraw)).Returns(Task.CompletedTask);
            _balanceMock.Setup(b => b.SaveChangesAsync()).ReturnsAsync(1);

            var manageTransactionMock = new Mock<ManageTransaction>(new FoodHavenDbContext(new DbContextOptionsBuilder<FoodHavenDbContext>().Options));
            manageTransactionMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async (func) => { await func(); return true; });

            typeof(AdminController).GetField("_managetrans", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_controller, manageTransactionMock.Object);

            // Act
            var result = await _controller.RejectWithdraw(guid.ToString());
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var value = jsonResult.Value;
            if (value is IDictionary<string, object> dict)
            {
                Assert.IsTrue((bool)dict["success"]);
                Assert.AreEqual("Confirm withdrawal rejection successful!", dict["msg"]);
            }
            else
            {
                var type = value?.GetType();
                var successProp = type?.GetProperty("success");
                var msgProp = type?.GetProperty("msg");

                Assert.IsTrue((bool)successProp.GetValue(value));
                Assert.AreEqual("Confirm withdrawal rejection successful!", msgProp.GetValue(value)?.ToString());
            }
        }
    }
}
