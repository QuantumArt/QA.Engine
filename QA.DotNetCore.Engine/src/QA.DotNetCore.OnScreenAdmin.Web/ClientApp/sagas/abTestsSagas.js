import _ from 'lodash';
import { getTestsData, setGlobalTestState } from 'api';
import { delay } from 'redux-saga';
import { all, call, put, takeEvery, } from 'redux-saga/effects';
import {
  API_GET_TESTS_DATA_ERROR,
  API_GET_TESTS_DATA_SUCCESS,
  APP_STARTED,
  GET_AVALAIBLE_TESTS,
  LAUNCH_SESSION_TEST,
  LAUNCH_TEST,
  SET_TEST_CASE,
  STOP_SESSION_TEST,
  STOP_TEST,
} from 'actions/actionTypes';
import { isTestsAvailable } from 'utils/features';

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
    'abt-629731': {
      choice: 1,
      cids: [629710, 629749],
      targetedCids: [],
    },
    'abt-629740': {
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
      title: 'Т126. B2C. Вертикальные карточки ВСЁ!',
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
      title: 'CAPSLOCK',
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
      title: 'English text',
      comment: null,
      startDate: null,
      endDate: '2018-01-31T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [],
      }, {
        choice: 1,
        percent: 33.33,
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
      id: 629731,
      enabled: true,
      title: 'Активный тест',
      comment: null,
      startDate: '2018-04-30T19:04:00',
      endDate: '2018-07-30T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [],
      }, {
        choice: 1,
        percent: 33.33,
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
      id: 629740,
      enabled: true,
      title: 'Будущий тест',
      comment: null,
      startDate: '2018-06-30T19:04:00',
      endDate: '2018-07-30T19:04:00',
      variants: [{
        choice: 0,
        percent: 75.0,
        containers: [],
      }, {
        choice: 1,
        percent: 33.33,
        containers: [{
          cid: 629741,
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
    if (fakeEnv && isTestsAvailable()) {
      yield put({ type: API_GET_TESTS_DATA_SUCCESS, payload: fake.api });
    } else if (isTestsAvailable()) {
      const cids = _.reduce(avalaibleTests, (result, value) => (result.concat(value.cids)), []);
      const testsInfo = yield call(getTestsData, cids);
      yield put({ type: API_GET_TESTS_DATA_SUCCESS, payload: testsInfo.data.data });
    }
  } catch (error) {
    yield put({ type: API_GET_TESTS_DATA_ERROR, payload: error });
  }
}

function* launchTest({ testId }) {
  try {
    const response = yield call(setGlobalTestState, testId, true);
    if (response.status === 200) {
      reload();
    } else {
      throw new Error(response.data.errorText);
    }
  } catch (error) {
    console.log(error);
  }
}

function* launchSessionTest({ testId }) {
  yield delay(400); // for button animation
  yield call(window.QA.OnScreen.AbTesting.enableTestForMe, testId);
  reload();
}

function* stopTest({ testId }) {
  try {
    const response = yield call(setGlobalTestState, testId, false);
    if (response.status === 200) {
      reload();
    } else {
      throw new Error(response.data.errorText);
    }
  } catch (error) {
    console.log(error);
  }
}

function* stopSessionTest({ testId }) {
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

function* watchTestLaunch() {
  yield takeEvery(LAUNCH_TEST, launchTest);
}

function* watchSessionTestLaunch() {
  yield takeEvery(LAUNCH_SESSION_TEST, launchSessionTest);
}

function* watchTestStop() {
  yield takeEvery(STOP_TEST, stopTest);
}

function* watchSessionTestStop() {
  yield takeEvery(STOP_SESSION_TEST, stopSessionTest);
}

function* watchCaseChange() {
  yield takeEvery(SET_TEST_CASE, setTestCase);
}

export default function* watchAbTests() {
  yield all([
    watchStart(),
    watchTestLaunch(),
    watchSessionTestLaunch(),
    watchTestStop(),
    watchSessionTestStop(),
    watchCaseChange(),
  ]);
}
