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
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Threading;
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

namespace Food_Haven.UnitTest.Admin_GetComplaint_Test
{
    [TestFixture]
    public class GetComplaint_Test
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
        public async Task GetComplaint_ReturnsEmptyList_WhenNoComplaintsExist()
        {
            // Arrange
            _complaintServiceMock.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>(), null, null))
                .ReturnsAsync(new List<Complaint>());

            // Act
            var result = await _controller.GetComplaint();

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOf<List<GetComplaintViewModel>>(jsonResult.Value);
            Assert.IsEmpty((List<GetComplaintViewModel>)jsonResult.Value);
        }

        [Test]
        public async Task GetComplaint_ReturnsComplaintList_WhenDataExists()
        {
            // Arrange
            var complaintId = Guid.NewGuid();
            var orderDetailId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var userId = "admin";
            var complaintRaw = new List<Complaint>
            {
                new Complaint {
                    ID = complaintId,
                    OrderDetailID = orderDetailId,
                    Description = "desc",
                    Status = "Pending",
                    Reply = "Seller reply",
                    AdminReply = "Admin reply",
                    CreatedDate = DateTime.Now,
                    ReplyDate = DateTime.Now,
                    AdminReportStatus = "Pending",
                    DateAdminReply = DateTime.Now,
                    IsReportAdmin = true
                }
            };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { ID = orderDetailId, OrderID = orderId }
            };
            var orders = new List<Order>
            {
                new Order { ID = orderId, UserID = userId, OrderTracking = "ORD123" }
            };
            var users = new List<AppUser>
            {
                new AppUser { Id = userId, UserName = "admin" }
            };
            _complaintServiceMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Complaint, bool>>>(), null, null))
                .ReturnsAsync(complaintRaw);
            _orderDetailMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);
            _orderMock.Setup(x => x.ListAsync(It.IsAny<Expression<Func<Order, bool>>>(), null, null))
                .ReturnsAsync(orders);
            _userManagerMock.Setup(x => x.Users).Returns(new TestAsyncEnumerable<AppUser>(users));
            // Act
            var result = await _controller.GetComplaint();
            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            var complaintList = jsonResult.Value as List<GetComplaintViewModel>;
            Assert.IsNotNull(complaintList);
            Assert.AreEqual(1, complaintList.Count);
            Assert.AreEqual("ORD123", complaintList[0].OrderCode);
            Assert.AreEqual("admin", complaintList[0].UserName);
            Assert.AreEqual("desc", complaintList[0].Description);
        }

        // Helper classes for async queryable/enumerable
        public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);
            public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                // The correct signature requires TResult to be a Task<T>, so we need to wrap the result in a Task if it's not already
                var result = Execute<TResult>(expression);
                if (result is Task<TResult> taskResult)
                {
                    return await taskResult;
                }
                return await Task.FromResult(result);
            }

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => (IQueryProvider)new TestAsyncQueryProvider<T>(this);
        }
        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }
    }
}
