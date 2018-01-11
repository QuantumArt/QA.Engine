import { connect } from 'react-redux';
import AbtestingScreen from 'Components/AbTestingScreen';

const mapStateToProps = state => ({
  testsList: state.abTestingScreen.avalaibleTests,
});

const AbtestingScreenContainer = connect(
  mapStateToProps,
  null,
)(AbtestingScreen);

export default AbtestingScreenContainer;
