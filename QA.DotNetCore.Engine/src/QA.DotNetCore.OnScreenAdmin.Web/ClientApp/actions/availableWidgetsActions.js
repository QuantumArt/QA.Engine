import { CHANGE_AVAILABLE_WIDGETS_SEARCH_TEXT, ADD_WIDGET_ACTIONS } from './actionTypes';


export function selectWidgetToAdd(id) {
  return { type: ADD_WIDGET_ACTIONS.SELECT_WIDGET_TO_ADD, id };
}

export function changeSearchText(newValue) {
  return { type: CHANGE_AVAILABLE_WIDGETS_SEARCH_TEXT, value: newValue };
}
