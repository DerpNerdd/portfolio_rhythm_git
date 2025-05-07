// src/components/MobileVersion.jsx
import React, { useState, useEffect } from 'react';
import './CSS/MobileLandingScreen.css';
import SkillsPage from './SkillsPage';
import ProjectsPage from './ProjectsPage';
import AboutMePage from './AboutMePage';

export default function MobileVersion() {
const [vw, setVw] = useState(window.innerWidth);
const [view, setView] = useState('circle'); 
const [clicked, setClicked] = useState(false);

useEffect(() => {
    const handleResize = () => setVw(window.innerWidth);
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
}, []);

const size = Math.min(Math.max(vw * 0.5, 100), 200);

const handleCircleClick = () => {
    setClicked(true);
    setView('menu');
};
const handleBack = () => {
    setClicked(false);
    setView('circle');
};
const handleSkills = () => setView('skills');
const handleProjects = () => setView('projects');
const handleAbout = () => setView('about');

if (view === 'skills') {
    return <SkillsPage onBack={handleBack} />;
}

if (view === 'projects') {
    return <ProjectsPage onBack={handleBack} />;
}

if (view === 'about') {
    return <AboutMePage onBack={handleBack} />;
}

return (
    <div className="mobile-landing">
    <audio src="/audio/cYsmix - Triangles.mp3" autoPlay loop />

    <img
        src="/images/Background.png"
        alt="Mobile Background"
        className="mobile-bg"
    />

    {(view === 'circle' || view === 'menu') && (
        <div
        className={`circle-container${clicked ? ' clicked' : ''}`}
        onClick={handleCircleClick}
        >
        <img
            src="/images/Main Circle.png"
            alt="Play Circle"
            className="play-circle"
            style={{ width: size, height: size }}
        />
        </div>
    )}

    {view === 'menu' && (
        <div className="rect-container">
        <button className="rect rect1">
            <span className="rect-text">
            Visit the website on computer to experience the “Play” portion
            </span>
        </button>

        <button className="rect rect2" onClick={handleProjects}>
            <img
            src="/images/clipboard.png"
            alt="Projects"
            className="rect-img"
            />
            <span className="rect-label">Projects</span>
        </button>

        <button className="rect rect3" onClick={handleAbout}>
            <img
            src="/images/person.png"
            alt="About Me"
            className="rect-img"
            />
            <span className="rect-label">About Me</span>
        </button>

        <button className="rect rect4" onClick={handleSkills}>
            <img
            src="/images/shield.png"
            alt="Skills"
            className="rect-img"
            />
            <span className="rect-label">Skills</span>
        </button>
        </div>
    )}
    </div>
);
}
