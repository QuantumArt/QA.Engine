import { createStore, applyMiddleware, compose } from 'redux';
import { persistStore, persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import createSagaMiddleware from 'redux-saga';
import { APP_STARTED } from 'actions/actionTypes';
import rootReducer from 'reducers';
import rootSaga from 'sagas';
import mutationWatcher from 'utils/mutationWatcher';

const sagaMiddleware = createSagaMiddleware();
/* eslint-disable no-underscore-dangle, global-require */
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const persistConfig = {
  key: 'root',
  storage,
  whitelist: ['sidebar', 'componentsHighlight'],
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

export default function configureStore() {
  const store = createStore(
    persistedReducer,
    {},
    composeEnhancers(
      applyMiddleware(sagaMiddleware),
    ),
  );
  mutationWatcher(store);

  sagaMiddleware.run(rootSaga);

  store.dispatch({ type: APP_STARTED });

  if (module.hot) {
    module.hot.accept('../reducers', () => {
      const nextGlobalReducer = require('../reducers').default;
      store.replaceReducer(nextGlobalReducer);
    });
  }

  const persistor = persistStore(store);
  return {
    store,
    persistor,
  };
}
