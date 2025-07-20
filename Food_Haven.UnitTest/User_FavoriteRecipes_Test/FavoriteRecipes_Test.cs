using BusinessLogic.Services.FavoriteFavoriteRecipes;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.TypeOfDishServices;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using X.PagedList;
using System.Linq.Expressions;

namespace Food_Haven.UnitTest.User_FavoriteRecipes_Test
{
    public class FavoriteRecipes_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IFavoriteRecipeService> _favoriteRecipeServiceMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<ITypeOfDishService> _typeOfDishServiceMock;
        private Mock<IHubContext<ChatHub>> _hubContextMock;
        private Mock<IHubContext<FollowHub>> _hubContext1Mock;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _favoriteRecipeServiceMock = new Mock<IFavoriteRecipeService>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _typeOfDishServiceMock = new Mock<ITypeOfDishService>();
            _hubContextMock = new Mock<IHubContext<ChatHub>>();
            _hubContext1Mock = new Mock<IHubContext<FollowHub>>();

            _controller = new UsersController(
                _userManagerMock.Object,
                null, null, _httpContextAccessorMock.Object, null, null, null, null, null, null,
                null, null, null, _recipeServiceMock.Object, null, null, _typeOfDishServiceMock.Object,
                null, null, null, null, null, _hubContextMock.Object, null, _favoriteRecipeServiceMock.Object,
                null, null, _hubContext1Mock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task FavoriteRecipes_NguoiDungHopLeCoCongThuc_TraVePagedListView()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new AppUser { Id = userId };

            var recipeId = Guid.NewGuid();
            var favorite = new FavoriteRecipe
            {
                ID = Guid.NewGuid(),
                UserID = userId,
                RecipeID = recipeId,
                CreatedDate = DateTime.Now
            };

            var recipe = new Recipe
            {
                ID = recipeId,
                ThumbnailImage = "image.jpg",
                Title = "Recipe1",
                DifficultyLevel = "Easy",
                status = "Published",
                ShortDescriptions = "Desc",
                TypeOfDishID = Guid.NewGuid()
            };

            var typeOfDish = new TypeOfDish
            {
                ID = recipe.TypeOfDishID,
                Name = "Main Course"
            };

            var favoritesList = new List<FavoriteRecipe> { favorite };

            // Mock user in HttpContext
            _httpContextAccessorMock.Setup(x => x.HttpContext.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId)
                })));

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _favoriteRecipeServiceMock.Setup(x => x.ListAsync())
                .ReturnsAsync(favoritesList);

            _recipeServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(recipe);

            _typeOfDishServiceMock.Setup(x => x.GetAsyncById(recipe.TypeOfDishID))
                .ReturnsAsync(typeOfDish);

            // Act
            var result = await _controller.FavoriteRecipes(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName == null || viewResult.ViewName == "FavoriteRecipes"); // Có thể null nếu tên view mặc định

            var model = viewResult.Model as IPagedList<FavoriteRecipeViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model.First().Title, Is.EqualTo("Recipe1"));
            Assert.That(model.First().TypeOfDishName, Is.EqualTo("Main Course"));
        }



        [Test]
        public async Task FavoriteRecipes_NguoiDungHopLeKhongCoCongThuc_TraVePagedListRong()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new AppUser { Id = userId };
            var favoritesList = new List<FavoriteRecipe>();

            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            })));
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _favoriteRecipeServiceMock.Setup(x => x.ListAsync()).ReturnsAsync(favoritesList);

            // Act
            var result = await _controller.FavoriteRecipes(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);

            // Sửa lại điều kiện tên View: hoặc là null (mặc định) hoặc bạn gán cụ thể
            Assert.That(viewResult.ViewName, Is.Null.Or.EqualTo("FavoriteRecipes"));

            var model = viewResult.Model as IPagedList<FavoriteRecipeViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(0));
        }


        [Test]
        public async Task FavoriteRecipes_CoNgoaiLe_TraVeException()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new AppUser { Id = userId };

            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            })));
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _favoriteRecipeServiceMock.Setup(x => x.ListAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.FavoriteRecipes(1));
        }

        [Test]
        public async Task FavoriteRecipes_NguoiDungKhongHopLe_TraVeRedirectToLogin()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(new ClaimsPrincipal());
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.FavoriteRecipes(1);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Login"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        }
    }
}
