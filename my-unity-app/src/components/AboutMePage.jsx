import React from 'react';
import './CSS/AboutMePage.css';

export default function AboutMePage({ onBack }) {
  return (
    <div className="about-page">
      <audio src="/audio/aboutme.mp3" autoPlay loop hidden />

      <button className="back-button" onClick={onBack}>
        <img src="/images/backbutton.png" alt="Go back" className="back-icon" />
      </button>

      <div className="about-content">
        <h1 className="greeting">
          Hello! My Name is <span className="name-text">Alan Sanchez!</span>
        </h1>
        <h2 className="scroll-text">Scroll down to learn more :3</h2>
      </div>

      <section className="about-section">
        <img
          src="/images/AboutMe/myeducation.png"
          alt="My Education"
          className="section-label"
        />
        <p className="section-text">
          I got my diploma at Mountain Ridge High School in Glendale, and got admitted into Purdue University for Mechanical Engineering Technology.
          <br /><br />
          I also enrolled at West-MEC Northeast Campus for their coding program, where I obtained many certifications in HTML, CSS, JavaScript, Python. I have also learned JSX, NextJS, NodeJS, and TypeScript.
        </p>
        <img
          src="/images/AboutMe/myeducation-large.png"
          alt=""
          className="section-large-image"
        />
      </section>

      {/* Section 3: The Game */}
      <section className="about-section">
        <img
          src="/images/AboutMe/thegame.png"
          alt="The Game"
          className="section-label"
        />
        <p className="section-text">
          If not obvious already, my portfolio is heavily inspired by the open-source, very popular rhythm game osu!Lazer!
          <br /><br />
          I’ve had an immense love for rhythm games for over 9 years, and I wanted to pour that passion into this portfolio for you all to enjoy!
        </p>
        <img
          src="/images/AboutMe/thegame-large.png"
          alt=""
          className="section-large-image"
        />
      </section>

      <section className="about-section">
        <img
          src="/images/AboutMe/personallife.png"
          alt="Personal Life"
          className="section-label"
        />
        <p className="section-text">
          I’m a web designer and full-stack developer in Arizona, though my first love has always been mechanical engineering.
          <br /><br />
          As a kid, I adored art, which naturally grew into a passion for web design—and now this portfolio. I love all things video games, music, and computers.
          <br /><br />
          Thank you to my friends and family for giving me the chance to live this insane dream.
        </p>
        <img
          src="/images/AboutMe/personallife-large.png"
          alt=""
          className="section-large-image"
        />
      </section>

      <div className="bottom-buttons">
        <button
          className="bottom-button"
          onClick={() =>
            window.scrollTo({ top: 0, behavior: 'smooth' })
          }
        >
          <img
            src="/images/AboutMe/backtotop.png"
            alt="Back to Top"
          />
        </button>
        <button className="bottom-button" onClick={onBack}>
          <img src="/images/AboutMe/goback.png" alt="Go back" />
        </button>
      </div>
    </div>
  );
}
