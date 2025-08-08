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
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Seller_Detailcomplant_Test
{
    public class Detailcomplant_Test
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
        public async Task Detailcomplant_ReturnsViewWithModel_WhenSuccess()
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "Shang12345" };
            var complaintId = Guid.NewGuid();
            var complaint = new Complaint
            {
                ID = complaintId,
                OrderDetailID = Guid.NewGuid(),
                Status = "Open",
                CreatedDate = DateTime.Now,
                AdminReply = "Admin reply",
                DateAdminReply = DateTime.Now,
                Reply = "Seller reply",
                ReplyDate = DateTime.Now,
                Description = "Complaint description",
                IsReportAdmin = true,
                AdminReportStatus = "Pending"
            };
            var orderDetail = new OrderDetail { ID = complaint.OrderDetailID, OrderID = Guid.NewGuid(), ProductTypesID = Guid.NewGuid() };
            var order = new Order { ID = orderDetail.OrderID, UserID = user.Id, OrderTracking = "ORD123" };
            var productType = new ProductTypes { ID = orderDetail.ProductTypesID, Name = "TypeA", ProductID = Guid.NewGuid() };
            var product = new Product { ID = productType.ProductID, Name = "ProductA", StoreID = Guid.NewGuid() };
            var store = new StoreDetails { ID = product.StoreID, Name = "StoreA" };
            var images = new List<ComplaintImage> { new ComplaintImage { ImageUrl = "img1.jpg", ComplaintID = complaintId } };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _complaintServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>())).ReturnsAsync(complaint);
            _orderDetailServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>())).ReturnsAsync(orderDetail);
            _ordersServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            _productVariantServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductTypes, bool>>>())).ReturnsAsync(productType);
            _productService2Mock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(product);
            _userManagerMock.Setup(x => x.FindByIdAsync(order.UserID)).ReturnsAsync(user);
            _storeDetailService2Mock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StoreDetails, bool>>>())).ReturnsAsync(store);
            _complaintImageServicesMock.Setup(x => x.ListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<ComplaintImage, bool>>>(),
                null,
                null)).ReturnsAsync(images);

            // Act
            var result = await _controller.Detailcomplant(complaintId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ComplantDetailViewmodels;
            Assert.IsNotNull(model);
            Assert.AreEqual(complaintId, model.ComplantID);
            Assert.AreEqual("ProductA", model.ProductName);
            Assert.AreEqual("TypeA", model.ProductType);
            Assert.AreEqual("StoreA", model.NameShop);
            Assert.AreEqual("Shang12345", model.UserName);
            Assert.AreEqual("ORD123", model.OrderTracking);
            Assert.AreEqual("img1.jpg", model.image[0]);
            Assert.IsTrue(model.IsreportAdmin);
            Assert.AreEqual("Pending", model.statusAdmin);
        }

        [Test]
        public async Task Detailcomplant_ReturnsNotFound_WhenIdInvalid()
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "Shang12345" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _complaintServiceMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Complaint, bool>>>())).ReturnsAsync((Complaint)null);

            // Act
            var result = await _controller.Detailcomplant(Guid.NewGuid());

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Complaint not found.", notFoundResult.Value);
        }
    }
}
