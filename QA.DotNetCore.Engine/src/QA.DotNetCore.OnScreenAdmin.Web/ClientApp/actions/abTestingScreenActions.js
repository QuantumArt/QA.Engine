import { GET_AVALAIBLE_TESTS, API_GET_TESTS_DATA } from './actionTypes';

export function getAvalaibleTests(payload) {
  return { type: GET_AVALAIBLE_TESTS, payload };
}

export function apiGetTestsData(payload) {
  return { type: API_GET_TESTS_DATA, payload };
}
