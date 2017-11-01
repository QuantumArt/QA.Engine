import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import rootReducer from '../reducers';

/* eslint-disable no-underscore-dangle, global-require */
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

export default function configureStore() {
  const store = createStore(
    rootReducer,
    {},
    composeEnhancers(
      applyMiddleware(thunk),
    ),
  );

  if (module.hot) {
    module.hot.accept('../reducers', () => {
      const nextGlobalReducer = require('../reducers').default;
      store.replaceReducer(nextGlobalReducer);
    });
  }

  return store;
}
