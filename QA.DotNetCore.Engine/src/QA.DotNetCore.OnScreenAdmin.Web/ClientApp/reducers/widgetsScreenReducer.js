import { ADD_WIDGET_ACTIONS } from '../actions/actionTypes';

export const MODES = {
  SHOW_COMPONENT_TREE: 0,
  SHOW_AVAILABLE_WIDGETS: 1,
};
const initialState = {
  mode: MODES.SHOW_COMPONENT_TREE,
};

export default function widgetsScreenReducer(state = initialState, action) {
  switch (action.type) {
    case ADD_WIDGET_ACTIONS.SHOW_AVAILABLE_WIDGETS:
      return { ...state, mode: MODES.SHOW_AVAILABLE_WIDGETS };
    case ADD_WIDGET_ACTIONS.HIDE_AVAILABLE_WIDGETS:
      return { ...state, mode: MODES.SHOW_COMPONENT_TREE };
    default:
      return state;
  }
}
