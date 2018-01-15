var QA = QA || {};
QA.OnScreen = QA.OnScreen || {};
QA.OnScreen.AbTesting = (function(cookies){
  var valueCookiePrefix = "abt-";
  var forceCookiePrefix = "force-abt-";

  return {
    setChoice: function(testId, value){
      cookies.set(valueCookiePrefix + testId, value, { path: "/abtest/inlinescript" });
    },
    enableTestForMe: function(testId){
      cookies.set(forceCookiePrefix + testId, 1, { path: "/abtest/inlinescript" });
    },
    disableTestForMe: function(testId){
      cookies.set(forceCookiePrefix + testId, 0, { path: "/abtest/inlinescript" });
    }
  };
})(Cookies);
