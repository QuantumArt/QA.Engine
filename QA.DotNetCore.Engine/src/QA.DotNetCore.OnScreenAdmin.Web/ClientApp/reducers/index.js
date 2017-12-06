import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';
import metaInfo from './metaInfoReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
  metaInfo,
});

export default rootReducer;
