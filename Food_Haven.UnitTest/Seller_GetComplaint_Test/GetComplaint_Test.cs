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
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Repository.BalanceChange;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static NUnit.Framework.Internal.OSPlatform;
using MockQueryable;

namespace Food_Haven.UnitTest.Seller_GetComplaint_Test
{
    [TestFixture]
    public class GetComplaint_Test
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

        // Test case 1: Normal - Seller logged in, data exists, returns complaint list
        [Test]  
        public async Task GetComplaint_ReturnsListOfComplaints_WhenUserIsSellerAndDataExists()
        {
            // Arrange
            var user = new AppUser { Id = "seller1", UserName = "seller" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var store = new StoreDetails { ID = Guid.NewGuid(), UserID = user.Id };
            _storeDetailService2Mock.Setup(s => s.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            // Product
            var product = new Product { ID = Guid.NewGuid(), StoreID = store.ID };
            _productService2Mock.Setup(p => p.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<Product> { product });

            // ProductType
            var productType = new ProductTypes { ID = Guid.NewGuid(), ProductID = product.ID };
            _productVariantServiceMock.Setup(v => v.ListAsync(
                It.IsAny<Expression<Func<ProductTypes, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<ProductTypes> { productType });

            // OrderDetail
            var orderDetail = new OrderDetail { ID = Guid.NewGuid(), ProductTypesID = productType.ID, OrderID = Guid.NewGuid() };
            _orderDetailServiceMock.Setup(o => o.ListAsync(
                It.IsAny<Expression<Func<OrderDetail, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<OrderDetail> { orderDetail });

            // Order
            var order = new Order { ID = orderDetail.OrderID, UserID = user.Id, OrderTracking = "ORD123" };
            _ordersServiceMock.Setup(o => o.ListAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(),
                null))
                .ReturnsAsync(new List<Order> { order });

            // Complaint
            var complaint = new Complaint
            {
                ID = Guid.NewGuid(),
                OrderDetailID = orderDetail.ID,
                Description = "Test complaint",
                Status = "Open",
                Reply = "Seller reply",
                AdminReply = "Admin reply",
                CreatedDate = DateTime.Now
            };
            _complaintServiceMock.Setup(c => c.ListAsync(
                It.IsAny<Expression<Func<Complaint, bool>>>(),
                null,
                null))
                .ReturnsAsync(new List<Complaint> { complaint });

            // Users
            var users = new List<AppUser> { user };
            var mockUsers = users.AsQueryable().BuildMock();
            _userManagerMock.Setup(m => m.Users).Returns(mockUsers);

            // Act
            var result = await _controller.GetComplaint() as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<GetComplaintViewModel>>(result.Value);
            var complaintList = (List<GetComplaintViewModel>)result.Value;
            Assert.IsTrue(complaintList.Count > 0);
            Assert.AreEqual("seller", complaintList[0].UserName);
        }

        // Test case 2: Abnormal - User not logged in, returns error message
        [Test]
        public async Task GetComplaint_ReturnsErrorMessage_WhenUserIsNull()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _controller.GetComplaint() as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErroMess>(result.Value);
            var erro = (ErroMess)result.Value;
            Assert.AreEqual("You are not logged in!", erro.msg);
        }
    }
}
