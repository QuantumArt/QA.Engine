import {
  SELECT_COMPONENT,
  LOADED_COMPONENT_TREE,
} from './actionTypes';

export function selectComponent(id) {
  return { type: SELECT_COMPONENT, id };
}

export function loadedComponentTree(componentTree) {
  return { type: LOADED_COMPONENT_TREE, componentTree };
}
