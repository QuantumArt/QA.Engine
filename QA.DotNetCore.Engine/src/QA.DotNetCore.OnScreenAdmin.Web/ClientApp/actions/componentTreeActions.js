import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  CHANGE_COMPONENT_TREE_SEARCH_TEXT,
  UPDATE_COMPONENTS,
  EDIT_WIDGET_ACTIONS,
  TOGGLE_SHOW_ONLY_WIDGETS,
  TOGGLE_COMPONENT_TREE_SEARCH_BOX,
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


export function updateComponents(components) {
  return { type: UPDATE_COMPONENTS, components };
}

export function toggleShowOnlyWidgets() {
  return { type: TOGGLE_SHOW_ONLY_WIDGETS };
}

export function toggleComponentTreeSearchBox() {
  return { type: TOGGLE_COMPONENT_TREE_SEARCH_BOX };
}
