using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.StoreDetail;
using Food_Haven.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Food_Haven.UnitTest.Admin_RecentShops_Test
{
    [TestFixture]
    public class RecentShops_Test
    {
        private Mock<IStoreDetailService> _storeServiceMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IProductVariantService> _variantServiceMock;
        private Mock<IOrderDetailService> _orderDetailServiceMock;
        private Mock<IOrdersServices> _orderServiceMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _storeServiceMock = new Mock<IStoreDetailService>();
            _productServiceMock = new Mock<IProductService>();
            _variantServiceMock = new Mock<IProductVariantService>();
            _orderDetailServiceMock = new Mock<IOrderDetailService>();
            _orderServiceMock = new Mock<IOrdersServices>();

            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            var hubContextMock = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<Food_Haven.Web.Hubs.ChatHub>>();
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _controller = new AdminController(
                _userManagerMock.Object,
                new Mock<BusinessLogic.Services.TypeOfDishServices.ITypeOfDishService>().Object,
                new Mock<BusinessLogic.Services.IngredientTagServices.IIngredientTagService>().Object,
                _storeServiceMock.Object,
                new Mock<AutoMapper.IMapper>().Object,
                new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>().Object,
                new Mock<BusinessLogic.Services.BalanceChanges.IBalanceChangeService>().Object,
                new Mock<BusinessLogic.Services.Categorys.ICategoryService>().Object,
                null,
                new Mock<BusinessLogic.Services.Complaints.IComplaintServices>().Object,
                _orderDetailServiceMock.Object,
                _orderServiceMock.Object,
                _variantServiceMock.Object,
                new Mock<BusinessLogic.Services.ComplaintImages.IComplaintImageServices>().Object,
                _storeServiceMock.Object,
                _productServiceMock.Object,
                new Mock<BusinessLogic.Services.VoucherServices.IVoucherServices>().Object,
                new Mock<BusinessLogic.Services.RecipeServices.IRecipeService>().Object,
                new Mock<BusinessLogic.Services.StoreReports.IStoreReportServices>().Object,
                new Mock<BusinessLogic.Services.StoreReports.IStoreReportServices>().Object,
                new Mock<BusinessLogic.Services.ProductImages.IProductImageService>().Object,
                new Mock<BusinessLogic.Services.RecipeIngredientTagIngredientTagServices.IRecipeIngredientTagIngredientTagSerivce>().Object,
                roleManagerMock.Object,
                new Mock<BusinessLogic.Services.ExpertRecipes.IExpertRecipeServices>().Object,
                hubContextMock.Object
            );
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

    }
}