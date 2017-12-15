import { combineReducers } from 'redux';
import sidebar from './sidebarReducer';
import componentTree from './componentTreeReducer';

const rootReducer = combineReducers({
  sidebar,
  componentTree,
});

export default rootReducer;
