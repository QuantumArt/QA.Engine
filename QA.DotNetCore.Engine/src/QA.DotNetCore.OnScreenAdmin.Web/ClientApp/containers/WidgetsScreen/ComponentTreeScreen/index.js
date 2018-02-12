import { connect } from 'react-redux';
import ComponentTreeScreen from 'Components/WidgetsScreen/ComponentTreeScreen';
import { getShowSearchBox, getShowOnlyWidgets } from 'selectors/componentTree';
import { toggleComponentTreeSearchBox } from 'actions/componentTreeActions';

const mapStateToProps = state => ({
  showSearchBox: getShowSearchBox(state),
  showOnlyWidgets: getShowOnlyWidgets(state),
});

const mapDispatchToProps = dispatch => ({
  toggleSearchBoxVisibility: () => {
    dispatch(toggleComponentTreeSearchBox());
  },
});

const ComponentTreeScreenContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTreeScreen);

export default ComponentTreeScreenContainer;
