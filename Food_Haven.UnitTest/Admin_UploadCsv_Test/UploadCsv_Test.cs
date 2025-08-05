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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.DBContext;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.BalanceChange;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_UploadCsv_Test
{
    public class UploadCsv_Test
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
        public async Task UploadCsv_ReturnsSuccess_WhenFileIsValid()
        {
            // Arrange: Tạo nội dung CSV hợp lệ
            var csvContent = new StringBuilder();
            csvContent.AppendLine("title,ingredients,directions,link,source,ner");

            var ingredientsJson = JsonConvert.SerializeObject(new[] { "Sugar" });   // ["Sugar"]
            var directionsJson = JsonConvert.SerializeObject(new[] { "Mix" });     // ["Mix"]
            var nerJson = JsonConvert.SerializeObject(new[] { "NER1" });           // ["NER1"]

            csvContent.AppendLine(
                $"\"New Recipe\"," +
                $"\"{ingredientsJson.Replace("\"", "\"\"")}\"," +
                $"\"{directionsJson.Replace("\"", "\"\"")}\"," +
                $"\"https://example.com\"," +
                $"\"Test Source\"," +
                $"\"{nerJson.Replace("\"", "\"\"")}\""
            );

            var fileName = "valid.csv";
            var fileContent = Encoding.UTF8.GetBytes(csvContent.ToString());
            var formFile = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };

            _expertRecipeServicesMock.Setup(x => x.GetAll())
                .Returns(new List<ExpertRecipe>().AsQueryable());

            _expertRecipeServicesMock.Setup(x => x.AddAsync(It.IsAny<ExpertRecipe>()))
                .Returns(Task.CompletedTask);

            _expertRecipeServicesMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var hubContextMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
            hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            typeof(AdminController).GetField("_hubContext", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_controller, hubContextMock.Object);

            // Act
            var result = await _controller.UploadCsv(formFile) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JObject.FromObject(result.Value);
            Console.WriteLine(json.ToString());

            Assert.IsTrue((bool)json["success"]);
            Assert.AreEqual(0, (int)json["totalErrors"]);
            Assert.AreEqual(1, (int)json["totalSuccess"]);

            var data = json["data"];
            Assert.IsNotNull(data);
            Assert.AreEqual("New Recipe", data[0]["Title"]?.ToString());
            Assert.AreEqual("[\"Sugar\"]", data[0]["Ingredients"]?.ToString());
            Assert.AreEqual("[\"Mix\"]", data[0]["Directions"]?.ToString());
            Assert.AreEqual("https://example.com", data[0]["Link"]?.ToString());
            Assert.AreEqual("Test Source", data[0]["Source"]?.ToString());
            Assert.AreEqual("[\"NER1\"]", data[0]["NER"]?.ToString());
        }



        [Test]
        public async Task UploadCsv_ReturnsError_WhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadCsv(null) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JObject.FromObject(result.Value);
            Assert.IsFalse((bool)json["success"]);
            Assert.AreEqual("Please select a CSV file.", (string)json["message"]);
        }
        [Test]
        public async Task UploadCsv_ReturnsError_WhenFileTooLarge()
        {
            var bigContent = new byte[101 * 1024 * 1024]; // >100MB
            var formFile = new FormFile(new MemoryStream(bigContent), 0, bigContent.Length, "file", "big.csv");

            // Act
            var result = await _controller.UploadCsv(formFile) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JObject.FromObject(result.Value);
            Assert.IsFalse((bool)json["success"]);
            Assert.AreEqual("The file exceeds the 100MB limit.", (string)json["message"]);
        }
        [Test]
        public async Task UploadCsv_ReturnsError_WhenHeaderIsInvalid()
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("wrong1,wrong2,wrong3,wrong4,wrong5,wrong6");

            var fileContent = Encoding.UTF8.GetBytes(csvContent.ToString());
            var formFile = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "file", "invalidHeader.csv");

            _expertRecipeServicesMock.Setup(x => x.GetAll())
                .Returns(new List<ExpertRecipe>().AsQueryable());

            // Act
            var result = await _controller.UploadCsv(formFile) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JObject.FromObject(result.Value);
            Assert.IsFalse((bool)json["success"]);
            Assert.AreEqual("The CSV headers are not in the required format.", (string)json["message"]);
        }

    }
}
