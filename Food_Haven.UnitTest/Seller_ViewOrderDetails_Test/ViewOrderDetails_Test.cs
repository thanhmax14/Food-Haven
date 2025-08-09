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
using static NUnit.Framework.Internal.OSPlatform;

namespace Food_Haven.UnitTest.Seller_ViewOrderDetails_Test
{
    public class ViewOrderDetails_Test
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
        public async Task ViewOrderDetails_ShouldReturnView_WhenDataIsValid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var storeId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var productTypeId = Guid.NewGuid();

            // 1. Mock user đang đăng nhập
            var user = new AppUser
            {
                Id = "seller1",
                UserName = "seller",
                Email = "seller@example.com"
            };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // 2. Mock order
            var order = new Order
            {
                ID = orderId,
                OrderTracking = "TRACK123",
                UserID = "customer1",
                TotalPrice = 200m,
                Note = "Test Note",
                PaymentMethod = "COD",
                DeliveryAddress = "123 Street",
                Description = "Order placed"
            };
            _ordersServiceMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);

            // 3. Mock order details
            var orderDetails = new List<OrderDetail>
    {
        new OrderDetail
        {
            OrderID = orderId,
            ProductTypesID = productTypeId,
            ProductPrice = 100m,
            Quantity = 2,
            Status = "confirmed"
        }
    };
            _orderDetailServiceMock
                .Setup(x => x.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);

            // 4. Mock product types
            var productTypes = new List<ProductTypes>
    {
        new ProductTypes
        {
            ID = productTypeId,
            Name = "Type A",
            ProductID = productId
        }
    };
            _productVariantServiceMock
                .Setup(x => x.ListAsync(It.IsAny<Expression<Func<ProductTypes, bool>>>(), null, null))
                .ReturnsAsync(productTypes);

            // 5. Mock store (sử dụng đúng service mà controller gọi)
            var store = new StoreDetails
            {
                ID = storeId,
                UserID = "seller1",
                Name = "Test Store"
            };
            _storeDetailService2Mock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<StoreDetails, bool>>>()))
                .ReturnsAsync(store);

            // 6. Mock product của store
            var products = new List<Product>
    {
        new Product
        {
            ID = productId,
            StoreID = storeId,
            Name = "Test Product"
        }
    };
            _productService2Mock
                .Setup(x => x.ListAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, null))
                .ReturnsAsync(products);

            // 7. Mock customer
            var customer = new AppUser
            {
                Id = "customer1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "0123456789",
                UserName = "johndoe"
            };
            _userManagerMock
                .Setup(x => x.FindByIdAsync("customer1"))
                .ReturnsAsync(customer);

            // 8. Mock product image
            _productImageServiceMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<ProductImage, bool>>>()))
                .ReturnsAsync(new ProductImage
                {
                    ProductID = productId,
                    IsMain = true,
                    ImageUrl = "https://example.com/image.jpg"
                });

            // Act
            var result = await _controller.ViewOrderDetails("TRACK123");

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult?.Model, Is.Not.Null);
        }





        [Test]
        public async Task ViewOrderDetails_InvalidOrderId_ReturnsNotFound()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.ViewOrderDetails("");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFound = result as NotFoundObjectResult;
            Assert.AreEqual("Invalid Order ID", notFound.Value);
        }

        [Test]
        public async Task ViewOrderDetails_ExceptionThrown_ReturnsException()
        {
            // Arrange
            var user = new AppUser { Id = "seller1" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
_ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ThrowsAsync(new Exception("An unknown error occurred..."));
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.ViewOrderDetails("550EV9TKH"));
            Assert.That(ex.Message, Is.EqualTo("An unknown error occurred..."));
        }
    }
}
