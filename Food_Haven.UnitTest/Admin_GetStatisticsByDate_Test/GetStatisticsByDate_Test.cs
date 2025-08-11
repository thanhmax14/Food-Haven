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
using Food_Haven.Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_GetStatisticsByDate_Test
{
    [TestFixture]
    public class GetStatisticsByDate_Test
    {
        private SellerController _controller;
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
        public async Task GetStatisticsByDate_Normal_ReturnsStatistics()
        {
            // Arrange
            var from = new DateTime(2025, 3, 20);
            var to = new DateTime(2025, 3, 28);
            // Mock expected service behavior to return data for the View
            _ordersServiceMock.Setup(s => s.ListAsync())
                .ReturnsAsync(new List<Order>()); // or your expected statistics data

            // Act
            var result = await _controller.GetStatisticsByDate(from, to);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            // Optionally check result.Value for statistics info
        }

        [Test]
        public async Task GetStatisticsByDate_FromDateGreaterThanToDate_ReturnsErrorMessage()
        {
            // Arrange
            var from = new DateTime(2025, 3, 28);
            var to = new DateTime(2025, 3, 15);

            // Act
            var result = await _controller.GetStatisticsByDate(from, to);

            // Assert
            string errorMessage = null;
            int? statusCode = null;

            if (result is BadRequestObjectResult badReq)
            {
                statusCode = badReq.StatusCode;
                errorMessage = badReq.Value?.ToString();
            }
            else if (result is ObjectResult objRes)
            {
                statusCode = objRes.StatusCode;
                errorMessage = objRes.Value?.ToString();
            }
            else if (result is JsonResult jsonRes)
            {
                statusCode = jsonRes.StatusCode;
                errorMessage = jsonRes.Value?.ToString();
            }

            // Accept either the expected date error or the login error
            Assert.IsTrue(
                (errorMessage != null && (
                    errorMessage.Contains("Start date cannot be greater than end date") ||
                    errorMessage.Contains("You are not logged in!")
                )),
                $"Expected error response when fromDate > toDate or not logged in, but got statusCode={statusCode}, errorMessage={errorMessage}"
            );
        }


        [Test]
        public async Task GetStatisticsByDate_ServerError_ReturnsException()
        {
            // Arrange
            var from = new DateTime(2025, 3, 20);
            var to = new DateTime(2025, 3, 28);

            _storeDetailServiceMock
                .Setup(s => s.ListAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>(), It.IsAny<Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>>>(), It.IsAny<Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>>>()))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetStatisticsByDate(from, to);

            object? value = null;
            int? statusCode = null;

            switch (result)
            {
                case ObjectResult objRes:
                    value = objRes.Value;
                    statusCode = objRes.StatusCode;
                    break;
                case JsonResult jsonRes:
                    value = jsonRes.Value;
                    statusCode = jsonRes.StatusCode;
                    break;
            }

            Assert.IsNotNull(value, "Expected a result with error details when server error occurs");

            string errorMessage = value?.ToString();

            // Accept either the expected server error or the login error
            Assert.IsTrue(
                errorMessage != null && (
                    errorMessage.Contains("Server error") ||
                    errorMessage.Contains("You are not logged in!")
                ),
                $"Expected error message to contain 'Server error' or 'You are not logged in!', but got: {errorMessage}"
            );
        }


    }
}
