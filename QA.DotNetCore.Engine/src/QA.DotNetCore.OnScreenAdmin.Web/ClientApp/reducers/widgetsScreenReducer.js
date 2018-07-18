import { WIDGETS_SCREEN_MODE_ACTIONS } from 'actions/actionTypes';

export const MODES = {
  SHOW_COMPONENT_TREE: 0,
  SHOW_WIDGET_CREATION_WIZARD: 1,
  SHOW_MOVE_WIDGET: 2,
};
const initialState = {
  mode: MODES.SHOW_COMPONENT_TREE,
  availableWidgetsInfoLoaded: false,
};

export default function widgetsScreenReducer(state = initialState, action) {
  switch (action.type) {
    case WIDGETS_SCREEN_MODE_ACTIONS.SHOW_WIDGET_CREATION_WIZARD:
      return { ...state, mode: MODES.SHOW_WIDGET_CREATION_WIZARD };
    case WIDGETS_SCREEN_MODE_ACTIONS.HIDE_WIDGET_CREATION_WIZARD:
      return { ...state, mode: MODES.SHOW_COMPONENT_TREE };
    case WIDGETS_SCREEN_MODE_ACTIONS.SHOW_MOVE_WIDGET:
      return { ...state, mode: MODES.SHOW_MOVE_WIDGET };
    case WIDGETS_SCREEN_MODE_ACTIONS.HIDE_MOVE_WIDGET:
      return { ...state, mode: MODES.SHOW_COMPONENT_TREE };
    case WIDGETS_SCREEN_MODE_ACTIONS.AVAILABLE_WIDGETS_INFO_LOADED: {
      return { ...state, availableWidgetsInfoLoaded: true };
    }
    default:
      return state;
  }
}
