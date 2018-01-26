import _ from 'lodash';
import {
  GET_AVALAIBLE_TESTS,
  API_GET_TESTS_DATA_SUCCESS,
  API_GET_TESTS_DATA_ERROR,
} from 'actions/actionTypes';

const initialState = {
  avalaibleTests: [],
  testsData: [],
};

export default function abTestingScreenReducer(state = initialState, action) {
  switch (action.type) {
    case GET_AVALAIBLE_TESTS:
      return { ...state, avalaibleTests: _.map(action.payload, (el, id) => ({ ...el, id })) };
    case API_GET_TESTS_DATA_SUCCESS:
      return { ...state, testsData: action.payload };
    case API_GET_TESTS_DATA_ERROR:
      return { ...state, testsData: [] };
    default:
      return state;
  }
}
