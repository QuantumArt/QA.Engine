import { GET_AVALAIBLE_TESTS } from 'actions/actionTypes';
import _ from 'lodash';

const initialState = {
  avalaibleTests: [],
};

export default function abTestingScreenReducer(state = initialState, action) {
  switch (action.type) {
    case GET_AVALAIBLE_TESTS:
      return { ...state,
        avalaibleTests: _.map(action.payload, (el, id) => ({ ...el, id })) };
    default:
      return state;
  }
}
