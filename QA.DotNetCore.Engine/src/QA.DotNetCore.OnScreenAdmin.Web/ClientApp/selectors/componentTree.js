import { createSelector } from 'reselect';
import _ from 'lodash';
import buildTree from 'utils/buildTreeNew';
import { allAvailableWidgetsSelector } from './availableWidgets';


const getComponentTree = state => buildTree(state.componentTree.components); // TODO: fix
const getComponentsList = state => state.componentTree.components;
const getMaxNestLevel = state => state.componentTree.maxNestLevel;
const getSelectedComponentId = state => state.componentTree.selectedComponentId;
const getSearchText = state => state.componentTree.searchText;
const getMovingWidget = state => state.articleManagement.moveWidget;
const getShowOnlyWidgets = state => state.componentTree.showOnlyWidgets;
const getShowSearchBox = state => state.componentTree.showSearchBox;

const getParentComponents = (allComponents, component) => {
  const parentIds = [];
  let currentComponent = component;
  while (currentComponent && currentComponent.parentOnScreenId !== null) {
    parentIds.push(currentComponent.parentOnScreenId);
    currentComponent = _.find(allComponents, { onScreenId: currentComponent.parentOnScreenId });
  }
  return parentIds;
};

export const getComponentTreeSelector = createSelector(getComponentTree, _.identity);
export const getComponentsListSelector = createSelector(getComponentsList, _.identity);
export const getMaxNestLevelSelector = createSelector(getMaxNestLevel, _.identity);
export const getSelectedComponentIdSelector = createSelector(getSelectedComponentId, _.identity);
export const getSearchTextSelector = createSelector(getSearchText, _.identity);
export const getShowSearchBoxSelector = createSelector(getShowSearchBox, _.identity);

const isMoving = movingWidget => !(movingWidget == null || !movingWidget.isActive || !movingWidget.onScreenId);
export const getIsMovingWidgetSelector = createSelector(
  getMovingWidget,
  movingWidget => isMoving(movingWidget),
);

export const getShowOnlyWidgetsSelector = createSelector(
  [getShowOnlyWidgets, getMovingWidget],
  (showOnlyWidgets, movingWidget) => showOnlyWidgets && !isMoving(movingWidget),
);

export const getDisabledComponentsSelector = createSelector(
  [getComponentsList, getMovingWidget],
  (componentsList, movingWidget) => {
    if (!isMoving(movingWidget)) {
      return [];
    }

    const hashMap = {};
    const widgetsId = [];
    _.forEach(componentsList, (el) => {
      hashMap[el.onScreenId] = el;
      if (el.type === 'widget') {
        widgetsId.push(el.onScreenId);
      }
    });

    const movingWidgetParentZoneId = hashMap[movingWidget.onScreenId].parentOnScreenId;
    const movingWidgetChildrenId = [];

    let currentComponent = movingWidget.onScreenId;
    while (currentComponent && hashMap[currentComponent]) {
      const el = _.find(componentsList, { parentOnScreenId: currentComponent });
      if (el) {
        movingWidgetChildrenId.push(el.onScreenId);
        currentComponent = el.onScreenId;
      } else {
        currentComponent = null;
      }
    }

    return _.uniq([...widgetsId, ...movingWidgetChildrenId, movingWidgetParentZoneId]);
  },
);

export const filteredComponentTree = createSelector(
  [
    getComponentsList,
    getSearchTextSelector,
    getDisabledComponentsSelector,
    getShowOnlyWidgetsSelector,
    allAvailableWidgetsSelector,
  ],
  (componentsList, keyword, disabledComponents, showOnlyWidgets, availableWidgets) => {
    if (keyword === '') {
      return buildTree(componentsList, disabledComponents, false, availableWidgets, showOnlyWidgets);
    }

    const searchText = _.toLower(keyword);
    const searchResultIds = [];
    const searchResults = _.filter(componentsList, (c) => {
      /**
       * @namespace
       * @property {Object} c
       * @property {Object} c.properties
       * @property {string} c.properties.alias
       * @property {string} c.properties.zoneName
       * @property {string} c.properties.title
       * @property {number} c.properties.widgetId
       */
      if (c.type === 'zone') {
        const result = !showOnlyWidgets && _.includes(_.toLower(c.properties.zoneName), searchText);
        if (result) {
          searchResultIds.push(c.onScreenId);
        }
        return result;
      }
      const result = _.includes(_.toLower(c.properties.alias), searchText)
          || _.includes(_.toLower(c.properties.title), searchText)
          || _.includes(_.toLower(c.properties.widgetId), searchText);
      if (result) {
        searchResultIds.push(c.onScreenId);
      }
      return result;
    });

    const parentComponentIds = _.reduce(searchResults,
      (prev, cur) => ([...prev, ...getParentComponents(componentsList, cur)]),
      [],
    );
    const uniqResults = _.uniq(_.concat(searchResultIds, parentComponentIds));
    const filteredFlatComponents = _.filter(componentsList, c =>
      _.some(uniqResults, componentId => componentId === c.onScreenId),
    );

    return buildTree(filteredFlatComponents, disabledComponents, true, availableWidgets, showOnlyWidgets);
  },
);

export const getMovingWidgetTargetZoneSelector = createSelector(
  [getComponentsList, getMovingWidget],
  (componentsList, movingWidget) => {
    if (!isMoving(movingWidget) || !movingWidget.targetZoneId) {
      return null;
    }

    return _.find(componentsList, { onScreenId: movingWidget.targetZoneId });
  },
);

export const movingWidgetSelector = createSelector(
  [getComponentsList, getMovingWidget],
  (componentsList, movingWidget) => {
    if (!isMoving(movingWidget)) {
      return null;
    }

    return _.find(componentsList, { onScreenId: movingWidget.onScreenId });
  },
);
