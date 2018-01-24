import _ from 'lodash';
import { getTestsData } from 'api';
import { delay } from 'redux-saga';
import {
  put,
  takeEvery,
  call,
  all,
} from 'redux-saga/effects';
import {
  APP_STARTED,
  GET_AVALAIBLE_TESTS,
  API_GET_TESTS_DATA_SUCCESS,
  API_GET_TESTS_DATA_ERROR,
  LAUNCH_SESSION_TEST,
  PAUSE_TEST,
  SET_TEST_CASE,
} from 'actions/actionTypes';

function reload() {
  window.location.reload();
}

/* eslint-disable no-unused-vars */
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
      cids: [629700, 629745],
      targetedCids: [],
    },
    'abt-629730': {
      choice: null,
      cids: [629710, 629749],
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
        containers: [],
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
/* eslint-enable no-unused-vars */

// workers
function* loadTestsData() {
  const fakeEnv = process.env.NODE_ENV !== 'production' && window.location.port === '5000';
  const avalaibleTests = fakeEnv ? fake.window : window.abTestingContext;

  try {
    yield put({ type: GET_AVALAIBLE_TESTS, payload: avalaibleTests });
    if (fakeEnv) {
      yield put({ type: API_GET_TESTS_DATA_SUCCESS, payload: fake.api });
    } else {
      const cids = _.reduce(avalaibleTests, (result, value) => (result.concat(value.cids)), []);
      const testsInfo = yield call(getTestsData, cids);
      yield put({ type: API_GET_TESTS_DATA_SUCCESS, payload: testsInfo.data.data });
    }
  } catch (error) {
    console.log(error);
    yield put({ type: API_GET_TESTS_DATA_ERROR, payload: error });
  }
}

function* launchSessionTest({ testId }) {
  yield delay(400); // for button animation
  yield call(window.QA.OnScreen.AbTesting.enableTestForMe, testId);
  reload();
}

function* pauseTest({ testId }) {
  yield delay(400);
  yield call(window.QA.OnScreen.AbTesting.disableTestForMe, testId);
  reload();
}

function* setTestCase({ payload: { testId, value } }) {
  yield delay(400);
  yield call(window.QA.OnScreen.AbTesting.enableTestForMe, testId);
  yield call(window.QA.OnScreen.AbTesting.setChoice, testId, value);
  reload();
}


// watchers
function* watchStart() {
  yield takeEvery(APP_STARTED, loadTestsData);
}

function* watchSessionLaunch() {
  yield takeEvery(LAUNCH_SESSION_TEST, launchSessionTest);
}

function* watchPause() {
  yield takeEvery(PAUSE_TEST, pauseTest);
}

function* watchCaseChange() {
  yield takeEvery(SET_TEST_CASE, setTestCase);
}

export default function* watchAbTests() {
  yield all([
    watchStart(),
    watchSessionLaunch(),
    watchPause(),
    watchCaseChange(),
  ]);
}
