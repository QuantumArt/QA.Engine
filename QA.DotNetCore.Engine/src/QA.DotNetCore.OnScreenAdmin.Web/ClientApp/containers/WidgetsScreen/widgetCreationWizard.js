import { connect } from 'react-redux';
import {
  selectCustomZone,
  selectTargetZone,
  goToPrevStep,
  changeZonesListSearchText,
  changeCustomZoneName,
  selectWidget,
} from 'actions/widgetCreation/actions';

import {
  getShowZonesList,
  getZonesList,
  getShowAvailableWidgets,
  getZonesListSearchText,
  getShowEnterCustomZoneName,
  getCustomZoneName,
} from 'selectors/widgetCreation';

import WidgetCreationWizard from 'Components/WidgetsScreen/WidgetCreationWizard';

const mapStateToProps = state => ({
  showZonesList: getShowZonesList(state),
  zonesListSearchText: getZonesListSearchText(state),
  showEnterCustomZoneName: getShowEnterCustomZoneName(state),
  showAvailableWidgets: getShowAvailableWidgets(state),
  customZoneName: getCustomZoneName(state),
  zones: getZonesList(state),
});


const mapDispatchToProps = dispatch => ({
  onSelectZone: (targetZoneName) => {
    console.log('onSelectZone');
    dispatch(selectTargetZone(targetZoneName));
  },
  onSelectCustomZone: () => {
    dispatch(selectCustomZone());
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


});

const WidgetCreationWizardContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(WidgetCreationWizard);

export default WidgetCreationWizardContainer;
