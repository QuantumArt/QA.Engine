import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';
import metaInfo from './metaInfoReducer';
import widgetsScreen from './widgetsScreenReducer';
import componentsHighlight from './componentsHighlightReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
  metaInfo,
  widgetsScreen,
  componentsHighlight,
});

export default rootReducer;
