import { connect } from 'react-redux';
import {
  launchTest,
  launchSessionTest,
  stopTest,
  pauseTest,
  setTestCase,
} from 'actions/abTestingScreenActions';
import AbtestingScreen from 'Components/AbTestingScreen';

const mapStateToProps = ({ abTestingScreen }) => {
  const { avalaibleTests, testsData } = abTestingScreen;

  if (testsData.length > 0) {
    const combined = avalaibleTests.map((el, i) => {
      const states = {
        globalActive: el.choice !== null && testsData[i].enabled,
        sessionActive: el.choice !== null && !testsData[i].enabled,
        paused: el.choice === null && testsData[i].enabled,
        stoped: el.choice === null && !testsData[i].enabled,
      };

      return { ...el, ...testsData[i], ...states };
    });

    return { tests: combined };
  }

  return { tests: [] };
};

const mapDispatchToProps = dispatch => ({
  launchSessionTest: (testId) => {
    dispatch(launchSessionTest(testId));
  },
  launchTest: (testId) => {
    dispatch(launchTest(testId));
  },
  stopTest: (testId) => {
    dispatch(stopTest(testId));
  },
  pauseTest: (testId) => {
    dispatch(pauseTest(testId));
  },
  setTestCase: (testId, value) => {
    dispatch(setTestCase(testId, value));
  },
});

const AbtestingScreenContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(AbtestingScreen);

export default AbtestingScreenContainer;
