import {
  TOGGLE_COMPONENT,
  LOADED_COMPONENT_TREE,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  CHANGE_COMPONENT_TREE_SEARCH_TEXT,
  ADD_WIDGET_ACTIONS,
} from './actionTypes';

export function toggleComponent(id) {
  return { type: TOGGLE_COMPONENT, id };
}

export function loadedComponentTree(componentTree) {
  return { type: LOADED_COMPONENT_TREE, componentTree };
}

export function toggleSubtree(id) {
  return { type: TOGGLE_SUBTREE, id };
}

export function toggleFullSubtree(id) {
  return { type: TOGGLE_FULL_SUBTREE, id };
}

export function selectWidgetToAdd(id) {
  return { type: ADD_WIDGET_ACTIONS.SELECT_WIDGET_TO_ADD, id };
}

export function hideAvailableWidgets() {
  return { type: ADD_WIDGET_ACTIONS.HIDE_AVAILABLE_WIDGETS };
}

export function changeSearchText(newValue) {
  return { type: CHANGE_COMPONENT_TREE_SEARCH_TEXT, value: newValue };
}
