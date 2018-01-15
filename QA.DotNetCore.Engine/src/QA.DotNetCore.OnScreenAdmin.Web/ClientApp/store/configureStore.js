import { createStore, applyMiddleware, compose } from 'redux';
import createSagaMiddleware from 'redux-saga';
import rootReducer from '../reducers';
import rootSaga from '../sagas';


const sagaMiddleware = createSagaMiddleware();

/* eslint-disable no-underscore-dangle, global-require */
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

export default function configureStore() {
  const store = createStore(
    rootReducer,
    {},
    composeEnhancers(
      applyMiddleware(sagaMiddleware),
    ),
  );

  if (module.hot) {
    module.hot.accept('../reducers', () => {
      const nextGlobalReducer = require('../reducers').default;
      store.replaceReducer(nextGlobalReducer);
    });
  }

  sagaMiddleware.run(rootSaga);

  return store;
}