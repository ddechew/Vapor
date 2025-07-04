import { useState, useEffect } from "react";

import Slider from "react-slick";

import "../styles/ThumbnailCarousel.css";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

const ThumbnailCarousel = ({ mediaItems = [], onMediaSelect, selectedMedia, currentIndex }) => {
  const [sliderRef, setSliderRef] = useState(null);

  useEffect(() => {
    if (sliderRef && sliderRef.slickGoTo) {
      sliderRef.slickGoTo(currentIndex); 
    }
  }, [currentIndex]);
  
  const settings = {
    infinite: true,
    speed: 500,
    slidesToShow: 5,
    slidesToScroll: 1,
    arrows: false,
    beforeChange: (_, next) => {
      if (mediaItems[next]) {
        onMediaSelect(mediaItems[next].url);
      }
    },
  };

  return (
    <div className="carousel-wrapper">
      <button className="carousel-arrow left" onClick={() => sliderRef?.slickPrev()}>
        &#10094;
      </button>

      <div className="carousel-inner">
        <Slider {...settings} ref={setSliderRef}>
          {mediaItems.map((item, index) => (
            <div key={index} className="thumbnail-container">
              <img
                src={item.thumbnail}
                alt={`Thumbnail ${index + 1}`}
                className={`thumbnail ${selectedMedia == item.url ? 'active' : ''}`}
                onClick={() => onMediaSelect(item.url)}
                draggable={false}
              />
            </div>
          ))}
        </Slider>
      </div>

      <button className="carousel-arrow right" onClick={() => sliderRef?.slickNext()}>
        &#10095;
      </button>
    </div>
  );
};

export default ThumbnailCarousel;
