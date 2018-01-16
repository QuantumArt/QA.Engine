import { connect } from 'react-redux';
import AbtestingScreen from 'Components/AbTestingScreen';

const mapStateToProps = ({ abTestingScreen }) => {
  const { avalaibleTests, testsData } = abTestingScreen;
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
};

const AbtestingScreenContainer = connect(
  mapStateToProps,
  null,
)(AbtestingScreen);

export default AbtestingScreenContainer;
