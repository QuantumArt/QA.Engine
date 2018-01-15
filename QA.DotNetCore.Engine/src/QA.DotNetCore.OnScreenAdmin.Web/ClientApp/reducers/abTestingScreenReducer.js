import _ from 'lodash';
import {
  GET_AVALAIBLE_TESTS,
  API_GET_TESTS_DATA,
} from 'actions/actionTypes';

const initialState = {
  avalaibleTests: [],
  testsData: [],
};

export default function abTestingScreenReducer(state = initialState, action) {
  switch (action.type) {
    case GET_AVALAIBLE_TESTS:
      return { ...state, avalaibleTests: _.map(action.payload, (el, id) => ({ ...el, id })) };
    case API_GET_TESTS_DATA:
      return { ...state, testsData: action.payload };
    default:
      return state;
  }
}
