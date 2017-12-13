import { createSelector } from 'reselect';
import buildTree from '../utils/buildTree';

const getComponentTreeSelector = state => buildTree(state.componentTree.components);
const getMaxNestLevelSelector = state => state.componentTree.maxNestLevel;
const getSelectedComponentIdSelector = state => state.componentTree.selectedComponentId;
const getSearchTextSelector = state => state.componentTree.searchText;

export const getComponentTree = createSelector(
  [getComponentTreeSelector],
  components => components,
);

export const getMaxNestLevel = createSelector(
  [getMaxNestLevelSelector],
  maxNestLevel => maxNestLevel,
);

export const getSelectedComponentId = createSelector(
  [getSelectedComponentIdSelector],
  selectedComponentId => selectedComponentId,
);

export const getSearchText = createSelector(
  [getSearchTextSelector],
  searchText => searchText,
);

