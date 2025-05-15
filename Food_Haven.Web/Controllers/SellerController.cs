<<<<<<< HEAD
﻿using System.Net.Http.Headers;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.OrderDetailService;
using BusinessLogic.Services.Orders;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository.StoreDetails;
using Repository.ViewModels;
=======
﻿using Microsoft.AspNetCore.Mvc;
>>>>>>> parent of ad1b828 (Merge branch 'main' into FixLogin)

namespace Food_Haven.Web.Controllers
{
    public class SellerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
<<<<<<< HEAD
        public async Task<IActionResult> FeedbackList()
        {
            var result = new List<ReivewViewModel>();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                var storeDetail = await _storeDetailService.FindAsync(s => s.UserID == user.Id);
                if (storeDetail == null)
                {
                    return View(result);
                }

                var storeId = storeDetail.ID;

                // Lấy danh sách sản phẩm theo StoreID
                var products = await _productService.ListAsync(p => p.StoreID == storeId);
                var productIds = products.Select(p => p.ID).ToList();

                // Lấy các review thuộc những sản phẩm này
                var reviews = await _reviewService.ListAsync(r => productIds.Contains(r.ProductID));

                foreach (var review in reviews)
                {
                    var reviewer = await _userManager.FindByIdAsync(review.UserID);
                    var product = products.FirstOrDefault(p => p.ID == review.ProductID);

                    var reviewModel = new ReivewViewModel
                    {
                        ID = review.ID,
                        Comment = review.Comment,
                        CommentDate = review.CommentDate,
                        Reply = review.Reply,
                        Status = review.Status,
                        Rating = review.Rating,
                        Username = reviewer?.UserName,
                        ProductName = product?.Name,
                        StoreId = storeId
                    };

                    result.Add(reviewModel);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = "There was an error loading Feedback List." });
            }

            return View(result);
        }

        //Review Details
        public async Task<IActionResult> ReplyFeedback(string id)
        {
            try
            {
                // Kiểm tra id có hợp lệ không
                if (!Guid.TryParse(id, out Guid reviewId))
                {
                    return Json(new { success = false, message = "Invalid ID." });
                }

                // Tìm review theo ReviewId
                var review = await _reviewService.FindAsync(r => r.ID == reviewId);

                if (review == null)
                {
                    return Json(new { success = false, message = "No reviews found." });
                }

                // Lấy thông tin người dùng
                var user = await _userManager.FindByIdAsync(review.UserID);
                if (user == null)
                {
                    return Json(new { success = false, message = "User does not exist." });
                }

                // Lấy thông tin sản phẩm
                var product = await _productService.GetAsyncById(review.ProductID);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product does not exist." });
                }

                // Tạo ViewModel để hiển thị trong View
                var reviewModel = new ReivewViewModel
                {
                    ID = review.ID,
                    Username = user.UserName,
                    ProductName = product.Name,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CommentDate = review.CommentDate,
                    Reply = review.Reply,
                    Status = review.Status,
                    UserID = review.UserID,
                    ProductID = review.ProductID
                };

                return View(reviewModel);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug sau này
                Console.WriteLine($"Error: {ex.Message}");

                // Trả về lỗi JSON để tránh chết chương trình
                return Json(new { success = false, message = "An error occurred, please try again later." });
            }
        }

        //update reply
        [HttpPost]
        public async Task<IActionResult> ReplyFeedback(ReivewViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Reply))
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(model.ID);

                if (review == null)
                {
                    return Json(new { success = false, message = "No reviews found!" });
                }

                // Cập nhật phản hồi
                review.Reply = model.Reply;
                review.ReplyDate = DateTime.Now;

                // Lưu thay đổi
                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Redirect("/Seller/FeedbackList");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Show(string id)
        {
            if (!Guid.TryParse(id, out Guid reviewId))
            {
                return BadRequest(new ErroMess { success = false, msg = "Invalid ID." });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(reviewId);

                if (review == null)
                {
                    return Json(new ErroMess { success = false, msg = "No reviews found!" });
                }

                // Cập nhật trạng thái từ ẩn sang hiện
                review.Status = false; // từ ẩn -> hiện

                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "System error: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hidden(string id)
        {
            if (!Guid.TryParse(id, out Guid reviewId))
            {
                return BadRequest(new ErroMess { success = false, msg = "Invalid ID." });
            }

            try
            {
                var review = await _reviewService.GetAsyncById(reviewId);

                if (review == null)
                {
                    return Json(new ErroMess { success = false, msg = "No reviews found!" });
                }

                // Cập nhật trạng thái từ ẩn sang hiện
                review.Status = true; // từ hiện -> ẩn

                await _reviewService.UpdateAsync(review);
                await _reviewService.SaveChangesAsync();

                return Json(new ErroMess { success = true, msg = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErroMess { success = false, msg = "System error: " + ex.Message });
            }
        }
=======
>>>>>>> parent of ad1b828 (Merge branch 'main' into FixLogin)
    }
}