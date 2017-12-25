import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  CHANGE_COMPONENT_TREE_SEARCH_TEXT,
} from '../actions/actionTypes';
import buildFlatList from '../utils/buildFlatList';


const components = buildFlatList();
const initialState = {
  selectedComponentId: '',
  components,
  maxNestLevel: components.map(c => c.nestLevel).reduce((max, cur) => Math.max(max, cur)),
  searchText: '',
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
    case CHANGE_COMPONENT_TREE_SEARCH_TEXT:
      return {
        ...state,
        searchText: action.value,
      };
    default:
      return state;
  }
}
