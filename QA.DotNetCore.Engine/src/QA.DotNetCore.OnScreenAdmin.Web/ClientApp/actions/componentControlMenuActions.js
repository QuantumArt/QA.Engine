import { EDIT_WIDGET_ACTIONS, ADD_WIDGET_ACTIONS } from './actionTypes';

export function addWidgetToZone(onScreenId) {
  return { type: ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE, onScreenId };
}

export function editWidget(onScreenId) {
  return { type: EDIT_WIDGET_ACTIONS.EDIT_WIDGET, onScreenId };
}

export function moveWidget(onScreenId) {
  return { type: EDIT_WIDGET_ACTIONS.MOVE_WIDGET, onScreenId };
}
