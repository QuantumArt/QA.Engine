import { 
  EDIT_WIDGET_ACTIONS,
  ADD_WIDGET_ACTIONS,
  QP_FORM_ACTIONS,
  EDIT_PAGE_ACTIONS,
} from 'actions/actionTypes';


const initialState = {
  editWidget: {
    isActive: false,
    onScreenId: null,
  },
  addWidget: {
    isActive: false,
    zoneOnScreenId: null,
    selectedWidgetId: null,
  },
  moveWidget: {
    isActive: false,
    onScreenId: null,
    targetZoneId: null,
  },
  needReload: false,
  qpFormOpened: false,
};

export default function articleManagementReducer(state = initialState, action) {
  switch (action.type) {
    case EDIT_WIDGET_ACTIONS.EDIT_WIDGET:
      return {
        ...state,
        editWidget: {
          ...state.editWidget,
          isActive: true,
          onScreenId: action.onScreenId,
        },
      };
    case ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE:
      return {
        ...state,
        addWidget: {
          ...state.addWidget,
          isActive: true,
          zoneOnScreenId: action.onScreenId,
          selectedWidgetId: null,
        },
      };
    case ADD_WIDGET_ACTIONS.SELECT_WIDGET_TO_ADD:
      return {
        ...state,
        addWidget: {
          ...state.addWidget,
          selectedWidgetId: action.id,
        },
      };
    case QP_FORM_ACTIONS.NEED_RELOAD:
      return {
        ...state,
        needReload: true,
      };
    case QP_FORM_ACTIONS.CLOSE_QP_FORM:
      return {
        ...state,
        addWidget: {
          ...state.addWidget,
          isActive: false,
          zoneOnScreenId: null,
          selectedWidgetId: null,
        },
        editWidget: {
          ...state.editWidget,
          isActive: false,
          onScreenId: null,
        },
        qpFormOpened: false,
      };
    case ADD_WIDGET_ACTIONS.SHOW_QP_FORM:
      return {
        ...state,
        qpFormOpened: true,
      };
    case EDIT_WIDGET_ACTIONS.SHOW_QP_FORM:
      return {
        ...state,
        qpFormOpened: true,
      };
    case EDIT_WIDGET_ACTIONS.MOVE_WIDGET:
      return {
        ...state,
        moveWidget: {
          ...state.moveWidget,
          isActive: true,
          onScreenId: action.onScreenId,
        },
      };
    case EDIT_WIDGET_ACTIONS.MOVING_WIDGET_SELECT_TARGET_ZONE:
      return {
        ...state,
        moveWidget: {
          ...state.moveWidget,
          targetZoneId: action.id,
        },
      };
    case EDIT_WIDGET_ACTIONS.FINISH_MOVING_WIDGET:
      return {
        ...state,
        moveWidget: {
          isActive: false,
          onScreenId: null,
          targetZoneId: null,
        },
      };
    case EDIT_PAGE_ACTIONS.SHOW_QP_FORM:
      return {
        ...state,
        qpFormOpened: true,
      };
    default:
      return state;
  }
}
