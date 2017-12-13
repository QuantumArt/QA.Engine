import { createSelector, createSelectorCreator } from 'reselect';
import _ from 'lodash';
import buildTree from '../utils/buildTree';

const getComponentTreeSelector = state => buildTree(state.componentTree.components);
const getFlatComponentsSelector = state => state.componentTree.components;
const getMaxNestLevelSelector = state => state.componentTree.maxNestLevel;
const getSelectedComponentIdSelector = state => state.componentTree.selectedComponentId;
const getSearchTextSelector = state => state.componentTree.searchText;

const hashFn = (...args) => args.reduce(
  (acc, val) => `${acc}-${JSON.stringify(val)}`,
  '',
);

const createJSONEqualSelector = createSelectorCreator(
  _.memoize,
  hashFn,
);

const getParentComponents = (allComponents, component) => {
  const parentIds = [];
  let currentComponent = component;
  while (currentComponent && currentComponent.parentOnScreenId !== 'page') {
    parentIds.push(currentComponent.parentOnScreenId);
    currentComponent = _.find(allComponents, { onScreenId: currentComponent.parentOnScreenId });
  }
  return parentIds;
};

const filterFunction = (componentsFlat, keyword) => {
  if (keyword === '') { return buildTree(componentsFlat); }
  const searchText = _.toLower(keyword);
  const searchResults = _.filter(componentsFlat, (c) => {
    switch (c.type) {
      case 'zone':
        return _.includes(_.toLower(c.properties.zoneName), searchText);
      case 'widget':
        return _.includes(_.toLower(c.properties.alias), searchText)
        || _.includes(_.toLower(c.properties.title), searchText);
      default:
        return false;
    }
  });
  const searchResultIds = _.map(searchResults, 'onScreenId');
  console.log('searchResultIds', searchResultIds);
  let parentComponentIds = [];
  _.each(searchResults, (c) => {
    const parents = getParentComponents(componentsFlat, c);
    parentComponentIds = _.concat(parentComponentIds, parents);
  });
  const uniqResults = _.uniq(_.concat(searchResultIds, parentComponentIds));


  console.log('uniqResults', uniqResults);


  const filteredFlatComponents = _.filter(componentsFlat, c =>
    _.some(uniqResults, componentId => componentId === c.onScreenId),
  );

  console.log(keyword, filteredFlatComponents);

  return buildTree(filteredFlatComponents, true);
};


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

export const filteredComponentTree = createJSONEqualSelector(
  [getSearchTextSelector, getFlatComponentsSelector],
  (searchText, componentsFlat) => filterFunction(componentsFlat, searchText),
);

