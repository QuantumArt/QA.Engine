import { WIDGETS_SCREEN_MODE_ACTIONS } from './actionTypes';

export function hideAvailableWidgets() {
  return { type: WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD };
}

export function showAvailableWidgets() {
  return { type: WIDGETS_SCREEN_MODE_ACTIONS.SHOW_WIDGET_CREATION_WIZARD };
}

