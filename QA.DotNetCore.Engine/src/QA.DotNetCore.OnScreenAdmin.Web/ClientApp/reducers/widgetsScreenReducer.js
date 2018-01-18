import { WIDGETS_SCREEN_MODE_ACTIONS } from 'actions/actionTypes';

export const MODES = {
  SHOW_COMPONENT_TREE: 0,
  SHOW_AVAILABLE_WIDGETS: 1,
  SHOW_MOVE_WIDGET: 2,
};
const initialState = {
  mode: MODES.SHOW_COMPONENT_TREE,
};

export default function widgetsScreenReducer(state = initialState, action) {
  switch (action.type) {
    case WIDGETS_SCREEN_MODE_ACTIONS.SHOW_AVAILABLE_WIDGETS:
      return { ...state, mode: MODES.SHOW_AVAILABLE_WIDGETS };
    case WIDGETS_SCREEN_MODE_ACTIONS.HIDE_AVAILABLE_WIDGETS:
      return { ...state, mode: MODES.SHOW_COMPONENT_TREE };
    case WIDGETS_SCREEN_MODE_ACTIONS.SHOW_MOVE_WIDGET:
      return { ...state, mode: MODES.SHOW_MOVE_WIDGET };
    case WIDGETS_SCREEN_MODE_ACTIONS.HIDE_MOVE_WIDGET:
      return { ...state, mode: MODES.SHOW_COMPONENT_TREE };
    default:
      return state;
  }
}
