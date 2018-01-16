import { createSelector } from 'reselect';
import _ from 'lodash/core';

const getSearchTextSelector = state => state.availableWidgets.searchText;
const availableWidgetsSelector = state => state.metaInfo.availableWidgets;

const filterFunction = (widgets, keyword) => {
  const lowerSearchText = _.toLower(keyword);
  return _.filter(widgets, w =>
    _.includes(_.toLower(w.title), lowerSearchText) || _.includes(_.toLower(w.description), lowerSearchText),
  );
};

export const getSearchText = createSelector(
  [getSearchTextSelector],
  searchText => searchText,
);

export const allAvailableWidgets = createSelector(
  [availableWidgetsSelector],
  widgets => widgets,
);

export const filteredAvailableWidgets = createSelector(
  [getSearchTextSelector, availableWidgetsSelector],
  (searchText, availableWidgets) => filterFunction(availableWidgets, searchText),
);
