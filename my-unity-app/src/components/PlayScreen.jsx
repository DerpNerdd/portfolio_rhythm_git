import React from 'react';

export default function PlayScreen({ onStart }) {
const buttonSize = 200; 

return (
    <div style={{
    width: '100vw',
    height: '100vh',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    background: '#000',
    position: 'relative',
    overflow: 'hidden'
    }}>
    <style>{`
        @keyframes breathing {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.1); }
        }
    `}</style>

    <img
        src="/images/play-background.jpg"
        alt="Play Background"
        style={{
        position: 'absolute',
        width: '100%',
        height: '100%',
        objectFit: 'cover',
        top: 0,
        left: 0,
        zIndex: 0,
        }}
    />

    <div style={{
        position: 'relative',
        zIndex: 1,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
    }}>
        <button
        onClick={onStart}
        style={{
            width: `${buttonSize}px`,
            height: `${buttonSize}px`,
            borderRadius: '50%',
            border: '4px solid #fff',
            background: '#E85B9B',
            color: '#fff',
            fontSize: '1.5rem',
            fontWeight: 'bold',
            cursor: 'pointer',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            outline: 'none',
            animation: 'breathing 3s ease-in-out infinite',
        }}
        >
        Play
        </button>
        <p style={{
        marginTop: '1rem',
        color: '#fff',
        fontSize: '0.8rem',
        fontStyle: 'italic',
        }}>
        Audio will play, you have been warned
        </p>
    </div>
    </div>
);
}
