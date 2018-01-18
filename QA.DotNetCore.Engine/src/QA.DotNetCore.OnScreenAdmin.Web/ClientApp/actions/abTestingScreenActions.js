import {
  GET_AVALAIBLE_TESTS,
  API_GET_TESTS_DATA,
  LAUNCH_SESSION_TEST,
  PAUSE_TEST,
  SET_TEST_CASE,
} from './actionTypes';

export function getAvalaibleTests(payload) {
  return { type: GET_AVALAIBLE_TESTS, payload };
}

export function apiGetTestsData(payload) {
  return { type: API_GET_TESTS_DATA, payload };
}

export function launchSessionTest(testId) {
  return { type: LAUNCH_SESSION_TEST, testId };
}

export function pauseTest(testId) {
  return { type: PAUSE_TEST, testId };
}

export function setTestCase(testId, value) {
  return { type: SET_TEST_CASE, payload: { testId, value } };
}
