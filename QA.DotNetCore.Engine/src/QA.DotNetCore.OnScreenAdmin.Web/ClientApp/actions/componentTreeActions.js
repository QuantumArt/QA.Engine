import {
  TOGGLE_COMPONENT,
  LOADED_COMPONENT_TREE,
  TOGGLE_SUBTREE,
} from './actionTypes';

export function toggleComponent(id) {
  return { type: TOGGLE_COMPONENT, id };
}

export function loadedComponentTree(componentTree) {
  return { type: LOADED_COMPONENT_TREE, componentTree };
}

export function toggleSubtree(id) {
  return { type: TOGGLE_SUBTREE, id };
}
