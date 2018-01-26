import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';
import metaInfo from './metaInfoReducer';
import widgetsScreen from './widgetsScreenReducer';
import componentsHighlight from './componentsHighlightReducer';
import availableWidgets from './availableWidgetsReducer';
import articleManagement from './articleManagementReducer';
import abTestingScreen from './abTestingScreenReducer';
import widgetCreation from './widgetCreationWizardReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
  metaInfo,
  widgetsScreen,
  abTestingScreen,
  componentsHighlight,
  availableWidgets,
  articleManagement,
  widgetCreation,
});

export default rootReducer;
