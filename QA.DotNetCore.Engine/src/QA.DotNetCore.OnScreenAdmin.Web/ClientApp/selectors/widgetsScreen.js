import { createSelector } from 'reselect';
import { MODES } from '../reducers/widgetsScreenReducer';

const getShowComponentTreeSelector = state => state.widgetsScreen.mode === MODES.SHOW_COMPONENT_TREE;
const getShowAvailableWidgetsSelector = state => state.widgetsScreen.mode === MODES.SHOW_AVAILABLE_WIDGETS;
const getShowMoveWidgetScreenSelector = state => state.widgetsScreen.mode === MODES.SHOW_MOVE_WIDGET;

export const getShowComponentTree = createSelector(
  [getShowComponentTreeSelector],
  showComponentTree => showComponentTree,
);

export const getShowAvailableWidgets = createSelector(
  [getShowAvailableWidgetsSelector],
  showAvailableWidgets => showAvailableWidgets,
);

export const getShowMoveWidgetScreen = createSelector(
  [getShowMoveWidgetScreenSelector],
  showMoveWidget => showMoveWidget,
);
