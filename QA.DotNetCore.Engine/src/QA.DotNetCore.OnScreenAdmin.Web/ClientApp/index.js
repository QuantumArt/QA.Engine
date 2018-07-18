import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader'; // eslint-disable-line import/no-extraneous-dependencies
import configureStore from './store/configureStore';
import App from './App';
import './qpInteraction/QP8BackendApi.Interaction';
import init from './init';

const { store, persistor } = configureStore();
const render = (Root) => {
  ReactDOM.render(
    <AppContainer>
      <Root store={store} persistor={persistor} />
    </AppContainer>,
    document.getElementById('sidebarplaceholder'),
  );
};

init();
render(App);

if (module.hot) {
  module.hot.accept('./App', () => {
    render(App);
  });
}
