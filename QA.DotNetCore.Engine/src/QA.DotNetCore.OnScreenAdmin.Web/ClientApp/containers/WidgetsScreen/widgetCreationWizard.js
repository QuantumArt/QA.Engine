import { connect } from 'react-redux';
import {
  selectCustomZoneType,
  selectExistingZoneType,
  selectTargetZone,
  goToPrevStep,
  changeZonesListSearchText,
  changeCustomZoneName,
  selectWidget,
  toggleZonesListSearchBox,
  toggleAvailableWidgetsSearchBox,
} from 'actions/widgetCreation/actions';

import {
  getShowZonesList,
  getShowZoneTypeSelect,
  getZonesList,
  getShowAvailableWidgets,
  getZonesListSearchText,
  getShowEnterCustomZoneName,
  getCustomZoneName,
  getShowZonesListSearchBox,
  getShowAvailableWidgetsSearchBox,
} from 'selectors/widgetCreation';

import WidgetCreationWizard from 'Components/WidgetsScreen/WidgetCreationWizard';

const mapStateToProps = state => ({
  showZoneTypeSelect: getShowZoneTypeSelect(state),
  showZonesList: getShowZonesList(state),
  zonesListSearchText: getZonesListSearchText(state),
  showEnterCustomZoneName: getShowEnterCustomZoneName(state),
  showAvailableWidgets: getShowAvailableWidgets(state),
  customZoneName: getCustomZoneName(state),
  showZonesListSearchBox: getShowZonesListSearchBox(state),
  showAvailableWidgetsSearchBox: getShowAvailableWidgetsSearchBox(state),
  zones: getZonesList(state),
});


const mapDispatchToProps = dispatch => ({
  onSelectZone: (targetZoneName) => {
    console.log('onSelectZone');
    dispatch(selectTargetZone(targetZoneName));
  },
  onSelectCustomZoneType: () => {
    dispatch(selectCustomZoneType());
  },
  onSelectExistingZoneType: () => {
    dispatch(selectExistingZoneType());
  },
  onClickBack: () => {
    dispatch(goToPrevStep());
  },
  onChangeZonesListSearchText: (event) => {
    dispatch(changeZonesListSearchText(event.target.value));
  },
  onChangeCustomZoneName: (event) => {
    dispatch(changeCustomZoneName(event.target.value));
  },
  onSelectWidget: (id) => {
    dispatch(selectWidget(id));
  },
  onToggleZonesListSearchBoxVisibility: () => {
    dispatch(toggleZonesListSearchBox());
  },
  onToggleAvailableWidgetsSearchBox: () => {
    dispatch(toggleAvailableWidgetsSearchBox());
  },


});

const WidgetCreationWizardContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(WidgetCreationWizard);

export default WidgetCreationWizardContainer;
