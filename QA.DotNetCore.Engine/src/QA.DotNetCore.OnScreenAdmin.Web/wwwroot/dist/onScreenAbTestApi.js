var QA = QA || {};
QA.OnScreen = QA.OnScreen || {};
QA.OnScreen.AbTesting = (function (cookies) {
  const valueCookiePrefix = 'abt-';
  const forceCookiePrefix = 'force-abt-';
  const abTestCookiePath = '/abtest/inlinescript';

  return {
    setChoice(testId, value) {
      cookies.set(valueCookiePrefix + testId, value, { path: abTestCookiePath });
    },
    enableTestForMe(testId) {
      cookies.set(forceCookiePrefix + testId, 1, { path: abTestCookiePath });
    },
    disableTestForMe(testId) {
      cookies.set(forceCookiePrefix + testId, 0, { path: abTestCookiePath });
    },
    getIsStageModeValue() {
      if (window.onScreenOverrideAbTestStageModeCookieName) {
        const cookieOverride = cookies.get(window.onScreenOverrideAbTestStageModeCookieName);
        if (cookieOverride === '0' || cookieOverride === '1') {
          return cookieOverride === '1';
        }
      }
      return window.isStage;
    },
    setIsStageModeValue(value) {
      if (window.onScreenOverrideAbTestStageModeCookieName) {
        if (window.hasOwnProperty('isStage') && value === window.isStage) {
          cookies.expire(window.onScreenOverrideAbTestStageModeCookieName);
        } else {
          cookies.set(window.onScreenOverrideAbTestStageModeCookieName, value ? 1 : 0);
        }
      }
    },
  };
}(Cookies));
