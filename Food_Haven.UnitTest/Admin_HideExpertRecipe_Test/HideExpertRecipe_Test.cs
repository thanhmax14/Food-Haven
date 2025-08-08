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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.StoreDetails;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_HideExpertRecipe_Test
{
    public class HideExpertRecipe_Test
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
        public async Task HideExpertRecipe_ReturnsSuccess_WhenRecipeExists()
        {
            var recipeId = Guid.NewGuid();
            var mockRecipe = new ExpertRecipe
            {
                ID = recipeId,
                IsActive = true,
                ModifiedDate = DateTime.MinValue
            };

            var hubContextMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();

            clientsMock.Setup(clients => clients.All).Returns(clientProxyMock.Object);
            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            typeof(AdminController)
                .GetField("_hubContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_controller, hubContextMock.Object);

            _expertRecipeServicesMock.Setup(x => x.GetAsyncById(recipeId))
                .ReturnsAsync(mockRecipe);

            _expertRecipeServicesMock.Setup(x => x.UpdateAsync(It.IsAny<ExpertRecipe>()))
                .Returns(Task.CompletedTask);

            _expertRecipeServicesMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await _controller.HideExpertRecipe(recipeId) as JsonResult;
            Assert.IsNotNull(result);

            var json = JObject.FromObject(result.Value);
            Assert.IsTrue((bool)json["success"]);

            _expertRecipeServicesMock.Verify(x => x.UpdateAsync(It.Is<ExpertRecipe>(r => r.IsActive == false)), Times.Once);
            _expertRecipeServicesMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            clientProxyMock.Verify(x => x.SendCoreAsync("ReceiveExperRecipe", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task HideExpertRecipe_ReturnsNotFound_WhenRecipeIsNull()
        {
            var recipeId = Guid.NewGuid();
            _expertRecipeServicesMock.Setup(x => x.GetAsyncById(recipeId))
                .ReturnsAsync((ExpertRecipe)null);

            var result = await _controller.HideExpertRecipe(recipeId) as JsonResult;
            Assert.IsNotNull(result);

            var json = JObject.FromObject(result.Value);
            Assert.IsFalse((bool)json["success"]);
            Assert.AreEqual("Not found", (string)json["message"]);

            _expertRecipeServicesMock.Verify(x => x.UpdateAsync(It.IsAny<ExpertRecipe>()), Times.Never);
            _expertRecipeServicesMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task HideExpertRecipe_ReturnsError_WhenExceptionThrown()
        {
            var recipeId = Guid.NewGuid();
            _expertRecipeServicesMock.Setup(x => x.GetAsyncById(recipeId))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _controller.HideExpertRecipe(recipeId) as JsonResult;
            Assert.IsNotNull(result);

            var json = JObject.FromObject(result.Value);
            Assert.IsFalse((bool)json["success"]);
            Assert.AreEqual("An error occurred while hiding the recipe.", (string)json["message"]);
        }

    }
}
