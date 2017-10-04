import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux'
import mutationWatcher from './mutationWatcher';
import configureStore from './store/configureStore';

// import {createStore} from 'redux';

// import './index.css';
// import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import Sidebar from './Components/Sidebar/Sidebar'

// import registerServiceWorker from './registerServiceWorker';

let store = configureStore();
console.log('configured store');
// let tree = buildTree();
// store.dispatch(loadedComponentTree(tree.components));
mutationWatcher(store);

const App = () => (
  <Provider store={store}>
    <Sidebar />
  </Provider>
);

ReactDOM.render(<App />, document.getElementById('sidebarplaceholder'));
// registerServiceWorker();
