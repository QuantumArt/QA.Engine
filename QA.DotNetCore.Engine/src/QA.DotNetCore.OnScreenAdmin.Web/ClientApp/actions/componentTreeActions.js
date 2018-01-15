import {
  TOGGLE_COMPONENT,
  LOADED_COMPONENT_TREE,
  TOGGLE_SUBTREE,
  EDIT_WIDGET_ACTIONS,
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

export function editWidget(id) {
  return { type: EDIT_WIDGET_ACTIONS.EDIT_WIDGET, id };
}

export function getAbstractItemInfoRequested() {
  return { type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_REQUESTED };
}

export function getAbstractItemInfoSuccess(info) {
  return { type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS, info };
}

export function getAbstractItemInfoFail(error) {
  return { type: EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_FAIL, error };
}

export function showQpForm() {
  return { type: EDIT_WIDGET_ACTIONS.SHOW_QP_FORM };
}

export function addWidgetToZone(zoneId) {
  return { type: ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE, zoneId };
}