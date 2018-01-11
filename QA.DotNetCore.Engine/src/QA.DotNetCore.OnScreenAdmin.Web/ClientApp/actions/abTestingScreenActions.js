import { GET_AVALAIBLE_TESTS } from './actionTypes';

/* eslint-disable import/prefer-default-export */
export function getAvalaibleTests(payload) {
  return { type: GET_AVALAIBLE_TESTS, payload };
}
