import React from 'react';
import PropTypes from 'prop-types';
import { Provider } from 'react-redux';
// import mutationWatcher from './mutationWatcher';
import Sidebar from './Components/Sidebar';


// let tree = buildTree();
// store.dispatch(loadedComponentTree(tree.components));
// mutationWatcher(store);

const App = ({ store }) => (
  <Provider store={store}>
    <Sidebar />
  </Provider>
);

App.propTypes = {
  store: PropTypes.object.isRequired, // eslint-disable-line react/forbid-prop-types
};

export default App;
