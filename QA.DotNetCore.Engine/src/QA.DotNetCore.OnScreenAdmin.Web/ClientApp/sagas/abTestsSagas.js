import {
  put,
  takeEvery,
  all,
} from 'redux-saga/effects';
import {
  GET_AVALAIBLE_TESTS,
  API_GET_TESTS_DATA,
  TOGGLE_TAB,
} from 'actions/actionTypes';

const fake = {
  window: {
    'abt-629727': {
      choice: 0,
      cids: [629728, 629734],
      targetedCids: [629728, 629734],
    },
    'abt-629728': {
      choice: 1,
      cids: [],
      targetedCids: [],
    },
    'abt-629729': {
      choice: null,
      cids: [],
      targetedCids: [],
    },
    'abt-629730': {
      choice: null,
      cids: [],
      targetedCids: [],
    },
  },
  api: [
    {
      id: 629727,
      enabled: true,
      title: 'Первый тест',
      comment: 'Первый пробный тест',
      startDate: null,
      endDate: '2018-01-31T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 0',
          variantId: 629730,
          type: 'Script',
        }, {
          cid: 629734,
          containerDescription: 'Смена цвета шапки',
          variantDescription: 'делаем красной шапку',
          variantId: 629736,
          type: 'Script',
        }],
      }, {
        choice: 1,
        percent: 25.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 1',
          variantId: 629731,
          type: 'Script',
        }],
      }],
    },
    {
      id: 629728,
      enabled: false,
      title: 'Второй тест',
      comment: 'Второй пробный тест',
      startDate: null,
      endDate: '2018-01-31T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 0',
          variantId: 629730,
          type: 'Script',
        }, {
          cid: 629734,
          containerDescription: 'Смена цвета шапки',
          variantDescription: 'делаем красной шапку',
          variantId: 629736,
          type: 'Script',
        }],
      }, {
        choice: 1,
        percent: 25.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 1',
          variantId: 629731,
          type: 'Script',
        }],
      }],
    },
    {
      id: 629729,
      enabled: true,
      title: 'Третий тест',
      comment: 'Третий пробный тест',
      startDate: null,
      endDate: '2018-01-31T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 0',
          variantId: 629730,
          type: 'Script',
        }, {
          cid: 629734,
          containerDescription: 'Смена цвета шапки',
          variantDescription: 'делаем красной шапку',
          variantId: 629736,
          type: 'Script',
        }],
      }, {
        choice: 1,
        percent: 25.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 1',
          variantId: 629731,
          type: 'Script',
        }],
      }],
    },
    {
      id: 629730,
      enabled: false,
      title: 'Четвертый тест',
      comment: 'Четвертый пробный тест',
      startDate: null,
      endDate: '2018-01-31T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 0',
          variantId: 629730,
          type: 'Script',
        }, {
          cid: 629734,
          containerDescription: 'Смена цвета шапки',
          variantDescription: 'делаем красной шапку',
          variantId: 629736,
          type: 'Script',
        }],
      }, {
        choice: 1,
        percent: 25.0,
        containers: [{
          cid: 629728,
          containerDescription: 'Пустышка, пишет в консоль',
          variantDescription: 'console.log ab test choice 1',
          variantId: 629731,
          type: 'Script',
        }],
      }],
    },
  ],
};

// worker
function* loadTestsData(targetIndex, { value }) {
  if (targetIndex === value) {
    // yield put({ type: GET_AVALAIBLE_TESTS, payload: window.abTestingContext });
    yield put({ type: GET_AVALAIBLE_TESTS, payload: fake.window });
    yield put({ type: API_GET_TESTS_DATA, payload: fake.api });
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
