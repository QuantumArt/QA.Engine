import { createSelector } from 'reselect';
import _ from 'lodash';

const getSearchText = state => state.availableWidgets.searchText;
const getAvailableWidgets = state => state.metaInfo.availableWidgets;

const filterFunction = (widgets, keyword) => {
  const lowerSearchText = _.toLower(keyword);
  return _.filter(widgets, w =>
    _.includes(_.toLower(w.title), lowerSearchText) || _.includes(_.toLower(w.description), lowerSearchText),
  );
};

export const getSearchTextSelector = createSelector(
  getSearchText,
  searchText => searchText,
);

export const allAvailableWidgetsSelector = createSelector(
  getAvailableWidgets,
  widgets => widgets,
);

export const filteredAvailableWidgets = createSelector(
  [getSearchText, getAvailableWidgets],
  (searchText, availableWidgets) => filterFunction(availableWidgets, searchText),
);
