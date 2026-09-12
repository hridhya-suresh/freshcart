type SlickElement = {
  hasClass(className: string): boolean;
  slick(options?: object | 'unslick'): void;
};

type SlickJQuery = {
  (selector: string | Element): SlickElement;
  fn: { slick?: unknown };
};

function getJQuery(): SlickJQuery | undefined {
  return (window as Window & { $?: SlickJQuery }).$;
}

const SLICK_ARROW_PREV =
  '<button class="slick-prev slick-arrow" aria-label="Previous" type="button">' +
  '<i class="feather-icon icon-chevron-left"></i></button>';
const SLICK_ARROW_NEXT =
  '<button class="slick-next slick-arrow" aria-label="Next" type="button">' +
  '<i class="feather-icon icon-chevron-right"></i></button>';

function initSlick(selector: string, options: object, root: HTMLElement): void {
  const jq = getJQuery();
  if (!jq?.fn?.slick) {
    return;
  }

  root.querySelectorAll(selector).forEach((element) => {
    const $el = jq(element);
    if (!$el.hasClass('slick-initialized')) {
      $el.slick(options);
    }
  });
}

export function initFreshcartSliders(root: HTMLElement = document.body): void {
  if (typeof window === 'undefined') {
    return;
  }

  initSlick(
    '.hero-slider',
    {
      dots: true,
      infinite: true,
      speed: 300,
      slidesToShow: 1,
      slidesToScroll: 1,
      autoplay: true,
      autoplaySpeed: 5000,
      arrows: false,
    },
    root,
  );

  initSlick(
    '.product-slider',
    {
      slidesToShow: 6,
      slidesToScroll: 1,
      autoplay: true,
      autoplaySpeed: 2000,
      infinite: true,
      dots: false,
      arrows: true,
      prevArrow: SLICK_ARROW_PREV,
      nextArrow: SLICK_ARROW_NEXT,
      responsive: [
        { breakpoint: 1400, settings: { slidesToShow: 4, slidesToScroll: 1 } },
        { breakpoint: 992, settings: { slidesToShow: 3, slidesToScroll: 1 } },
        { breakpoint: 768, settings: { slidesToShow: 2, slidesToScroll: 1, arrows: false } },
        { breakpoint: 480, settings: { slidesToShow: 1, slidesToScroll: 1, arrows: false } },
      ],
    },
    root,
  );
 initSlick(
    '.category-slider',
    {
      slidesToShow: 6,
      slidesToScroll: 1,
      autoplay: true,
      autoplaySpeed: 2000,
      infinite: true,
      dots: false,
      arrows: true,
      prevArrow: SLICK_ARROW_PREV,
      nextArrow: SLICK_ARROW_NEXT,
      responsive: [
        { breakpoint: 1400, settings: { slidesToShow: 4, slidesToScroll: 1 } },
        { breakpoint: 992, settings: { slidesToShow: 3, slidesToScroll: 1 } },
        { breakpoint: 768, settings: { slidesToShow: 2, slidesToScroll: 1, arrows: false } },
        { breakpoint: 480, settings: { slidesToShow: 1, slidesToScroll: 1, arrows: false } },
      ],
    },
    root,
  );
  initBootstrapTooltips(root);
}

export function destroyFreshcartSliders(root: HTMLElement = document.body): void {
  const jq = getJQuery();
  if (!jq?.fn?.slick) {
    return;
  }

  root.querySelectorAll('.slick-initialized').forEach((element) => {
    jq(element).slick('unslick');
  });
}

function initBootstrapTooltips(root: HTMLElement): void {
  const bootstrap = (window as Window & { bootstrap?: { Tooltip: new (el: Element) => void } })
    .bootstrap;
  if (!bootstrap?.Tooltip) {
    return;
  }

  root.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((el) => {
    new bootstrap.Tooltip(el);
  });
}
