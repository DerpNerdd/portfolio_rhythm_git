import React, { useState, useEffect } from 'react';
import './MobileLandingScreen.css';

export default function MobileVersion() {
  const [vw, setVw] = useState(window.innerWidth);
  const [clicked, setClicked] = useState(false);

  useEffect(() => {
    const handleResize = () => setVw(window.innerWidth);
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const size = Math.min(Math.max(vw * 0.5, 100), 200);

  const handleClick = () => setClicked(true);

  return (
    <div className="mobile-landing">
      <img
        src="/images/Background.png"
        alt="Mobile Background"
        className="mobile-bg"
      />

      <div
        className={`circle-container${clicked ? ' clicked' : ''}`}
        onClick={handleClick}
      >
        <img
          src="/images/Main Circle.png"
          alt="Play Circle"
          className="play-circle"
          style={{ width: size, height: size }}
        />
      </div>

      {clicked && (
        <div className="rect-container">
          <button className="rect rect1">
            <span className="rect-text">Visit the website on computer to experience the “Play” portion </span>
          </button>
          <button className="rect rect2">
            <img src="/images/clipboard.png" alt="Icon 2" className="rect-img" />
            <span className="rect-label">projects</span>
          </button>
          <button className="rect rect3">
            <img src="/images/person.png" alt="Icon 3" className="rect-img" />
            <span className="rect-label">about me</span>
          </button>
          <button className="rect rect4">
            <img src="/images/shield.png" alt="Icon 4" className="rect-img" />
            <span className="rect-label">skills</span>
          </button>
        </div>
      )}
    </div>
  );
}