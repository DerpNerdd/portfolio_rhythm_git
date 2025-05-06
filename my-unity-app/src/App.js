import React, { useState } from 'react';
import { isMobile } from 'react-device-detect';
import Overlay from './components/Overlay';
import UnityGame from './components/UnityGame';
import MobileVersion from './components/MobileVersion';
import PlayScreen from './components/PlayScreen';

export default function App() {
  const [started, setStarted] = useState(false);

  // Always show mobile version on small devices
  if (isMobile) {
    return <MobileVersion />;
  }

  // Display a custom Play screen to unlock audio
  if (!started) {
    return <PlayScreen onStart={() => setStarted(true)} />;
  }

  // Once started, show the Unity game with decorative overlays
  return (
    <Overlay>
      <UnityGame />
    </Overlay>
  );
}
