import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
  TOGGLE_ALL_ZONES,
  TOGGLE_TAB,
} from '../actions/actionTypes';

const initialState = {
  opened: false,
  side: 'left',
  showAllZones: false,
  activeTab: 0,
};

export default function sidebarReducer(state = initialState, action) {
  switch (action.type) {
    case TOGGLE_OPEN_STATE:
      return { ...state, opened: !state.opened };
    case TOGGLE_LEFT_POSITION:
      return { ...state, side: 'left' };
    case TOGGLE_RIGHT_POSITION:
      return { ...state, side: 'right' };
    case TOGGLE_ALL_ZONES:
      return { ...state, showAllZones: !state.showAllZones };
    case TOGGLE_TAB:
      return { ...state, activeTab: action.value };
    default:
      return state;
  }
}
