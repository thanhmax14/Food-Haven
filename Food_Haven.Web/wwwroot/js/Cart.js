"use strict";

$(document).ready(function () {
  console.log("Document ready, cart.js loaded");

  // SignalR
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

  // Update stock display every 30s
  function updateStockDisplay() {
    var variantId = $("#selectedVariantId").val();
    if (!variantId) return;

    $.ajax({
      url: "/Home/GetVariantPrice",
      type: "GET",
      data: { variantId: variantId },
      dataType: "json",
      success: function (data) {
        if (data && data.stock != null && !isNaN(data.stock)) {
          $("#stock-display").text(data.stock);

          if (data.price != null && !isNaN(data.price)) {
            var formattedPrice = data.price.toLocaleString("vi-VN", {
              style: "currency",
              currency: "VND",
            });
            $("#price-display").text(formattedPrice);
          } else {
            console.warn("Invalid price:", data);
            $("#price-display").text("0 đ");
          }
        } else {
          console.warn("Invalid stock data:", data);
          $("#stock-display").text("0");
          $("#price-display").text("0 đ");
        }
      },
      error: function () {
        console.error("Error fetching data from server.");
        $("#stock-display").text("0");
        $("#price-display").text("0 đ");
      },
    });
  }

  // Variant selection - changed selector to .variant-button
  $(document).on("click", ".variant-button", function (e) {
    e.preventDefault();

    console.log("Clicked variant button");

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

    // Update selected variant ID
    $("#selectedVariantId").val(variantId);

    // UI active selection
    $(".variant-button").removeClass("active");
    $(this).addClass("active");

    $.ajax({
      url: "/Home/GetVariantPrice",
      type: "GET",
      cache: false,
      data: { variantId: variantId },
      dataType: "json",
      success: function (result) {
        if (
          result &&
          result.price != null &&
          !isNaN(parseFloat(result.price))
        ) {
          var price = parseFloat(result.price);
          var formattedPrice = new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
            maximumFractionDigits: 0,
          }).format(price);

          $(".current-price.text-brand").text(formattedPrice);
          $("#stock-display").text(result.stock != null ? result.stock : "N/A");

          var targetSelector = $(
            '.variant-button[data-variantid="' + result.variantId + '"]'
          ).data("target");
          if (targetSelector) {
            $(targetSelector).text(formattedPrice);
          }

          if (window.stockInterval) clearInterval(window.stockInterval);
          window.stockInterval = setInterval(updateStockDisplay, 20000);
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

  // Add to cart (with validation)
  $(document).on("click", "#btnAddToCart", function () {
    var variantId = $("#selectedVariantId").val();
    if (!variantId) {
      new Notify({
        status: "error",
        title: "Variant not selected!",
        text: "Please select a variant before adding to cart.",
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

    // ✅ Find quantity input near the button
    var $container = $(this).closest(".product-detail, .product-area, .container, .product-box");
    var $qtyInput = $container.find(".qty-val");

    var quantity = parseInt($qtyInput.val());
    var maxStock = parseInt($qtyInput.data("max")) || 999;

    if (isNaN(quantity) || quantity < 1) {
      new Notify({
        status: "error",
        title: "Invalid quantity!",
        text: "Please enter a valid quantity (at least 1).",
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

    if (quantity > maxStock) {
      new Notify({
        status: "error",
        title: "Exceeded stock!",
        text: "You can only order up to " + maxStock + " items.",
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

    // Send correct quantity
    AddToCart(variantId, quantity);
  });

  // AJAX add to cart
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

  // Refresh cart
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
        $("#cart-container").html(data);
      },
      error: function (xhr, status, error) {
        console.error("CartPart error:", xhr.status, error);
        new Notify({
          status: "error",
          title: "Error!",
          text: xhr.responseJSON?.message || "Could not refresh cart.",
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

  // If no variant selected, trigger the first one
  if (!$("#selectedVariantId").val()) {
    var $firstVariant = $(".variant-button").first();
    if ($firstVariant.length > 0) {
      $firstVariant.trigger("click");
    }
  }

  // Clean up interval on page unload
  $(window).on("beforeunload", function () {
    if (window.stockInterval) {
      clearInterval(window.stockInterval);
    }
  });

$(document).on("click", ".qty-up, .qty-down", function (e) { 
    e.preventDefault(); 
 
    const $button = $(this); 
    const productTypeId = $button.data("id"); 
    if (!productTypeId) return; 
 
    const $input = $("input.qty-val[data-id='" + productTypeId + "']"); 
    if ($input.length === 0) return; 
 
    // ✅ Đọc giá trị hiện tại TRƯỚC khi blur
    const current = parseInt($input.val()) || 1; 
    const max = parseInt($input.data("max")) || 999; 

    const isIncrease = $button.hasClass("qty-up"); 
    const newQty = isIncrease ? current + 1 : current - 1; 

    console.log("Before: ", current); 

    if (newQty < 1) { 
      console.log("Reached min: 1"); 
      return; 
    } 
    if (newQty > max) { 
      console.log("Reached max: ", max); 
      return; 
    } 

    // ✅ Set giá trị mới TRƯỚC khi blur
    $input.val(newQty);
    console.log("After: ", newQty); 

    // ✅ Blur sau khi đã set giá trị
    $input.blur(); 

    // Log after 10s 
    setTimeout(() => { 
      console.log("✅ After 10 seconds: Current value is", $input.val()); 
    }, 10000); 
  }); 
 
  // Only allow numbers 
  $(document).on("input", ".qty-val", function () { 
    this.value = this.value.replace(/[^0-9]/g, ""); 
    console.log("User typed: ", this.value); 
  }); 
 
  // Press Enter to commit 
  $(document).on("keydown", ".qty-val", function (e) { 
    if (e.key === "Enter") { 
      this.blur(); 
    } 
  });
});
