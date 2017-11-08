import * as types from '../actions/actionTypes';
import buildTree from '../utils/buildFlatList';
// import buildTree from '../utils/buildTree';

const components = buildTree();

const initialState = {
  selectedComponentId: null,
  components,
};

export default function componentTreeReducer(state = initialState, action) {
  switch (action.type) {
    case types.SELECT_COMPONENT:
      return { ...state, selectedComponentId: action.id };
    case types.LOADED_COMPONENT_TREE:
      console.log('loaded tree reducer', state, action.componentTree);
      return { ...state, components: action.componentTree };
    default:
      return state;
  }
}
