using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using System;
using System.Linq;

namespace Food_Haven.UnitTest.Admin_CheckNameExistsForUpdate_Test
{
    public class CheckNameExistsForUpdate_Test
    {
        private Mock<ICategoryService> _categoryServiceMock;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            // Truyền đúng 23 tham số, các tham số không dùng thì để null hoặc default
            _controller = new AdminController(
                null, // UserManager<AppUser>
                null, // ITypeOfDishService
                null, // IIngredientTagService
                null, // IStoreDetailService
                null, // IMapper
                null, // IWebHostEnvironment
                null, // IBalanceChangeService
                _categoryServiceMock.Object, // ICategoryService
                null, // ManageTransaction
                null, // IComplaintServices
                null, // IOrderDetailService
                null, // IOrdersServices
                null, // IProductVariantService
                null, // IComplaintImageServices
                null, // IStoreDetailService
                null, // IProductService
                null, // IVoucherServices
                null, // IRecipeService
                null, // IStoreReportServices
                null, // IStoreReportServices
                null, // IProductImageService
                null, // IRecipeIngredientTagIngredientTagSerivce
                null  // RoleManager<IdentityRole>
            );
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task CheckNameExistsForUpdate_ReturnsTrue_WhenCategoryExists()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var testName = "TestCategory";
            var categories = new List<Categories> { new Categories { ID = Guid.NewGuid() } };
            _categoryServiceMock
                .Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.CheckNameExistsForUpdate(testName, testId);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value.GetType().GetProperty("exists") != null
                ? jsonResult.Value
                : (object)new Dictionary<string, object>(jsonResult.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(jsonResult.Value)));
            bool exists = (bool)(data is IDictionary<string, object> dict ? dict["exists"] : data.GetType().GetProperty("exists").GetValue(data));
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task CheckNameExistsForUpdate_ReturnsFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var testName = "TestCategory";
            var categories = new List<Categories>();
            _categoryServiceMock
                .Setup(s => s.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Categories, bool>>>(), null, null))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.CheckNameExistsForUpdate(testName, testId);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var data = jsonResult.Value.GetType().GetProperty("exists") != null
                ? jsonResult.Value
                : (object)new Dictionary<string, object>(jsonResult.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(jsonResult.Value)));
            bool exists = (bool)(data is IDictionary<string, object> dict ? dict["exists"] : data.GetType().GetProperty("exists").GetValue(data));
            Assert.IsFalse(exists);
        }

        
    }
}
