import { connect } from 'react-redux';
import AvailableWidgetsSearch from 'Components/WidgetsScreen/WidgetCreationWizard/AvailableWidgetsListStep/AvailableWidgetsSearch';
import { getSearchText } from 'selectors/availableWidgets';
import { changeSearchText } from 'actions/availableWidgetsActions';

const mapStateToProps = state => ({
  searchText: getSearchText(state),
});

const mapDispatchToProps = dispatch => ({
  changeSearchText: (event) => {
    dispatch(changeSearchText(event.target.value));
  },
});

const AvailableWidgetsSearchContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(AvailableWidgetsSearch);

export default AvailableWidgetsSearchContainer;
