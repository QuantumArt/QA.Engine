import { createSelector } from 'reselect';
import buildTree from '../utils/buildTree';

const getComponentTreeSelector = state => buildTree(state.componentTree.components);

const getComponentTree = createSelector(
  [getComponentTreeSelector],
  components => components,
);

export default getComponentTree;
