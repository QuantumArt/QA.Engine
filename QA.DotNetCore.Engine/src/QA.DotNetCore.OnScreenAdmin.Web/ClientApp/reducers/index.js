import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';
import metaInfo from './metaInfoReducer';
import widgetsScreen from './widgetsScreenReducer';
import componentsHighlight from './componentsHighlightReducer';
import availableWidgets from './availableWidgetsReducer';
import articleManagement from './articleManagementReducer';
import abTestingScreen from './abTestingScreenReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
  metaInfo,
  widgetsScreen,
  abTestingScreen,
  componentsHighlight,
  availableWidgets,
  articleManagement,
});

export default rootReducer;
