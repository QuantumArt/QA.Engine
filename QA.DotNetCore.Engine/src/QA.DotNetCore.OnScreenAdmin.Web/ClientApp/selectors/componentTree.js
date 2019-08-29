/* eslint-disable */
import { createSelector, createSelectorCreator } from 'reselect';
import _ from 'lodash';
// import buildTree from 'utils/buildTree';
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
  while (currentComponent && currentComponent.parentOnScreenId !== null) {
    parentIds.push(currentComponent.parentOnScreenId);
    currentComponent = _.find(allComponents, { onScreenId: currentComponent.parentOnScreenId });
  }
  return parentIds;
};

const filterFunction = (componentsList, keyword, disabledComponents, showOnlyWidgets, availableWidgets) => {
  if (keyword === '') {
    return buildTree(componentsList, disabledComponents, false, availableWidgets);
  }

  const searchText = _.toLower(keyword);
  const searchResults = _.filter(componentsList, (c) => {
    switch (c.type) {
      case 'zone':
        return !showOnlyWidgets && _.includes(_.toLower(c.properties.zoneName), searchText);

      case 'widget':
        return _.includes(_.toLower(c.properties.alias), searchText)
          || _.includes(_.toLower(c.properties.title), searchText)
          || _.includes(_.toLower(c.properties.widgetId), searchText);

      default:
        return false;
    }
  });
  const searchResultIds = _.map(searchResults, 'onScreenId');
  console.log('searchResultIds', searchResults);

  let parentComponentIds = [];
  _.each(searchResults, (c) => {
    const parents = getParentComponents(componentsList, c);
    parentComponentIds = _.concat(parentComponentIds, parents);
  });
  const uniqResults = _.uniq(_.concat(searchResultIds, parentComponentIds));


  const filteredFlatComponents = _.filter(componentsList, c =>
    _.some(uniqResults, componentId => componentId === c.onScreenId),
  );

  return buildTree(filteredFlatComponents, disabledComponents, true, availableWidgets);
};

export const getComponentTreeSelector = createSelector(getComponentTree, _.identity);
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

// export const _getDisabledComponentsSelector = createSelector(
//   [getComponentsList, getMovingWidget],
//   (componentsList, movingWidget) => {
//     if (!isMoving(movingWidget)) { return []; }
//
//     const movingWidgetParentZoneId = _.find(componentsList, { onScreenId: movingWidget.onScreenId }).parentOnScreenId;
//     // дочерние элементы самого виджета - тут основано на методе генерации onScreenId, если что - нужно будет поменять
//     const movingWidgetChildren = _.filter(componentsList, c => _.includes(c.onScreenId, movingWidget.onScreenId));
//     const movingWidgetChildrenIds = _.map(movingWidgetChildren, 'onScreenId');
//     console.log('movingWidgetChildrenIds', movingWidgetChildren, movingWidgetChildrenIds);
//     const widgets = _.filter(componentsList, { type: 'widget' });
//     const widgetIds = _.map(widgets, 'onScreenId');
//     const disabledComponents = _.concat(widgetIds, [movingWidgetParentZoneId], movingWidgetChildrenIds);
//     return _.uniq(disabledComponents);
//   },
// );


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
    while(currentComponent && hashMap[currentComponent]) {
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

// export const filteredComponentTree = createJSONEqualSelector(
//   [
//     getSearchTextSelector,
//     getComponentsList,
//     getDisabledComponentsSelector,
//     getShowOnlyWidgetsSelector,
//     getMovingWidget,
//     allAvailableWidgetsSelector,
//   ],
//   (searchText, componentsList, disabledComponents, showOnlyWidgets, movingWidget, availableWidgets) =>
//     filterFunction(componentsList,
//       searchText,
//       disabledComponents,
//       showOnlyWidgets && !isMoving(movingWidget),
//       availableWidgets),
// );

export const filteredComponentTree = createSelector(
  [
    getComponentsList,
    getSearchTextSelector,
    getDisabledComponentsSelector,
    getShowOnlyWidgetsSelector,
    allAvailableWidgetsSelector
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
      } else {
        const result = _.includes(_.toLower(c.properties.alias), searchText)
          || _.includes(_.toLower(c.properties.title), searchText)
          || _.includes(_.toLower(c.properties.widgetId), searchText);
        if (result) {
          searchResultIds.push(c.onScreenId);
        }
        return result;
      }
    });

    const parentComponentIds = _.reduce(
      searchResults,
      (prev, cur) => ([...prev, ...getParentComponents(componentsList, cur)]),
      []
    );
    const uniqResults = _.uniq(_.concat(searchResultIds, parentComponentIds));


    const filteredFlatComponents = _.filter(componentsList, c =>
      _.some(uniqResults, componentId => componentId === c.onScreenId),
    );
    console.log(
      buildTree(filteredFlatComponents, disabledComponents, true, availableWidgets, showOnlyWidgets)
    );

    return buildTree(filteredFlatComponents, disabledComponents, true, availableWidgets, showOnlyWidgets);
  }
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
