mergeInto(LibraryManager.library, {
  // returns milliseconds since Unix epoch (UTC)
  GetLocalTimeMillis: function() {
    return Date.now();
  },
  // returns minutes offset: local → UTC (e.g. MST returns 420)
  GetTimezoneOffsetMinutes: function() {
    return new Date().getTimezoneOffset();
  }
});
