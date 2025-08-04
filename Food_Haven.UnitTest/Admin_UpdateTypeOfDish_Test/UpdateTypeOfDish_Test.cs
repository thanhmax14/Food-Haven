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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Models.DBContext;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_UpdateTypeOfDish_Test
{
    [TestFixture]
    public class UpdateTypeOfDish_Test
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
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

      

        [Test]
        public async Task UpdateTypeOfDish_ReturnsView_WhenNameExists()
        {
            var model = new TypeOfDishUpdateViewModel { Name = "Trung", IsActive = true, ID = Guid.NewGuid() };
            _typeOfDishServiceMock.Setup(x => x.ExistsAsync(model.Name, model.ID)).ReturnsAsync(true);

            var result = await _controller.UpdateTypeOfDish(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(_controller.TempData["SwalError"], Is.EqualTo("The dish type name already exists."));
        }


        [Test]
        public async Task UpdateTypeOfDish_Redirects_WhenSuccess()
        {
            var model = new TypeOfDishUpdateViewModel { Name = "MonMoi", IsActive = true, ID = Guid.NewGuid() };
            var entity = new TypeOfDish { ID = model.ID, Name = "OldName", IsActive = false, CreatedDate = DateTime.Now };
            _typeOfDishServiceMock.Setup(x => x.ExistsAsync(model.Name, model.ID)).ReturnsAsync(false);
            _typeOfDishServiceMock.Setup(x => x.GetAsyncById(model.ID)).ReturnsAsync(entity);
            _typeOfDishServiceMock.Setup(x => x.UpdateAsync(entity)).Returns(Task.CompletedTask);
            _typeOfDishServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _controller.UpdateTypeOfDish(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("GetAllTypeOfDish"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(model.ID));
            Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo("Dish type has been updated successfully!"));
        }

        [Test]
        public async Task UpdateTypeOfDish_ReturnsNotFound_WhenIdIsInvalid()
        {
            var model = new TypeOfDishUpdateViewModel { Name = "MonMoi", IsActive = true, ID = Guid.Empty };
            _typeOfDishServiceMock.Setup(x => x.ExistsAsync(model.Name, model.ID)).ReturnsAsync(false);
            _typeOfDishServiceMock.Setup(x => x.GetAsyncById(model.ID)).ReturnsAsync((TypeOfDish)null);

            var result = await _controller.UpdateTypeOfDish(model);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
