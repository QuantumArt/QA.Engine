import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
  TOGGLE_TAB,
  WIDGET_SCREEN_CHANGE_SEARCH_TEXT,
} from '../actions/actionTypes';

const initialState = {
  opened: false,
  side: 'left',
  activeTab: 0,
  widgetScreenSearchText: '',
};

export default function sidebarReducer(state = initialState, action) {
  switch (action.type) {
    case TOGGLE_OPEN_STATE:
      return { ...state, opened: !state.opened };
    case TOGGLE_LEFT_POSITION:
      return { ...state, side: 'left' };
    case TOGGLE_RIGHT_POSITION:
      return { ...state, side: 'right' };
    case TOGGLE_TAB:
      return { ...state, activeTab: action.value };
    case WIDGET_SCREEN_CHANGE_SEARCH_TEXT:
      return { ...state, widgetScreenSearchText: action.value };
    default:
      return state;
  }
}
