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

namespace Food_Haven.UnitTest.Admin_ResolveComplaint_Test
{
    public class ResolveComplaint_Test
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
        public async Task ResolveComplaint_Accept_ReturnsSuccessJson()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var action = "Accept";
            var note = "Refund approved.";
            var complaint = new Complaint { ID = complaintId, Status = "Pending", OrderDetailID = Guid.NewGuid() };
            var orderDetail = new OrderDetail { ID = complaint.OrderDetailID, OrderID = Guid.NewGuid(), TotalPrice = 100, Status = "Pending" };
            var order = new Order { ID = orderDetail.OrderID, UserID = "user1", Status = "Pending", PaymentStatus = "Pending" };
            var balance = 500m;

            _complaintServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>()))
                .ReturnsAsync(complaint);
            _orderDetailMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>()))
                .ReturnsAsync(orderDetail);
            _orderMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);
            _balanceMock.Setup(x => x.GetBalance(order.UserID)).ReturnsAsync(balance);
            _orderDetailMock.Setup(x => x.UpdateAsync(orderDetail)).Returns(Task.CompletedTask);
            _balanceMock.Setup(x => x.AddAsync(It.IsAny<BalanceChange>())).Returns(Task.CompletedTask);
            _orderMock.Setup(x => x.UpdateAsync(order)).Returns(Task.CompletedTask);
            _orderDetailMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _balanceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _complaintServiceMock.Setup(x => x.UpdateAsync(complaint)).Returns(Task.CompletedTask);
            _complaintServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.ResolveComplaint(complaintId, action, note);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var value = json.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);
            Assert.IsTrue(success);
            Assert.AreEqual("The order has been refunded and cancelled successfully.", message);
        }

        [Test]
        public async Task ResolveComplaint_Reject_ReturnsSuccessJson()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var action = "Reject";
            var note = "Complaint rejected.";
            var complaint = new Complaint { ID = complaintId, Status = "Pending", OrderDetailID = Guid.NewGuid() };
            var orderDetail = new OrderDetail { ID = complaint.OrderDetailID, OrderID = Guid.NewGuid(), TotalPrice = 100, Status = "Pending" };
            var order = new Order { ID = orderDetail.OrderID, UserID = "user1", Status = "Pending", PaymentStatus = "Pending" };
            var balance = 500m;

            _complaintServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>()))
                .ReturnsAsync(complaint);
            _orderDetailMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>()))
                .ReturnsAsync(orderDetail);
            _orderMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);
            _balanceMock.Setup(x => x.GetBalance(order.UserID)).ReturnsAsync(balance);
            _orderDetailMock.Setup(x => x.UpdateAsync(orderDetail)).Returns(Task.CompletedTask);
            _balanceMock.Setup(x => x.AddAsync(It.IsAny<BalanceChange>())).Returns(Task.CompletedTask);
            _orderMock.Setup(x => x.UpdateAsync(order)).Returns(Task.CompletedTask);
            _orderDetailMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _balanceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _complaintServiceMock.Setup(x => x.UpdateAsync(complaint)).Returns(Task.CompletedTask);
            _complaintServiceMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.ResolveComplaint(complaintId, action, note);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var value = json.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);
            Assert.IsTrue(success);
            Assert.AreEqual("The complaint has been rejected successfully.", message);
        }

        [Test]
        public async Task ResolveComplaint_Reject_EmptyNote_ReturnsErrorJson()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var action = "Reject";
            var note = "";

            // Act
            var result = await _controller.ResolveComplaint(complaintId, action, note);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var value = json.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);
            Assert.IsFalse(success);
            Assert.IsTrue(message.ToLower().Contains("note") || message.ToLower().Contains("ghi chú") || message.ToLower().Contains("invalid"));
        }

        [Test]
        public async Task ResolveComplaint_InvalidGuid_ReturnsErrorJson()
        {
            // Act
            var result = await _controller.ResolveComplaint(Guid.Empty, "Accept", "Some note");

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var json = result as JsonResult;
            var value = json.Value;
            var type = value.GetType();
            var success = (bool)type.GetProperty("success")?.GetValue(value);
            var message = (string)type.GetProperty("message")?.GetValue(value);
            Assert.IsFalse(success);
            Assert.AreEqual("Invalid submission information.", message);
        }

    }
}
