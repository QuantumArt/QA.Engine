import { createSelector } from 'reselect';
import { MODES } from 'reducers/widgetsScreenReducer';

const getShowComponentTreeSelector = state => state.widgetsScreen.mode === MODES.SHOW_COMPONENT_TREE;
const getShowWidgetsCreationWizardSelector = state => state.widgetsScreen.mode === MODES.SHOW_WIDGET_CREATION_WIZARD;
const getShowMoveWidgetScreenSelector = state => state.widgetsScreen.mode === MODES.SHOW_MOVE_WIDGET;
const getShowLoadingIndicatorSelector = state =>
  !state.widgetsScreen.availableWidgetsInfoLoaded || state.metaInfo.isLoading;


export const getShowComponentTree = createSelector(
  [getShowComponentTreeSelector, getShowLoadingIndicatorSelector],
  (showComponentTree, showLoading) => showComponentTree && !showLoading,
);

export const getShowWidgetsCreationWizard = createSelector(
  [getShowWidgetsCreationWizardSelector, getShowLoadingIndicatorSelector],
  (showWidgetsCreationWizard, showLoading) => showWidgetsCreationWizard && !showLoading,
);

export const getShowMoveWidgetScreen = createSelector(
  [getShowMoveWidgetScreenSelector, getShowLoadingIndicatorSelector],
  (showMoveWidget, showLoading) => showMoveWidget && !showLoading,
);

export const getShowLoadingIndicator = createSelector(
  [getShowLoadingIndicatorSelector],
  show => show,
);
