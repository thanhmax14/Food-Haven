"use strict";

$(document).ready(function () {
  console.log("Document ready, cart.js loaded");

  // Kết nối SignalR
  var connection = new signalR.HubConnectionBuilder()
    .withUrl("/CartHub")
    .build();

  connection.on("ReceiveCartUpdate", function () {
    console.log("Received SignalR CartUpdate");
    refreshCart();
  });

  connection
    .start()
    .then(function () {
      console.log("SignalR connected successfully");
    })
    .catch(function (err) {
      console.error("SignalR error:", err.toString());
      refreshCart();
    });

  // Hàm cập nhật Stock tự động mỗi 10 giây
  function updateStockDisplay() {
    var variantId = $("#selectedVariantId").val();
    if (!variantId) return;

    $.ajax({
      url: "/Home/GetVariantPrice",
      type: "GET",
      data: { variantId: variantId },
      dataType: "json",
      success: function (result) {
        if (result && result.stock != null) {
          $("#stock-display").text(result.stock);
        }
      },
      error: function (xhr, status, error) {
        console.error("Auto stock update error:", xhr.status, error);
      },
    });
  }

  // Bắt sự kiện chọn variant
  $(document).on("click", ".variant-select", function (e) {
    e.preventDefault();

    var variantId = $(this).data("variantid");

    if (!variantId) {
      new Notify({
        status: "error",
        title: "Error!",
        text: "Cannot get variant ID.",
        effect: "fade",
        speed: 300,
        showIcon: true,
        showCloseButton: true,
        autoclose: true,
        autotimeout: 3000,
        position: "right top",
      });
      return;
    }

    $("#selectedVariantId").val(variantId);

    $.ajax({
      url: "/Home/GetVariantPrice",
      type: "GET",
      cache: false,
      data: { variantId: variantId },
      dataType: "json",
      success: function (result) {
        if (result && result.price != null && !isNaN(parseFloat(result.price))) {
          var price = parseFloat(result.price);
          var formattedPrice = new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
            maximumFractionDigits: 0,
          }).format(price);

          $(".current-price.text-brand").text(formattedPrice);
          $("#stock-display").text(result.stock != null ? result.stock : "N/A");

          var targetSelector = $('.variant-select[data-variantid="' + result.variantId + '"]').data("target");
          if (targetSelector) {
            $(targetSelector).text(formattedPrice);
          }

          // Bắt đầu cập nhật stock định kỳ 10s
          if (window.stockInterval) clearInterval(window.stockInterval);
          window.stockInterval = setInterval(updateStockDisplay, 10000);
        } else {
          new Notify({
            status: "error",
            title: "Error!",
            text: "Failed to retrieve price from server.",
            effect: "fade",
            speed: 300,
            showIcon: true,
            showCloseButton: true,
            autoclose: true,
            autotimeout: 3000,
            position: "right top",
          });
        }
      },
      error: function (xhr, status, error) {
        console.error("GetVariantPrice error:", xhr.status, error);
        new Notify({
          status: "error",
          title: "Server Error!",
          text: "An error occurred while getting price.",
          effect: "fade",
          speed: 300,
          showIcon: true,
          showCloseButton: true,
          autoclose: true,
          autotimeout: 3000,
          position: "right top",
        });
      },
    });
  });

  $(document).on("click", "#btnAddToCart", function () {
    var variantId = $("#selectedVariantId").val();

    if (!variantId) {
      new Notify({
        status: "error",
        title: "Warning!",
        text: "Please choose option before adding to cart.",
        effect: "fade",
        speed: 300,
        showIcon: true,
        showCloseButton: true,
        autoclose: true,
        autotimeout: 3000,
        position: "right top",
      });
      return;
    }

    var quantity = parseInt($(".qty-val").val()) || 1;
    AddToCart(variantId, quantity);
  });

  window.AddToCart = function (variantId, quantity) {
    $.ajax({
      url: "/Home/AddToCart",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({ ProductTypeID: variantId, quantity: quantity }),
      success: function (response) {
        new Notify({
          status: response.success ? "success" : "error",
          title: response.success ? "Success!" : "Error!",
          text: response.message,
          effect: "fade",
          speed: 300,
          showIcon: true,
          showCloseButton: true,
          autoclose: true,
          autotimeout: 3000,
          position: "right top",
        });

        if (response.success) {
          console.log("AddToCart success, refreshing cart");
          refreshCart();
        }
      },
      error: function (xhr, status, error) {
        console.error("AddToCart error:", xhr.status, error);
        new Notify({
          status: "error",
          title: "Server Error!",
          text: "An error occurred, please try again.",
          effect: "fade",
          speed: 300,
          showIcon: true,
          showCloseButton: true,
          autoclose: true,
          autotimeout: 3000,
          position: "right top",
        });
      },
    });
  };

  function refreshCart() {
    console.log("Refreshing cart");
    if ($("#cart-container").length === 0) {
      console.warn("Cart container not found in DOM");
      return;
    }

    $.ajax({
      url: "/Home/CartPart",
      type: "GET",
      cache: false,
      success: function (data) {
        console.log("CartPart loaded successfully:", data);
        $("#cart-container").html(data);
      },
      error: function (xhr, status, error) {
        console.error("CartPart error:", xhr.status, error);
        new Notify({
          status: "error",
          title: "Lỗi!",
          text: xhr.responseJSON?.message || "Không thể làm mới giỏ hàng.",
          effect: "fade",
          speed: 300,
          showIcon: true,
          showCloseButton: true,
          autoclose: true,
          autotimeout: 3000,
          position: "right top",
        });
      },
    });
  }

  // Dọn dẹp interval nếu rời trang
  $(window).on("beforeunload", function () {
    if (window.stockInterval) {
      clearInterval(window.stockInterval);
    }
  });
});
