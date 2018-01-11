import { put, takeEvery, all } from 'redux-saga/effects';
import { GET_AVALAIBLE_TESTS, TOGGLE_TAB } from 'actions/actionTypes';

// worker
function* loadTestsData(targetIndex, { value }) {
  if (targetIndex === value) {
  // yield put({ type: GET_AVALAIBLE_TESTS, payload: window.abTestingContext });
    yield put({ type: GET_AVALAIBLE_TESTS,
      payload: {
        'abt-629727': {
          choice: 0,
          title: 'Первый тест',
          comment: 'Первый пробный тест',
          percentage: [3, 1],
        },
        'abt-629717': {
          choice: 0,
          title: 'Второй тест',
          comment: 'Sed pop-up assumenda, Shoreditch letterpress PBR&B American Apparel cray drinking vinegar polaroid deep v four loko umami try-hard.',
          percentage: [3, 1],
        },
      },
    });
  }
}

// watcher
function* watchTab(targetIndex) {
  yield takeEvery(TOGGLE_TAB, loadTestsData, targetIndex);
}

export default function* watchAbTests() {
  yield all([
    watchTab(1),
  ]);
}
