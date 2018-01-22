import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import AvailableWidgetsListStep from './AvailableWidgetsListStep';
import ZonesListStep from './ZonesListStep';
import EnterCustomZoneNameStep from './EnterCustomZoneNameStep';
import WizardHeader from '../WizardHeader';


const WidgetCreationWizard = ({
  showZonesList,
  showEnterCustomZoneName,
  showAvailableWidgets,
  zones,
  onSelectZone,
  onSelectCustomZone,
  onClickBack,
  zonesListSearchText,
  onChangeZonesListSearchText,
  onChangeCustomZoneName,
  customZoneName,
  onSelectWidget,
}) => (
  <Fragment>
    <WizardHeader
      text="Widget creation"
      onClickBack={onClickBack}
    />
    {showZonesList
      ? (<ZonesListStep
        zones={zones}
        onSelectZone={onSelectZone}
        onSelectCustomZone={onSelectCustomZone}
        searchText={zonesListSearchText}
        changeSearchText={onChangeZonesListSearchText}
      />)
      : null
    }
    {showEnterCustomZoneName
      ? (<EnterCustomZoneNameStep
        customZoneName={customZoneName}
        onChangeCustomZoneName={onChangeCustomZoneName}
        onConfirmCustomZoneName={onSelectZone}
      />)
      : null
    }
    {showAvailableWidgets
      ? (<AvailableWidgetsListStep
        onSelectWidget={onSelectWidget}
      />)
      : null
    }
  </Fragment>
);

WidgetCreationWizard.propTypes = {
  zones: PropTypes.arrayOf(
    PropTypes.shape({
      onScreenId: PropTypes.string.isRequired,
      properties: PropTypes.object.isRequired,
    }).isRequired,
  ).isRequired,
  onSelectZone: PropTypes.func.isRequired,
  onSelectCustomZone: PropTypes.func.isRequired,
  onClickBack: PropTypes.func.isRequired,
  showZonesList: PropTypes.bool.isRequired,
  showEnterCustomZoneName: PropTypes.bool.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
  zonesListSearchText: PropTypes.string.isRequired,
  onChangeZonesListSearchText: PropTypes.func.isRequired,
  onChangeCustomZoneName: PropTypes.func.isRequired,
  onSelectWidget: PropTypes.func.isRequired,
  customZoneName: PropTypes.string.isRequired,

};

export default WidgetCreationWizard;
