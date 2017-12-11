import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
  TOGGLE_TAB,
  WIDGET_SCREEN_CHANGE_SEARCH_TEXT,
} from './actionTypes';

export function toggleState() {
  return { type: TOGGLE_OPEN_STATE };
}

export function toggleLeftPosition() {
  return { type: TOGGLE_LEFT_POSITION };
}

export function toggleRightPosition() {
  return { type: TOGGLE_RIGHT_POSITION };
}


export function toggleTab(value) {
  return { type: TOGGLE_TAB, value };
}

export function widgetScreenChangeSearchText(value) {
  return { type: WIDGET_SCREEN_CHANGE_SEARCH_TEXT, value };
}
