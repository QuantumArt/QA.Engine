import { connect } from 'react-redux';
import {
  selectCustomZone,
  selectTargetZone,
  goToPrevStep,
  changeZonesListSearchText,
} from 'actions/widgetCreation/actions';

import { getShowZonesList, getZonesList, getZonesListSearchText } from 'selectors/widgetCreation';

import WidgetCreationWizard from 'Components/WidgetsScreen/WidgetCreationWizard';

const mapStateToProps = state => ({
  showZonesList: getShowZonesList(state),
  zonesListSearchText: getZonesListSearchText(state),
  zones: getZonesList(state),
});


const mapDispatchToProps = dispatch => ({
  onSelectZone: (targetZoneName) => {
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
});

const WidgetCreationWizardContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(WidgetCreationWizard);

export default WidgetCreationWizardContainer;
