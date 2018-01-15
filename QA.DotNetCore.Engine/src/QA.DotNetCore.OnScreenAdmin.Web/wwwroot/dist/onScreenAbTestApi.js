var QA = QA || {};
QA.OnScreen = QA.OnScreen || {};
QA.OnScreen.AbTesting = (function(cookies){
  var valueCookiePrefix = "abt-";
  var forceCookiePrefix = "force-abt-";
  var abTestCookiePath = "/abtest/inlinescript";

  return {
    setChoice: function(testId, value){
      cookies.set(valueCookiePrefix + testId, value, { path: abTestCookiePath });
    },
    enableTestForMe: function(testId){
      cookies.set(forceCookiePrefix + testId, 1, { path: abTestCookiePath });
    },
    disableTestForMe: function(testId){
      cookies.set(forceCookiePrefix + testId, 0, { path: abTestCookiePath });
    },
    getIsStageModeValue: function () {
      if (window.onScreenOverrideAbTestStageModeCookieName) {
        var cookieOverride = cookies.get(window.onScreenOverrideAbTestStageModeCookieName);
        if (cookieOverride === "0" || cookieOverride === "1") {
          return cookieOverride === "1";
        }
      }
      return window.isStage;
    },
    setIsStageModeValue: function (value) {
      if (window.onScreenOverrideAbTestStageModeCookieName) {
        if (window.hasOwnProperty("isStage") && value === window.isStage) {
          cookies.expire(window.onScreenOverrideAbTestStageModeCookieName);
        }
        else {
          cookies.set(window.onScreenOverrideAbTestStageModeCookieName, value ? 1 : 0);
        }
      }
    }
  };
})(Cookies);
