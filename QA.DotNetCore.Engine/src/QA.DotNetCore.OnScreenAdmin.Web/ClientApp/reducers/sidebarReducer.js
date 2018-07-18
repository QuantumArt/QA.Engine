import _ from 'lodash';
import { getAvailableFeatures } from 'utils/features';
import { ONSCREEN_FEATURES } from 'constants/features';

import {
  TOGGLE_OPEN_STATE,
  TOGGLE_LEFT_POSITION,
  TOGGLE_RIGHT_POSITION,
  TOGGLE_TAB,
  SAVE_CORDS,
} from '../actions/actionTypes';

const availableFeatures = getAvailableFeatures();
let activeTab = 0;
if (_.indexOf(availableFeatures, ONSCREEN_FEATURES.WIDGETS_MANAGEMENT) === -1) { activeTab = 1; }

const initialState = {
  opened: false,
  side: 'left',
  activeTab,
  widgetScreenSearchText: '',
  cords: {
    componentX: 10,
    componentY: 10,
    nodeX: 0,
    nodeY: 0,
  },
};

export default function sidebarReducer(state = initialState, action) {
  switch (action.type) {
    case TOGGLE_OPEN_STATE:
      return { ...state, opened: !state.opened };
    case TOGGLE_LEFT_POSITION:
      return { ...state, side: 'left' };
    case TOGGLE_RIGHT_POSITION:
      return { ...state, side: 'right' };
    case TOGGLE_TAB:
      return { ...state, activeTab: action.value };
    case SAVE_CORDS:
      return { ...state, cords: action.payload };
    default:
      return state;
  }
}
