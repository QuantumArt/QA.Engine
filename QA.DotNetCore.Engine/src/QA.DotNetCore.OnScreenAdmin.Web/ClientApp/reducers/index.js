import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';
import metaInfo from './metaInfoReducer';
import widgetsScreen from './widgetsScreenReducer';
import componentsHighlight from './componentsHighlightReducer';
import availableWidgets from './availableWidgetsReducer';
import articleManagement from './articleManagementReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
  metaInfo,
  widgetsScreen,
  componentsHighlight,
  availableWidgets,
  articleManagement,
});

export default rootReducer;
