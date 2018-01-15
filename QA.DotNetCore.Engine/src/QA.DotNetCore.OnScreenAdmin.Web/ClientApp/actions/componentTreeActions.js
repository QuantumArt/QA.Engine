import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  CHANGE_COMPONENT_TREE_SEARCH_TEXT,
  EDIT_WIDGET_ACTIONS,
} from './actionTypes';

export function toggleComponent(id) {
  return { type: TOGGLE_COMPONENT, id };
}

export function toggleSubtree(id) {
  return { type: TOGGLE_SUBTREE, id };
}

export function toggleFullSubtree(id) {
  return { type: TOGGLE_FULL_SUBTREE, id };
}

export function changeSearchText(newValue) {
  return { type: CHANGE_COMPONENT_TREE_SEARCH_TEXT, value: newValue };
}

export function finishMovingWidget(id) {
  return { type: EDIT_WIDGET_ACTIONS.FINISH_MOVING_WIDGET, id };
}

export function movingWidgetSelectTargetZone(id) {
  return { type: EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SELECT_TARGET_ZONE, id };
}
