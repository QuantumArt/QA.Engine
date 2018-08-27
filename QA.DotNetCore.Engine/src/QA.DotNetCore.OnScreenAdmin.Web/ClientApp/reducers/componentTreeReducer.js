import {
  TOGGLE_COMPONENT,
  TOGGLE_SUBTREE,
  TOGGLE_FULL_SUBTREE,
  CHANGE_COMPONENT_TREE_SEARCH_TEXT,
  UPDATE_COMPONENTS,
  TOGGLE_SHOW_ONLY_WIDGETS,
  TOGGLE_COMPONENT_TREE_SEARCH_BOX,
} from 'actions/actionTypes';
import buildFlatListNew from 'utils/buildFlatListNew';
import buildFlatList from 'utils/buildFlatList';
import buildTreeNew from 'utils/buildTreeNew';


// const components = buildFlatList();
const components = buildFlatListNew();

const getMaxNestLevel = comps => comps.map(c => c.nestLevel).reduce((max, cur) => Math.max(max, cur));

const initialState = {
  selectedComponentId: '',
  components,
  maxNestLevel: getMaxNestLevel(components),
  searchText: '',
  showOnlyWidgets: false,
  showSearchBox: false,
};

export default function componentTreeReducer(state = initialState, action) {
  switch (action.type) {
    case UPDATE_COMPONENTS:
      return {
        ...state,
        components: action.components,
        maxNestLevel: getMaxNestLevel(action.components),
      };
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
    case TOGGLE_SHOW_ONLY_WIDGETS:
      return {
        ...state,
        showOnlyWidgets: !state.showOnlyWidgets,
      };
    case TOGGLE_COMPONENT_TREE_SEARCH_BOX:
      return {
        ...state,
        showSearchBox: !state.showSearchBox,
        searchText: '',
      };
    default:
      return state;
  }
}
