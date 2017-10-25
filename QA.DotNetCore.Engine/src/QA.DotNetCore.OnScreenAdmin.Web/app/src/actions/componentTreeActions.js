import * as types from './actionTypes';

export function selectComponent(id) {
  return {type: types.SELECT_COMPONENT, id: id}
}

export function loadedComponentTree(componentTree){
  return {type: types.LOADED_COMPONENT_TREE, componentTree: componentTree};
}