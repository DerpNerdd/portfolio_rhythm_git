// src/App.js
import React, { useState } from 'react';
import { isMobile } from 'react-device-detect';

import PlayScreen    from './components/PlayScreen';
import MobileVersion from './components/MobileVersion';
import Overlay       from './components/Overlay';
import UnityGame     from './components/UnityGame';

export default function App() {
  const [started, setStarted] = useState(false);

  // MOBILE FLOW: PlayScreen → MobileVersion
  if (isMobile) {
    return !started
      ? <PlayScreen onStart={() => setStarted(true)} />
      : <MobileVersion />;
  }

  // DESKTOP FLOW: PlayScreen → Overlay + UnityGame
  if (!started) {
    return <PlayScreen onStart={() => setStarted(true)} />;
  }

  return (
    <Overlay>
      <UnityGame />
    </Overlay>
  );
}
