import { createSelector, createSelectorCreator } from 'reselect';
import _ from 'lodash';
import buildTree from '../utils/buildTree';


const getComponentTreeSelector = state => buildTree(state.componentTree.components);
const getFlatComponentsSelector = state => state.componentTree.components;
const getMaxNestLevelSelector = state => state.componentTree.maxNestLevel;
const getSelectedComponentIdSelector = state => state.componentTree.selectedComponentId;
const getSearchTextSelector = state => state.componentTree.searchText;
const getMovingWidgetSelector = state => state.articleManagement.moveWidget;
const getShowOnlyWidgetsSelector = state => state.componentTree.showOnlyWidgets;
const getShowSearchBoxSelector = state => state.componentTree.showSearchBox;


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

const filterFunction = (componentsFlat, keyword, disabledComponents, showOnlyWidgets) => {
  console.log('showOnlyWidgets', showOnlyWidgets);
  if (keyword === '') { return buildTree(componentsFlat, disabledComponents); }
  const searchText = _.toLower(keyword);
  const searchResults = _.filter(componentsFlat, (c) => {
    switch (c.type) {
      case 'zone':
        return !showOnlyWidgets && _.includes(_.toLower(c.properties.zoneName), searchText);
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


  // console.log('uniqResults', uniqResults);


  const filteredFlatComponents = _.filter(componentsFlat, c =>
    _.some(uniqResults, componentId => componentId === c.onScreenId),
  );

  // console.log(keyword, filteredFlatComponents);

  return buildTree(filteredFlatComponents, disabledComponents, true);
};

const isMoving = movingWidget => !(movingWidget == null || !movingWidget.isActive || !movingWidget.onScreenId);


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

export const getShowSearchBox = createSelector(
  [getShowSearchBoxSelector],
  show => show,
);


export const getIsMovingWidget = createSelector(
  [getMovingWidgetSelector],
  movingWidget => isMoving(movingWidget),
);

export const getShowOnlyWidgets = createSelector(
  [getShowOnlyWidgetsSelector, getMovingWidgetSelector],
  (showOnlyWidgets, movingWidget) => showOnlyWidgets && !isMoving(movingWidget),
);

export const getDisabledComponents = createSelector(
  [getFlatComponentsSelector, getMovingWidgetSelector],
  (componentsFlat, movingWidget) => {
    if (!isMoving(movingWidget)) { return []; }

    const movingWidgetParentZoneId = _.find(componentsFlat, { onScreenId: movingWidget.onScreenId }).parentOnScreenId;
    // дочерние элементы самого виджета - тут основано на методе генерации onScreenId, если что - нужно будет поменять
    const movingWidgetChildren = _.filter(componentsFlat, c => _.includes(c.onScreenId, movingWidget.onScreenId));
    const movingWidgetChildrenIds = _.map(movingWidgetChildren, 'onScreenId');
    console.log('movingWidgetChildrenIds', movingWidgetChildren, movingWidgetChildrenIds);
    const widgets = _.filter(componentsFlat, { type: 'widget' });
    const widgetIds = _.map(widgets, 'onScreenId');
    const disabledComponents = _.concat(widgetIds, [movingWidgetParentZoneId], movingWidgetChildrenIds);
    return _.uniq(disabledComponents);
  },
);

export const filteredComponentTree = createJSONEqualSelector(
  [
    getSearchTextSelector,
    getFlatComponentsSelector,
    getDisabledComponents,
    getShowOnlyWidgetsSelector,
    getMovingWidgetSelector],
  (searchText, componentsFlat, disabledComponents, showOnlyWidgets, movingWidget) =>
    filterFunction(componentsFlat, searchText, disabledComponents, showOnlyWidgets && !isMoving(movingWidget)),
);

export const getMovingWidgetTargetZoneSelector = createSelector(
  [getFlatComponentsSelector, getMovingWidgetSelector],
  (componentsFlat, movingWidget) => {
    if (!isMoving(movingWidget)
      || !movingWidget.targetZoneId) {
      return null;
    }
    return _.find(componentsFlat, { onScreenId: movingWidget.targetZoneId });
  },
);

export const movingWidgetSelector = createSelector(
  [getFlatComponentsSelector, getMovingWidgetSelector],
  (componentsFlat, movingWidget) => {
    if (!isMoving(movingWidget)) {
      return null;
    }
    return _.find(componentsFlat, { onScreenId: movingWidget.onScreenId });
  },
);
