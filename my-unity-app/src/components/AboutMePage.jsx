// src/components/AboutMePage.jsx
import React from 'react';
import './CSS/AboutMePage.css';

export default function AboutMePage({ onBack }) {
  return (
    <div className="about-page">
      <button className="back-button" onClick={onBack}>
        <img
          src="/images/backbutton.png"
          alt="Back"
          className="back-icon"
        />
      </button>

      <div className="about-content">
        <h1 className="greeting">
          Hello! My Name is <span className="name-text">Alan Sanchez!</span>
        </h1>
        <h2 className="scroll-text">Scroll down to learn more :3</h2>
      </div>

      {/* future sections go here */}
    </div>
  );
}
