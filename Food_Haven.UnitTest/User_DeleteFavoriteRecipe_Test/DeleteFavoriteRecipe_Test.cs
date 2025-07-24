using BusinessLogic.Services.FavoriteFavoriteRecipes;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.TypeOfDishServices;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.DotNet.MSIdentity.Shared;

namespace Food_Haven.UnitTest.User_DeleteFavoriteRecipe_Test
{
    public class DeleteFavoriteRecipe_Test
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
                null, null, _hubContext1Mock.Object,null
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task DeleteFavoriteRecipe_Success_ReturnsSuccessMessage()
        {
            // Arrange
            var favoriteId = Guid.NewGuid();
            var favoriteRecipe = new FavoriteRecipe { ID = favoriteId };

            _favoriteRecipeServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<FavoriteRecipe, bool>>>()))
                .ReturnsAsync(favoriteRecipe);
            _favoriteRecipeServiceMock.Setup(x => x.DeleteAsync(favoriteRecipe))
                .Returns(Task.CompletedTask);
            _favoriteRecipeServiceMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteFavoriteRecipe(favoriteId) as JsonResult;

            // ✅ Parse JSON an toàn không cần class
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(result.Value));
            var root = doc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            string message = root.GetProperty("message").GetString();

            // Assert
            Assert.AreEqual(true, success);
            Assert.AreEqual("Favorite removed successfully.", message);
        }


        [Test]
        public async Task DeleteFavoriteRecipe_NotFound_ReturnsFailureMessage()
        {
            var favoriteId = Guid.NewGuid();

            _favoriteRecipeServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<FavoriteRecipe, bool>>>()))
                .ReturnsAsync((FavoriteRecipe)null);

            var result = await _controller.DeleteFavoriteRecipe(favoriteId) as JsonResult;

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(result.Value));
            var root = doc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            string message = root.GetProperty("message").GetString();

            Assert.AreEqual(false, success);
            Assert.AreEqual("Favorite not found.", message);
        }



        [Test]
        public async Task DeleteFavoriteRecipe_ExceptionThrown_ReturnsErrorMessage()
        {
            var favoriteId = Guid.NewGuid();

            _favoriteRecipeServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<FavoriteRecipe, bool>>>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.DeleteFavoriteRecipe(favoriteId) as JsonResult;

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(result.Value));
            var root = doc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            string message = root.GetProperty("message").GetString();

            Assert.AreEqual(false, success);
            Assert.AreEqual("Unexpected error", message);
        }


    }
}
