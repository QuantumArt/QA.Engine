import { createSelector } from 'reselect';


const getSidebarSideSelector = state => state.sidebar.side;
const getOpenedSelector = state => state.sidebar.opened;


export const getSidebarSide = createSelector(
  [getSidebarSideSelector],
  side => side,
);

export const getOpened = createSelector(
  [getOpenedSelector],
  opened => opened,
);

