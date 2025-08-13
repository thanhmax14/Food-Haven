using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.ExpertRecipes;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.RecipeServices;
using BusinessLogic.Services.RecipeViewHistorys;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using BusinessLogic.Services.StoreFollowers;
using BusinessLogic.Services.StoreReports;
using BusinessLogic.Services.VoucherServices;
using BusinessLogic.Services.Wishlists;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Net.payOS;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Home_Invoice_Test
{
    public class Invoice_Test
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IRecipeService> _recipeServiceMock;
        private Mock<IStoreDetailService> _storeDetailServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private Mock<IWishlistServices> _wishlistServiceMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IProductImageService> _productImageServiceMock;
        private Mock<IProductVariantService> _productVariantServiceMock;
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<IBalanceChangeService> _balanceChangeServiceMock;
        private Mock<IOrdersServices> _ordersServiceMock;
        private Mock<IVoucherServices> _voucherServiceMock;
        private Mock<IStoreReportServices> _storeReportServiceMock;
        private Mock<IStoreFollowersService> _storeFollowersServiceMock;
        private Mock<IExpertRecipeServices> _expertRecipeServicesMock;
        private Mock<IRecipeViewHistoryServices> _recipeViewHistoryServicesMock;
        // Nếu muốn mock luôn RecipeSearchService, bạn cần tạo interface cho nó
        // private Mock<IRecipeSearchService> _recipeSearchServiceMock;

        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);
            _expertRecipeServicesMock = new Mock<IExpertRecipeServices>();
            _recipeViewHistoryServicesMock = new Mock<IRecipeViewHistoryServices>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _recipeServiceMock = new Mock<IRecipeService>();
            _storeDetailServiceMock = new Mock<IStoreDetailService>();
            _cartServiceMock = new Mock<ICartService>();
            _wishlistServiceMock = new Mock<IWishlistServices>();
            _productServiceMock = new Mock<IProductService>();
            _productImageServiceMock = new Mock<IProductImageService>();
            _productVariantServiceMock = new Mock<IProductVariantService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _balanceChangeServiceMock = new Mock<IBalanceChangeService>();
            _ordersServiceMock = new Mock<IOrdersServices>();
            _voucherServiceMock = new Mock<IVoucherServices>();
            _storeReportServiceMock = new Mock<IStoreReportServices>();
            _storeFollowersServiceMock = new Mock<IStoreFollowersService>();
            var hubContextMock = new Mock<IHubContext<ChatHub>>();

            // Nếu RecipeSearchService cần được mock, bạn nên refactor thành interface IRecipeSearchService và mock nó
            var recipeSearchService = new RecipeSearchService(""); // Hoặc dùng Mock<IRecipeSearchService>()

            var payOS = new PayOS("client-id", "api-key", "https://callback.url");

            _controller = new HomeController(
                _signInManagerMock.Object,
                _orderDetailServiceMock.Object,
                _recipeServiceMock.Object,
                _userManagerMock.Object,
                _categoryServiceMock.Object,
                _storeDetailServiceMock.Object,
                _emailSenderMock.Object,
                _cartServiceMock.Object,
                _wishlistServiceMock.Object,
                _productServiceMock.Object,
                _productImageServiceMock.Object,
                _productVariantServiceMock.Object,
                _reviewServiceMock.Object,
                _balanceChangeServiceMock.Object,
                _ordersServiceMock.Object,
                payOS,
                _voucherServiceMock.Object,
                _storeReportServiceMock.Object,
                _storeFollowersServiceMock.Object,
                recipeSearchService,
                 _expertRecipeServicesMock.Object, // <-- Add this argument
 _recipeViewHistoryServicesMock.Object,
                 hubContextMock.Object
            // <-- Add this argument
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        [Test]
        public async Task Invoice_WithValidOrder_ReturnsView_WithComputedTotals_AndItems()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var voucherId = Guid.NewGuid();
            var userId = "user-1";

            var order = new Order
            {
                ID = orderId,
                IsActive = true,
                UserID = userId,
                TotalPrice = 200_000m,
                VoucherID = voucherId,
                PaymentMethod = "Wallet",
                Status = "Paid",
                OrderTracking = "ORD-001",
                CreatedDate = DateTime.Now.AddDays(-1),
                ModifiedDate = DateTime.Now
            };

            var user = new AppUser
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "0123456789",
                Address = "123 Street"
            };

            var voucher = new Voucher
            {
                ID = voucherId,
                IsActive = true,
                Code = "SALE10",
                DiscountType = "Percent",
                DiscountAmount = 10 // 10%
            };

            var pt1Id = Guid.NewGuid();
            var pt2Id = Guid.NewGuid();

            var orderDetails = new List<OrderDetail>
    {
        new OrderDetail
        {
            ID = Guid.NewGuid(),
            OrderID = orderId,
            IsActive = true,
            ProductTypesID = pt1Id,
            ProductTypeName = null, // forces fallback to variant name
            ProductPrice = 50_000m,
            Quantity = 2
        },
        new OrderDetail
        {
            ID = Guid.NewGuid(),
            OrderID = orderId,
            IsActive = true,
            ProductTypesID = pt2Id,
            ProductTypeName = "Custom Name",
            ProductPrice = 100_000m,
            Quantity = 1
        }
    };

            var variant1 = new ProductTypes { ID = pt1Id, IsActive = true, Name = "Variant A" };
            var variant2 = new ProductTypes { ID = pt2Id, IsActive = true, Name = "Variant B" };

            // Mocks
            _ordersServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(order);

            _userManagerMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _voucherServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<Voucher, bool>>>()))
                .ReturnsAsync(voucher);

            _orderDetailServiceMock
                .Setup(s => s.ListAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>(), null, null))
                .ReturnsAsync(orderDetails);

            _productVariantServiceMock
                .Setup(s => s.FindAsync(It.Is<Expression<Func<ProductTypes, bool>>>(expr =>
                    expr.Compile().Invoke(variant1))))
                .ReturnsAsync(variant1);

            _productVariantServiceMock
                .Setup(s => s.FindAsync(It.Is<Expression<Func<ProductTypes, bool>>>(expr =>
                    expr.Compile().Invoke(variant2))))
                .ReturnsAsync(variant2);

            // Act
            var result = await _controller.Invoice(orderId.ToString());

            // Assert
            var view = result as ViewResult;
            Assert.IsNotNull(view, "Should return ViewResult");

            var model = view.Model as InvoiceViewModels;
            Assert.IsNotNull(model, "Model should be InvoiceViewModels");

            // Header fields
            Assert.AreEqual(order.OrderTracking, model.orderCoce);
            Assert.AreEqual(order.PaymentMethod, model.paymentMethod);
            Assert.AreEqual(order.Status, model.status);
            Assert.AreEqual($"{user.FirstName} {user.LastName}", model.NameUse);
            Assert.AreEqual(user.Email, model.emailUser);
            Assert.AreEqual(user.PhoneNumber, model.phoneUser);
            Assert.AreEqual(user.Address, model.AddressUse);
            Assert.AreEqual("SALE10", model.vocherName);

            // Voucher math: 10% of 200,000 = 20,000; subtotal = 180,000
            Assert.AreEqual(20_000m, model.discountVocher);
            Assert.AreEqual(180_000m, model.subtotal);

            // Items
            Assert.AreEqual(2, model.itemList.Count);
            // First item: name falls back to variant name
            var item1 = model.itemList[0];
            Assert.AreEqual("Variant A", item1.nameItem);
            Assert.AreEqual(2, item1.quantity);
            Assert.AreEqual(50_000m, item1.unitPrice);
            Assert.AreEqual(100_000m, item1.amount);

            // Second item: uses ProductTypeName override
            var item2 = model.itemList[1];
            Assert.AreEqual("Custom Name", item2.nameItem);
            Assert.AreEqual(1, item2.quantity);
            Assert.AreEqual(100_000m, item2.unitPrice);
            Assert.AreEqual(100_000m, item2.amount);
        }

        [Test]
        public async Task Invoice_WithBalanceDeposit_ReturnsView_WithSingleItem()
        {
            // Arrange
            var balanceId = Guid.NewGuid();
            var userId = "user-2";

            // Make sure order path returns null → forces balance path
            _ordersServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync((Order)null!);

            var user = new AppUser
            {
                Id = userId,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                PhoneNumber = "0987654321",
                Address = "456 Avenue",
                UserName = "janedoe"
            };

            var balance = new BalanceChange
            {
                ID = balanceId,
                Display = true,
                UserID = userId,
                Method = "Deposit",
                Status = "Success",
                StartTime = DateTime.Now.AddMinutes(-30),
                DueTime = DateTime.Now,
                MoneyChange = 150_000m // positive/negative is normalized with Math.Abs in controller
            };

            _balanceChangeServiceMock
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<BalanceChange, bool>>>()))
                .ReturnsAsync(balance);

            _userManagerMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Invoice(balanceId.ToString());

            // Assert
            var view = result as ViewResult;
            Assert.IsNotNull(view, "Should return ViewResult");

            var model = view.Model as InvoiceViewModels;
            Assert.IsNotNull(model, "Model should be InvoiceViewModels");

            Assert.AreEqual(balanceId.ToString(), model.orderCoce);
            Assert.AreEqual("Deposit", model.paymentMethod);
            Assert.AreEqual("Success", model.status);
            Assert.AreEqual($"{user.FirstName} {user.LastName}", model.NameUse);
            Assert.AreEqual(user.Email, model.emailUser);
            Assert.AreEqual(user.PhoneNumber, model.phoneUser);
            Assert.AreEqual(user.Address, model.AddressUse);

            Assert.AreEqual(Math.Abs(balance.MoneyChange), model.subtotal);
            Assert.AreEqual(1, model.itemList.Count);
            var item = model.itemList.Single();
            Assert.That(item.nameItem, Does.Contain("Deposit to"));
            Assert.AreEqual(1, item.quantity);
            Assert.AreEqual(150_000m, item.unitPrice);
            Assert.AreEqual(150_000m, item.amount);
        }

        [Test]
        public async Task Invoice_WithInvalidId_RedirectsToNotFoundPage()
        {
            // Arrange
            var invalid = "not-a-guid";

            // Act
            var result = await _controller.Invoice(invalid);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect, "Should redirect");
            Assert.AreEqual("NotFoundPage", redirect.ActionName);
        }

    }
}
