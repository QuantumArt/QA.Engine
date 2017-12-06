import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  EDIT_WIDGET_ACTIONS,
  ADD_WIDGET_ACTIONS,
  QP_FORM_ACTIONS,
} from '../actions/actionTypes';
import buildTree from '../utils/buildFlatList';

const components = buildTree();

const initialState = {
  selectedComponentId: '',
  components,
  isLoading: false,
  editingComponent: false,
  editingComponentId: null,
  error: null,
  showQpForm: false,
  abstractItemMetaInfo: null,
  showAvailableWidgets: false,
  availableWidgets: null,
};

export default function componentTreeReducer(state = initialState, action) {
  switch (action.type) {
    case TOGGLE_COMPONENT:
      return {
        ...state,
        components: state.components.map(component =>
          (component.onScreenId === action.id
            ? { ...component, isSelected: true }
            : { ...component, isSelected: false }),
        ),
        selectedComponentId: state.selectedComponentId === action.id
          ? ''
          : action.id,
      };
    case TOGGLE_SUBTREE:
      return {
        ...state,
        components: state.components.map(component =>
          (component.onScreenId === action.id
            ? { ...component, isOpened: !component.isOpened }
            : component),
        ),
      };
    case TOGGLE_FULL_SUBTREE: {
      const getParentId = (id) => {
        const arr = id.split(';');

        return arr.slice(0, arr.length - 1).join(';');
      };

      const checkIfLastParent = (id) => {
        const arr = id.split(';');

        return arr.length < 2;
      };

      const componentsToEdit = [];
      const getIds = (id) => {
        const newId = getParentId(id);
        if (!checkIfLastParent(id)) {
          state.components.forEach((component) => {
            if (component.onScreenId === id) {
              componentsToEdit.push(component.onScreenId);
            }
          });
          getIds(newId);
        }
      };
      state.components.forEach((component) => {
        if (component.onScreenId === action.id) {
          getIds(component.onScreenId);
        }
      });

      return {
        ...state,
        components: state.components.map(component =>
          (componentsToEdit.indexOf(component.onScreenId) !== -1
            ? { ...component, isOpened: true }
            : { ...component, isOpened: false }),
        ),
      };
    }
    case EDIT_WIDGET_ACTIONS.EDIT_WIDGET:
      return {
        ...state,
        editingComponent: true,
        editingComponentId: action.id,
      };
    case EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_REQUESTED:
      return {
        ...state,
        isLoading: true,
      };
    case EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        abstractItemMetaInfo: action.info,
      };
    case EDIT_WIDGET_ACTIONS.GET_ABSTRACT_ITEM_INFO_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.error,
      };
    case EDIT_WIDGET_ACTIONS.SHOW_QP_FORM:
      return {
        ...state,
        showQpForm: true,
      };
    case QP_FORM_ACTIONS.CLOSE_QP_FORM:
      return {
        ...state,
        showQpForm: false,
        editingComponent: false,
        editingComponentId: null,
      };
    case QP_FORM_ACTIONS.NEED_RELOAD:
      return {
        ...state,
        needReload: true,
      };
    case ADD_WIDGET_ACTIONS.ADD_WIDGET_TO_ZONE:
      return {
        ...state,
        addingWidget: true,
        zoneId: action.zoneId,
      };
    case ADD_WIDGET_ACTIONS.SHOW_AVAILABLE_WIDGETS:
      return {
        ...state,
        showAvailableWidgets: true,
      };
    case ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_REQUESTED:
      return {
        ...state,
        isLoading: true,
      };
    case ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        availableWidgets: action.availableWidgets,
      };
    case ADD_WIDGET_ACTIONS.GET_AVAILABLE_WIDGETS_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.error,
      };
    default:
      return state;
  }
}
