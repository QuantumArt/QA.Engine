import { connect } from 'react-redux';
import AvailableWidgetsList from 'Components/WidgetsScreen/WidgetCreationWizard/AvailableWidgetsListStep/AvailableWidgetsList';
import { filteredAvailableWidgets } from 'selectors/availableWidgets';

const mapStateToProps = state => ({
  availableWidgets: filteredAvailableWidgets(state),
});

// const mapDispatchToProps = (dispatch, ownProps) => ({
//   onSelectWidget: (id) => {
//     dispatch(ownProps.onSelectWidget(id));
//   },
// });

const AvailableWidgetsListContainer = connect(
  mapStateToProps,
  // mapDispatchToProps,
)(AvailableWidgetsList);

export default AvailableWidgetsListContainer;
