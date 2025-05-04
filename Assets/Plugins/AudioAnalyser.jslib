mergeInto(LibraryManager.library, {

  /** 
   * Replace Unity’s ScriptProcessorNode→destination chain with
   * ScriptProcessorNode→AnalyserNode→destination, recording the analyser. 
   */
  AudioAnalyser_Init: function() {
    // guard
    if (window._audioAnalyser) return;

    // grab Unity's existing AudioContext & ScriptProcessorNode
    var ctx = Module.audioContext;
    var sp  = Module.scriptProcessorNode; // Unity defines this in its loader

    if (!ctx || !sp) {
      console.error("AudioAnalyser: Unity audio context or scriptProcessorNode not found.");
      return;
    }

    // create our analyser
    var analyser = ctx.createAnalyser();
    analyser.fftSize = 256;

    // reroute audio: disconnect original → dest,
    // hook ScriptProcessor → analyser → dest
    try {
      sp.disconnect();
    } catch(e) { /* maybe already disconnected */ }
    sp.connect(analyser);
    analyser.connect(ctx.destination);

    // stash for later
    window._audioAnalyser = {
      ctx:     ctx,
      node:    analyser,
      bins:    analyser.frequencyBinCount,
      dataF32: new Float32Array(analyser.frequencyBinCount)
    };
  },


  /**
   * Copies analyser.frequencyBinCount floats into Unity’s HEAPF32 at ptr.
   */
  AudioAnalyser_GetData: function(ptr, len) {
    var a = window._audioAnalyser;
    if (!a || !a.node) {
      // fill zeros if we failed to init
      for (var i = 0; i < len; i++)
        HEAPF32[(ptr >> 2) + i] = 0;
      return;
    }

    a.node.getFloatFrequencyData(a.dataF32);
    for (var i = 0; i < len; i++)
      HEAPF32[(ptr >> 2) + i] = a.dataF32[i];
  }

});
