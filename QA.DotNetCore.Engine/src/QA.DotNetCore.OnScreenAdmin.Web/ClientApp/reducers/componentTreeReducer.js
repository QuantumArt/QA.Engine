import { TOGGLE_COMPONENT, TOGGLE_SUBTREE } from '../actions/actionTypes';
import buildTree from '../utils/buildFlatList';

const components = buildTree();

const initialState = {
  selectedComponentId: '',
  components,
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

    default:
      return state;
  }
}
