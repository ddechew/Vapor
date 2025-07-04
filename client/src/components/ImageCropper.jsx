import { useState, useCallback } from "react";

import Cropper from "react-easy-crop";
import getCroppedImg from "../utils/getCroppedImg"; 

import "../styles/ImageCropper.css";

const ImageCropper = ({ image, onCropDone, onCancel }) => {
  const [crop, setCrop] = useState({ x: 0, y: 0 });
  const [zoom, setZoom] = useState(1);
  const [croppedAreaPixels, setCroppedAreaPixels] = useState(null);

  const onCropComplete = useCallback((_, croppedAreaPixels) => {
    setCroppedAreaPixels(croppedAreaPixels);
  }, []);

  const handleCrop = async () => {
    const blob = await getCroppedImg(image, croppedAreaPixels);
    onCropDone(blob);
  };

  return (
    <div className="cropper-container">
      <div className="cropper">
        <Cropper
          image={image}
          crop={crop}
          zoom={zoom}
          aspect={1}
          onCropChange={setCrop}
          onZoomChange={setZoom}
          onCropComplete={onCropComplete}
        />
      </div>
      <div className="cropper-actions">
        <button onClick={handleCrop}>Crop & Upload</button>
        <button onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
};

export default ImageCropper;
