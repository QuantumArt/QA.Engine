import { connect } from 'react-redux';
import AvailableWidgetsList from '../../../Components/WidgetsScreen/AvailableWidgetsScreen/AvailableWidgetsList';
import { selectWidgetToAdd } from '../../../actions/availableWidgetsActions';
import { hideAvailableWidgets } from '../../../actions/widgetsScreenModeActions';
import { filteredAvailableWidgets } from '../../../selectors/availableWidgets';

const mapStateToProps = state => ({
  availableWidgets: filteredAvailableWidgets(state),
});

const mapDispatchToProps = dispatch => ({
  onSelectWidget: (id) => {
    dispatch(selectWidgetToAdd(id));
  },
  onCancel: () => {
    dispatch(hideAvailableWidgets());
  },
});

const AvailableWidgetsListContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(AvailableWidgetsList);

export default AvailableWidgetsListContainer;
