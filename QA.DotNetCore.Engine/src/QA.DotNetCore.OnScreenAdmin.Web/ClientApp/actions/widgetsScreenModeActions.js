import { WIDGETS_SCREEN_MODE_ACTIONS } from './actionTypes';

export function hideAvailableWidgets() {
  return { type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_AVAILABLE_WIDGETS };
}

export function showAvailableWidgets() {
  return { type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_AVAILABLE_WIDGETS };
}

