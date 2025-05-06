import React, { useState, useEffect } from 'react';

// Paths to your decorative images in public/
const LEFT_ART    = '/images/leftimage.jpg';
const RIGHT_ART   = '/images/rightimage.jpg';
const TOP_ART     = '/images/topimage.jpg';
const BOTTOM_ART  = '/images/bottomimage.jpg';

export default function Overlay({ children }) {
  const [dims, setDims] = useState({ sideWidth: 0, barHeight: 0 });

  useEffect(() => {
    function computeDims() {
      const w = window.innerWidth;
      const h = window.innerHeight;
      const desiredW = (h * 16) / 9;
      const desiredH = (w * 9) / 16;
      const sideWidth = Math.max((w - desiredW) / 2, 0);
      const barHeight = Math.max((h - desiredH) / 2, 0);
      setDims({ sideWidth, barHeight });
    }
    computeDims();
    window.addEventListener('resize', computeDims);
    return () => window.removeEventListener('resize', computeDims);
  }, []);

  return (
    <div
      style={{
        position: 'relative',
        width: '100vw',
        height: '100vh',
        overflow: 'hidden',
      }}
    >
      {/* Underlying game or content */}
      {children}

      {/* Left decorative overlay */}
      {dims.sideWidth > 0 && (
        <div
          style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: `${dims.sideWidth}px`,
            height: '100%',
            backgroundImage: `url(${LEFT_ART})`,
            backgroundRepeat: 'no-repeat',
            backgroundPosition: 'center',
            backgroundSize: 'cover',
            pointerEvents: 'none',
          }}
        />
      )}

      {/* Right decorative overlay */}
      {dims.sideWidth > 0 && (
        <div
          style={{
            position: 'absolute',
            top: 0,
            right: 0,
            width: `${dims.sideWidth}px`,
            height: '100%',
            backgroundImage: `url(${RIGHT_ART})`,
            backgroundRepeat: 'no-repeat',
            backgroundPosition: 'center',
            backgroundSize: 'cover',
            pointerEvents: 'none',
          }}
        />
      )}

      {/* Top decorative overlay */}
      {dims.barHeight > 0 && (
        <div
          style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            height: `${dims.barHeight}px`,
            backgroundImage: `url(${TOP_ART})`,
            backgroundRepeat: 'no-repeat',
            backgroundPosition: 'center',
            backgroundSize: 'cover',
            pointerEvents: 'none',
          }}
        />
      )}

      {/* Bottom decorative overlay */}
      {dims.barHeight > 0 && (
        <div
          style={{
            position: 'absolute',
            bottom: 0,
            left: 0,
            width: '100%',
            height: `${dims.barHeight}px`,
            backgroundImage: `url(${BOTTOM_ART})`,
            backgroundRepeat: 'no-repeat',
            backgroundPosition: 'center',
            backgroundSize: 'cover',
            pointerEvents: 'none',
          }}
        />
      )}
    </div>
  );
}
