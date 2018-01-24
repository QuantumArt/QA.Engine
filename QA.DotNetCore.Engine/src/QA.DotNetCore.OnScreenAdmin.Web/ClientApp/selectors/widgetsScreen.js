import { createSelector } from 'reselect';
import { MODES } from 'reducers/widgetsScreenReducer';

const getShowComponentTreeSelector = state => state.widgetsScreen.mode === MODES.SHOW_COMPONENT_TREE;
const getShowWidgetsCreationWizardSelector = state => state.widgetsScreen.mode === MODES.SHOW_WIDGET_CREATION_WIZARD;
const getShowMoveWidgetScreenSelector = state => state.widgetsScreen.mode === MODES.SHOW_MOVE_WIDGET;

export const getShowComponentTree = createSelector(
  [getShowComponentTreeSelector],
  showComponentTree => showComponentTree,
);

export const getShowWidgetsCreationWizard = createSelector(
  [getShowWidgetsCreationWizardSelector],
  showWidgetsCreationWizard => showWidgetsCreationWizard,
);

export const getShowMoveWidgetScreen = createSelector(
  [getShowMoveWidgetScreenSelector],
  showMoveWidget => showMoveWidget,
);
