using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.ComplaintImages;
using BusinessLogic.Services.Complaints;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.VoucherServices;
using Food_Haven.Web.Controllers;
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_FeedbackList_Test
{
    public class FeedbackList_Test
    {
        private SellerController _controller;

        // Các Mock Dependencies
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IStoreDetailService> _storeDetailService2Mock;
        private Mock<IProductService> _productService2Mock;
        private Mock<IVoucherServices> _voucherServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IComplaintImageServices> _complaintImageServicesMock;
        private Mock<IComplaintServices> _complaintServiceMock;
        private Mock<ManageTransaction> _manageTransactionMock;
        private Mock<IHubContext<ChatHub>> _hubContextMock;

        [SetUp]
        public void Setup()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _productServiceMock = new Mock<IProductService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _mapperMock = new Mock<IMapper>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _storeDetailService2Mock = new Mock<IStoreDetailService>();
            _productService2Mock = new Mock<IProductService>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _complaintImageServicesMock = new Mock<IComplaintImageServices>();
            _complaintServiceMock = new Mock<IComplaintServices>();
            _manageTransactionMock = new Mock<ManageTransaction>();
            _hubContextMock = new Mock<IHubContext<ChatHub>>();

            // Khởi tạo SellerController với các dependency mock
            _controller = new SellerController(
                _reviewServiceMock.Object,
                _userManagerMock.Object,
                _productServiceMock.Object,
                _storeDetailServiceMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _productVariantServiceMock.Object,
                _ordersServiceMock.Object,
                _balanceChangeServiceMock.Object,
                _orderDetailServiceMock.Object,
                _storeDetailService2Mock.Object,
                _productService2Mock.Object,
                _voucherServiceMock.Object,
                _productImageServiceMock.Object,
                _complaintImageServicesMock.Object,
                _complaintServiceMock.Object,
               null,
                _hubContextMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task FeedbackList_ReturnsListOfFeedback_WhenUserLoggedInAndDataExists() // UTCID01
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "seller1" };
            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            var products = new List<Product>
    {
        new Product { ID = Guid.NewGuid(), StoreID = store.ID, Name = "Product 1" }
    };
            var reviews = new List<Review>
    {
        new Review { ID = Guid.NewGuid(), ProductID = products[0].ID, UserID = "buyer1", Comment = "Good", CommentDate = DateTime.Now, Rating = 5 }
    };
            var reviewer = new AppUser { Id = "buyer1", UserName = "buyer1" };

            // Mock user
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);

            // Explicitly include all parameters, including optional ones
            _productServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                null, // orderBy
                null  // includeProperties
            )).ReturnsAsync(products);

            _reviewServiceMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                null, // orderBy
                null  // includeProperties
            )).ReturnsAsync(reviews);

            _userManagerMock.Setup(x => x.FindByIdAsync("buyer1")).ReturnsAsync(reviewer);

            // Act
            var result = await _controller.FeedbackList();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<ReivewViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Good", model[0].Comment);
        }

        [Test]
        public async Task FeedbackList_ReturnsErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "seller1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Gây lỗi để vào catch block
            _storeDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>())).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.FeedbackList();

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Ép kiểu object sang Dictionary để truy cập các thuộc tính
            var dict = jsonResult.Value.GetType()
                        .GetProperties()
                        .ToDictionary(p => p.Name, p => p.GetValue(jsonResult.Value));

            Assert.AreEqual("error", dict["status"]);
            Assert.AreEqual("There was an error loading Feedback List.", dict["msg"]);
        }


    }
}
