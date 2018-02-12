import React from 'react';
import PropTypes from 'prop-types';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/lib/integration/react';
import Reboot from 'material-ui/Reboot';
import Sidebar from './containers/sidebar';

const App = ({ store, persistor }) => (
  <Provider store={store}>
    <PersistGate loading={null} persistor={persistor}>
      <Reboot />
      <Sidebar />
    </PersistGate>
  </Provider>
);

App.propTypes = {
  store: PropTypes.object.isRequired, // eslint-disable-line react/forbid-prop-types
  persistor: PropTypes.object.isRequired,
};

export default App;
