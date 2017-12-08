import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  EDIT_WIDGET_ACTIONS,
  ADD_WIDGET_ACTIONS,
  QP_FORM_ACTIONS,
} from '../actions/actionTypes';
import buildFlatList from '../utils/buildFlatList';


const components = buildFlatList();
const initialState = {
  selectedComponentId: '',
  components,
  maxNestLevel: components.map(c => c.nestLevel).reduce((max, cur) => Math.max(max, cur)),
  isLoading: false,
  editingComponent: false,
  editingComponentOnScreenId: null,
  error: null,
  showQpForm: false,
  showAvailableWidgets: false,
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
        editingComponentOnScreenId: action.onScreenId,
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
        editingCompoeditingComponentOnScreenIdnentId: null,
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
        zoneToAddWidgetOnScreenId: action.onScreenId,
      };
    case ADD_WIDGET_ACTIONS.SHOW_AVAILABLE_WIDGETS:
      return {
        ...state,
        showAvailableWidgets: true,
      };

    case ADD_WIDGET_ACTIONS.SELECT_WIDGET_TO_ADD:
      return {
        ...state,
        selectedWidgetToAddId: action.id,
      };
    case ADD_WIDGET_ACTIONS.HIDE_AVAILABLE_WIDGETS:
      return {
        ...state,
        showAvailableWidgets: false,
      };
    default:
      return state;
  }
}
