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
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_ReplyFeedback_Test
{
    public class ReplyFeedback_Test
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
        public async Task ReplyFeedback_ValidId_ReturnsRedirect()
        {
            // Arrange
            var reviewId = Guid.Parse("b133a995-2268-4a5b-893a-0c93feab60b3");
            var model = new ReivewViewModel
            {
                ID = reviewId,
                Reply = "Thank you so much"
            };

            var review = new Review { ID = reviewId, Comment = "Good", CommentDate = DateTime.Now, UserID = "user1" };

            _reviewServiceMock.Setup(x => x.GetAsyncById(reviewId)).ReturnsAsync(review);

            // Act
            var result = await _controller.ReplyFeedback(model);

            // Assert
            var redirect = result as RedirectResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Seller/FeedbackList", redirect.Url);
            _reviewServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Review>()), Times.Once);
            _reviewServiceMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        [Test]
        public async Task ReplyFeedback_InvalidId_ReturnsJsonError()
        {
            // Arrange
            var reviewId = Guid.Parse("b133a995-2268-4a5b-893a-0c93feab6abc");
            var model = new ReivewViewModel
            {
                ID = reviewId,
                Reply = "Thank you so much"
            };

            _reviewServiceMock.Setup(x => x.GetAsyncById(reviewId)).ReturnsAsync((Review)null);

            // Act
            var result = await _controller.ReplyFeedback(model);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult.Value.GetType().GetProperty("message")?.GetValue(jsonResult.Value)?.ToString();
            Assert.AreEqual("No reviews found!", data);
        }
        [Test]
        public async Task ReplyFeedback_ExceptionThrown_Returns500Error()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var model = new ReivewViewModel
            {
                ID = reviewId,
                Reply = "Thank you so much"
            };

            _reviewServiceMock.Setup(x => x.GetAsyncById(reviewId)).ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.ReplyFeedback(model);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var message = objectResult.Value.GetType().GetProperty("message")?.GetValue(objectResult.Value)?.ToString();
            Assert.IsTrue(message.Contains("An error occurred"));
        }


    }
}
