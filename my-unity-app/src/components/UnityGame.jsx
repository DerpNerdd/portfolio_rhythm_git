import React, { useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";

export default function UnityGame() {
  const { unityProvider, isLoaded, loadingProgression, unload } = useUnityContext({
    loaderUrl:       "/FinishedBuild/Build/FinishedBuild.loader.js",
    dataUrl:         "/FinishedBuild/Build/FinishedBuild.data",
    frameworkUrl:    "/FinishedBuild/Build/FinishedBuild.framework.js",
    codeUrl:         "/FinishedBuild/Build/FinishedBuild.wasm",
  });

  useEffect(() => {
    return () => {
      if (isLoaded && typeof unload === 'function') {
        const res = unload();
        if (res && typeof res.catch === 'function') {
          res.catch(() => {});
        }
      }
    };
  }, [isLoaded, unload]);

  return (
    <div style={{ width: "100vw", height: "100vh", position: "relative" }}>
      {!isLoaded && (
        <div style={{
          position: "absolute", top: 0, left: 0, right: 0, bottom: 0,
          display: "flex", alignItems: "center", justifyContent: "center",
          background: "#000", color: "#fff", overflow: "hidden",
        }}>
          Loading… {Math.round(loadingProgression * 100)}%
        </div>
      )}
      <Unity unityProvider={unityProvider} style={{ width: "100%", height: "100%" }} />
    </div>
  );
}