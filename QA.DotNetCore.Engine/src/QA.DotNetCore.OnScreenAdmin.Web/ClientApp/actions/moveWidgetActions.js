import {
  EDIT_WIDGET_ACTIONS,
  // WIDGET_SCREEN_CHANGE_SEARCH_TEXT,
} from './actionTypes';

export default function cancelMoveWidget() {
  return { type: EDIT_WIDGET_ACTIONS.CANCEL_MOVING_WIDGET };
}

