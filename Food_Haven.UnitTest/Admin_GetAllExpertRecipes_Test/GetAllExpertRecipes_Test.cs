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
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Reflection;

namespace Food_Haven.UnitTest.Admin_GetAllExpertRecipes_Test
{
    [TestFixture]
    public class GetAllExpertRecipes_Test
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
        public async Task GetAllExpertRecipes_ReturnsOrderedJsonResult()
        {
            // Arrange
            var now = DateTime.Now;
            var recipes = new List<ExpertRecipe>
            {
                new ExpertRecipe
                {
                    ID = Guid.NewGuid(),
                    Title = "Recipe 1",
                    Ingredients = "[\"Eggs\",\"Milk\"]",
                    Directions = "[\"Step 1\",\"Step 2\"]",
                    NER = "[\"Eggs\",\"Milk\"]",
                    Link = "http://example.com/1",
                    Source = "Source 1",
                    IsActive = true,
                    CreatedDate = now.AddDays(-2),
                    ModifiedDate = now.AddDays(-1)
                },
                new ExpertRecipe
                {
                    ID = Guid.NewGuid(),
                    Title = "Recipe 2",
                    Ingredients = "[\"Bread\",\"Butter\"]",
                    Directions = "[\"Step 1\"]",
                    NER = "[\"Bread\",\"Butter\"]",
                    Link = "http://example.com/2",
                    Source = "Source 2",
                    IsActive = false,
                    CreatedDate = now.AddDays(-3),
                    ModifiedDate = null
                }
            };

            _expertRecipeServicesMock.Setup(s => s.ListAsync())
                .ReturnsAsync(recipes);

            // Act
            var result = await _controller.GetAllExpertRecipes();

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.IsNotNull(jsonResult.Value);

            // The returned value is a List of anonymous objects, so cast to IEnumerable and use reflection to access properties
            var data = (jsonResult.Value as IEnumerable<object>)?.ToList();
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count);

            // Use reflection to check properties
            object first = data[0];
            object second = data[1];

            Assert.AreEqual(recipes[0].ID, first.GetType().GetProperty("ID")?.GetValue(first));
            Assert.AreEqual(recipes[1].ID, second.GetType().GetProperty("ID")?.GetValue(second));

            Assert.AreEqual(recipes[0].Title, first.GetType().GetProperty("Title")?.GetValue(first));
            Assert.AreEqual(recipes[0].Ingredients, first.GetType().GetProperty("Ingredients")?.GetValue(first));
            Assert.AreEqual(recipes[0].Directions, first.GetType().GetProperty("Directions")?.GetValue(first));
            Assert.AreEqual(recipes[0].NER, first.GetType().GetProperty("NER")?.GetValue(first));
            Assert.AreEqual(recipes[0].Link, first.GetType().GetProperty("Link")?.GetValue(first));
            Assert.AreEqual(recipes[0].Source, first.GetType().GetProperty("Source")?.GetValue(first));
            Assert.AreEqual(recipes[0].IsActive, first.GetType().GetProperty("IsActive")?.GetValue(first));
            Assert.AreEqual(recipes[0].CreatedDate, first.GetType().GetProperty("CreatedDate")?.GetValue(first));
        }
    }
}
