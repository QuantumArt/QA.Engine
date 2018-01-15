import { connect } from 'react-redux';
import AbtestingScreen from 'Components/AbTestingScreen';

const mapStateToProps = ({ abTestingScreen: { avalaibleTests, testsData } }) => ({
  tests: avalaibleTests.map((el, i) => ({ ...el, ...testsData[i] })),
});

const AbtestingScreenContainer = connect(
  mapStateToProps,
  null,
)(AbtestingScreen);

export default AbtestingScreenContainer;
