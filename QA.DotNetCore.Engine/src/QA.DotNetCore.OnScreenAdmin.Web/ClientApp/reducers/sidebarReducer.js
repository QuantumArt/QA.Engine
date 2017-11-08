import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
} from '../actions/actionTypes';

const initialState = {
  opened: false,
  side: 'left',
};

export default function sidebarReducer(state = initialState, action) {
  switch (action.type) {
    case TOGGLE_OPEN_STATE:
      return { ...state, opened: !state.opened };
    case TOGGLE_LEFT_POSITION:
      return { ...state, side: 'left' };
    case TOGGLE_RIGHT_POSITION:
      return { ...state, side: 'right' };
    default:
      return state;
  }
}
