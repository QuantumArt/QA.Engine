import { TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  EDIT_WIDGET,
  GET_ABSTRACT_ITEM_INFO_REQUESTED,
  GET_ABSTRACT_ITEM_INFO_SUCCESS,
  GET_ABSTRACT_ITEM_INFO_FAIL,
  EDIT_WIDGET_SHOW_QP_FORM,
  EDIT_WIDGET_CLOSE_QP_FORM } from '../actions/actionTypes';
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
        selectedComponentId: state.selectedComponentId === action.id ? '' : action.id,
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

    case EDIT_WIDGET:
      return {
        ...state,
        editingComponent: true,
        editingComponentId: action.id,
      };
    case GET_ABSTRACT_ITEM_INFO_REQUESTED:
      return {
        ...state,
        isLoading: true,
      };
    case GET_ABSTRACT_ITEM_INFO_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        abstractItemMetaInfo: action.info,
      };
    case GET_ABSTRACT_ITEM_INFO_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.error,
      };
    case EDIT_WIDGET_SHOW_QP_FORM:
      return {
        ...state,
        showQpForm: true,
      };
    case EDIT_WIDGET_CLOSE_QP_FORM:
      return {
        ...state,
        showQpForm: false,
        editingComponent: false,
        editingComponentId: null,
      };
    default:
      return state;
  }
}
