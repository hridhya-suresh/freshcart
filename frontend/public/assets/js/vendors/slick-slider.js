(function () {
  if (typeof $ === 'undefined' || !$.fn.slick) {
    return;
  }

  var prevArrow =
    '<button class="slick-prev slick-arrow" aria-label="Previous" type="button">' +
    '<i class="feather-icon icon-chevron-left"></i></button>';
  var nextArrow =
    '<button class="slick-next slick-arrow" aria-label="Next" type="button">' +
    '<i class="feather-icon icon-chevron-right"></i></button>';

  if ($('.hero-slider').length) {
    $('.hero-slider').slick({
      dots: true,
      infinite: true,
      speed: 300,
      slidesToShow: 1,
      slidesToScroll: 1,
      autoplay: true,
      autoplaySpeed: 5000,
      arrows: false,
    });
  }

  if ($('.product-slider').length) {
    $('.product-slider').slick({
      slidesToShow: 6,
      slidesToScroll: 1,
      autoplay: true,
      autoplaySpeed: 2000,
      infinite: true,
      dots: false,
      arrows: true,
      prevArrow: prevArrow,
      nextArrow: nextArrow,
      responsive: [
        { breakpoint: 1400, settings: { slidesToShow: 4, slidesToScroll: 1 } },
        { breakpoint: 992, settings: { slidesToShow: 3, slidesToScroll: 1 } },
        { breakpoint: 768, settings: { slidesToShow: 2, slidesToScroll: 1, arrows: false } },
        { breakpoint: 480, settings: { slidesToShow: 1, slidesToScroll: 1, arrows: false } },
      ],
    });
  }
})();
