// src/components/ProjectsPage.jsx
import React, { useEffect, useRef } from 'react';
import './CSS/ProjectsPage.css';

export default function ProjectsPage({ onBack }) {
  const canvasRef = useRef(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    let stars = [];

    function resize() {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
      initStars();
    }

    function initStars() {
      stars = [];
      const count = 300;
      for (let i = 0; i < count; i++) {
        stars.push({
          x: Math.random() * canvas.width,
          y: Math.random() * canvas.height,
          r: Math.random() * 1.5 + 0.2,
          alpha: Math.random(),
          speed: (Math.random() * 0.005) + 0.002
        });
      }
    }

    function animate() {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      for (const s of stars) {
        s.alpha += s.speed;
        if (s.alpha <= 0 || s.alpha >= 1) s.speed *= -1;
        ctx.beginPath();
        ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(255,255,255,${s.alpha})`;
        ctx.fill();
      }
      requestAnimationFrame(animate);
    }

    window.addEventListener('resize', resize);
    resize();
    animate();
    return () => window.removeEventListener('resize', resize);
  }, []);

  const projects = [
    { title: 'Personal GitHub',                         url: 'https://github.com/derpnerdd' },
    { title: 'Year 2 Portfolio Project',                url: 'https://github.com/DerpNerdd/portfolio_rhythm_git' },
    { title: 'Troop 747 Website',                       url: 'https://troop747.onrender.com' },
    { title: 'FBLA State Coding and Programming 2025',  url: 'https://github.com/sshafe928/cash-compass' },
    { title: 'FBLA State Mobile App Dev 2025',          url: 'https://github.com/DerpNerdd/fbla-mad-cordova-state' },
    { title: 'NextJS Year 1 Portfolio',                 url: 'https://github.com/DerpNerdd/portfolio_nextjs' },
    { title: 'FBLA Regionals Mobile App Dev 2025',      url: 'https://github.com/DerpNerdd/fbla-mad-cordova' },
    { title: 'React Decipher Game',                     url: 'https://react-decipher-app.onrender.com' },
    { title: 'FBLA State Coding and Programming 2024',  url: 'https://derpnerdd.github.io/fblacodingandprogrammingstate/public/' },
    { title: 'React Quiz App',                          url: 'https://reactquizizz.netlify.app' },
    { title: 'RidgeGPT',                                url: 'https://github.com/DerpNerdd/RidgeGPT' },
    { title: 'Pet Adoption Website',                    url: 'https://pet-adoption-webservice.onrender.com' },
    { title: 'Event Registration Website',              url: 'https://github.com/DerpNerdd/eventsejs' },
    { title: 'Rhythm Game API',                         url: 'https://github.com/DerpNerdd/MyAPI' },
    { title: 'Summer Website',                          url: 'https://derpnerdd.github.io/wbsummersite/' },
    { title: 'FBLA Regionals Coding & Programming 2024',url: 'https://derpnerdd.github.io/fblacodingandprogramming/HTML/login.html' },
    { title: 'Year 1 Portfolio Project',                url: 'https://derpnerdd.github.io/Portfolio-Project/' },
    { title: 'Single Page Java',                        url: 'https://derpnerdd.github.io/SinglePageJava/' },
    { title: 'Memory Card Game',                        url: 'https://derpnerdd.github.io/memorycardgame/' },
    { title: 'Responsive Gallery',                      url: 'https://derpnerdd.github.io/ResponsiveGalleryPage/' },
    { title: 'Tic Tac Toe',                             url: 'https://derpnerdd.github.io/tictactoe/' },
    { title: 'FBLA Regionals Web Coding & Dev 2024',    url: 'https://derpnerdd.github.io/fblawebcodingdevelopmentproj/' },
    { title: 'Christmas Website',                       url: 'https://derpnerdd.github.io/ChristmasWebsite/' },
    { title: 'Merge Unit Project',                      url: 'https://derpnerdd.github.io/endofunitproject/index.html' },
    { title: 'Business Website',                        url: 'https://derpnerdd.github.io/businesswebsitept1/' },
    { title: 'Vivid Loading',                           url: 'https://derpnerdd.github.io/vividloading/' },
    { title: 'First Website',                           url: 'https://derpnerdd.github.io/firstwebpage/' },
  ];

const title = "PROJECTS";


return (
<div className="projects-page">
    <audio src="/audio/bgmusic.mp3" autoPlay loop />

    <button className="back-button" onClick={onBack}>
    <img src="/images/blackbackbutton.png" alt="Back" className="back-icon" />
    </button>

    <canvas ref={canvasRef} className="star-canvas" />

    <h1 className="projects-title">
    {title.split('').map((char, i) => (
        <span key={i} style={{ '--i': i }}>{char}</span>
    ))}
    </h1>

    <ul className="projects-list">
    {projects.map(p => (
        <li key={p.url}>
        <a
            href={p.url}
            target="_blank"
            rel="noopener noreferrer"
            className="project-link"
        >
            {p.title}
        </a>
        </li>
    ))}
    </ul>
</div>
);
}