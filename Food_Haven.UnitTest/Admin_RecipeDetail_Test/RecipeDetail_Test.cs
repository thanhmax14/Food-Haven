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
using Models;
using Models.DBContext;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_RecipeDetail_Test
{
    public class RecipeDetail_Test
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
        public async Task RecipeDetail_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var typeOfDishId = Guid.NewGuid();
            var cateId = Guid.NewGuid();
            var ingredientTagId = Guid.NewGuid();

            var recipe = new Recipe
            {
                ID = recipeId,
                Title = "Test Recipe",
                ShortDescriptions = "Short desc",
                PreparationTime = "10",
                CookTime = "20",
                TotalTime = "30",
                DifficultyLevel = "Easy",
                Servings = "2",
                CreatedDate = DateTime.Now,
                IsActive = true,
                CateID = cateId,
                ThumbnailImage = "img.jpg",
                TypeOfDishID = typeOfDishId,
                CookingStep = "Step 1",
                Ingredient = "Ingredient 1"
            };

            var typeOfDish = new TypeOfDish { ID = typeOfDishId, Name = "Main Dish" };
            var ingredientTag = new IngredientTag { ID = ingredientTagId, Name = "Tag1" };
            var selectedTags = new List<RecipeIngredientTag>
            {
                new RecipeIngredientTag { RecipeID = recipeId, IngredientTagID = ingredientTagId, IngredientTag = ingredientTag }
            };
            var allTags = new List<IngredientTag> { ingredientTag };
            var allDishes = new List<TypeOfDish> { typeOfDish };
            var allCategories = new List<Categories> { new Categories { ID = cateId, Name = "Category1" } };

            _recipeServiceMock.Setup(s => s.GetAsyncById(recipeId)).ReturnsAsync(recipe);
            _typeOfDishServiceMock.Setup(s => s.GetAsyncById(typeOfDishId)).ReturnsAsync(typeOfDish);
            _recipeIngredientTagServiceMock.Setup(s => s.ListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<RecipeIngredientTag, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeIngredientTag>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RecipeIngredientTag, object>>>()
            )).ReturnsAsync(selectedTags);
            _ingredientTagServiceMock.Setup(s => s.ListAsync()).ReturnsAsync(allTags); // returns 1 tag for allTags
            _typeOfDishServiceMock.Setup(s => s.ListAsync()).ReturnsAsync(allDishes);
            _categoryServiceMock.Setup(s => s.ListAsync()).ReturnsAsync(allCategories);

            // Act
            var result = await _controller.RecipeDetail(recipeId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("RecipeDetail", viewResult.ViewName);

            var model = viewResult.Model as RecipeViewModels;
            Assert.IsNotNull(model);
            Assert.AreEqual(recipeId, model.ID);
            Assert.AreEqual("Test Recipe", model.Title);
            Assert.AreEqual("Main Dish", model.TypeOfDishName);
            Assert.AreEqual(typeOfDishId, model.TypeOfDishID);
            Assert.AreEqual(cateId, model.CateID);
            Assert.AreEqual("img.jpg", model.ThumbnailImage);
            Assert.AreEqual("Step 1", model.CookingStep);
            Assert.AreEqual("Ingredient 1", model.Ingredient);
            Assert.AreEqual(1, model.SelectedIngredientTags.Count, $"SelectedIngredientTags count should be 1 but was {model.SelectedIngredientTags.Count}");
            Assert.AreEqual(ingredientTagId, model.SelectedIngredientTags[0]);
            Assert.IsNotNull(model.IngredientTags, "IngredientTags should not be null");
            Assert.AreEqual(1, model.IngredientTags.Count, $"IngredientTags count should be 1 but was {model.IngredientTags.Count}");
            Assert.AreEqual(ingredientTagId, model.IngredientTags[0].ID);
            Assert.AreEqual(1, model.typeOfDishes.Count, $"typeOfDishes count should be 1 but was {model.typeOfDishes.Count}");
            Assert.AreEqual(typeOfDish.ID, model.typeOfDishes[0].ID);
            Assert.AreEqual(1, model.Categories.Count, $"Categories count should be 1 but was {model.Categories.Count}");
        }

        [Test]
        public async Task RecipeDetail_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _recipeServiceMock.Setup(s => s.GetAsyncById(invalidId)).ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.RecipeDetail(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
