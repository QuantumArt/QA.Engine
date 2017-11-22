import {
  TOGGLE_COMPONENT,
  LOADED_COMPONENT_TREE,
  TOGGLE_SUBTREE,
  EDIT_WIDGET,
  GET_ABSTRACT_ITEM_INFO_REQUESTED,
  GET_ABSTRACT_ITEM_INFO_SUCCESS,
  GET_ABSTRACT_ITEM_INFO_FAIL,
  EDIT_WIDGET_SHOW_QP_FORM,
  EDIT_WIDGET_CLOSE_QP_FORM,
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
  return { type: EDIT_WIDGET, id };
}

export function getAbstractItemInfoRequested() {
  return { type: GET_ABSTRACT_ITEM_INFO_REQUESTED };
}

export function getAbstractItemInfoSuccess(info) {
  return { type: GET_ABSTRACT_ITEM_INFO_SUCCESS, info };
}

export function getAbstractItemInfoFail(error) {
  return { type: GET_ABSTRACT_ITEM_INFO_FAIL, error };
}

export function showQpForm() {
  return { type: EDIT_WIDGET_SHOW_QP_FORM };
}

export function closeQpForm() {
  return { type: EDIT_WIDGET_CLOSE_QP_FORM };
}
