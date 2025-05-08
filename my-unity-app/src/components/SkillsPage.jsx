import React, { useState } from 'react';
import './CSS/SkillsPage.css';

const skillData = [
{
    name: 'HTML',
    image: '1051277.png',
    description: `I’ve been learning HTML5 for 2 years now, with a certificate in Information Technology Specialist in HTML5 Application. I have created a multitude of websites and projects, many on a professional scale and some winning awards for their functionality.`,
},
{
    name: 'CSS',
    image: 'CSS3_logo.svg.png',
    description: `I’ve been learning CSS for 2 years now, with a certificate to go along with it. Along with HTML5, I infuse my websites with alot of CSS that matches my style. I also have minor experience with SASS (SCSS).`,
},
{
    name: 'JavaScript',
    image: 'javascript.png',
    description: `I’ve been learning JS for a year and a half now, with my Javascript certificate as well. I have done many projects that implement basic JS for functionality and NodeJS for backend support, having extensive experience in both. `,
},
{
    name: 'Raspberry Pi',
    image: 'raspberry-pi.svg',
    description: `The RaspberryPi was my first introduction to coding, starting when I was 9 years old. Tinkering with very basic object oriented programming gave me the passion to continue coding nearly a decade later.`,
},
{
    name: 'React',
    image: 'React-icon.svg.png',
    description: `I’ve been learning React for about a year now, and it has been an amazing language to use. Using it to replace HTML and invest time into learning it and Tailwind has led to great results, even placing in a state competition for it.`,
},
{
    name: 'TypeScript',
    image: 'typescript-icon-icon-2048x2048-2rhh1z66.png',
    description: `TypeScript is the latest language I have been learning so far. While I enjoy React more due to my experience with it, TypeScript has been shown to be easier and even more powerful, and I enjoy learning it everyday. `,
},
{
    name: 'C#',
    image: 'c-sharp-c-icon-1822x2048-wuf3ijab.png',
    description: `While I am not experience with C#, this portfolio was my first real introduction into it. While I have had experience with Object Oriented programming, this project allowed my vision to go far and beyond any of my expectations.`,
},
{
    name: 'GitHub',
    image: 'Octicons-mark-github.svg.png',
    description: `Github is my main place for all my projects and information about me. You can find my older projects, LinkedIn, and more on my github page. If you are ever in the mood to check it out, go to github.com/derpnerdd and take a look!`,
},
];

function SkillSlide({ name, image, description }) {
    return (
    <div className="skill-slide">
        <h1 className="skills-title">{name}</h1>
        <div className="skill-image-container">
        <img
            src={`/images/skills/${image}`}
            alt={name}
            className="skill-img"
        />
        </div>
        <div className="skill-description">{description}</div>
    </div>
    );
}

export default function SkillsPage({ onBack }) {
    const [idx, setIdx] = useState(0);
    const prev = () => setIdx(i => (i - 1 + skillData.length) % skillData.length);
    const next = () => setIdx(i => (i + 1) % skillData.length);

    return (
    <div className="skills-page">
        <audio src="/audio/Depression Shop.mp3" autoPlay loop />

        <button className="back-button" onClick={onBack}>
        <img
            src="/images/backbutton.png"
            alt="Back"
            className="back-icon"
        />
        </button>

        <div className="slider-container">
        <div
            className="skills-slider"
            style={{ transform: `translateX(-${idx * 100}%)` }}
        >
            {skillData.map(s => (
            <SkillSlide
                key={s.name}
                name={s.name}
                image={s.image}
                description={s.description}
            />
            ))}
        </div>
        </div>

        <div className="skills-nav">
        <button className="nav-button" onClick={prev}>
            <img src="/images/Skills/Arrowleft.png" alt="Previous" />
        </button>
        <button className="nav-button" onClick={next}>
            <img src="/images/Skills/Arrowright.png" alt="Next" />
        </button>
        </div>
    </div>
    );
}