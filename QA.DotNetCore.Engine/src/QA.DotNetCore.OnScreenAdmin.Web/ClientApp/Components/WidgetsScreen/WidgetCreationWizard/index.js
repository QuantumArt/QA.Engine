import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import ZoneTypeSelectStep from './ZoneTypeSelectStep';
import AvailableWidgetsListStep from './AvailableWidgetsListStep';
import ZonesListStep from './ZonesListStep';
import EnterCustomZoneNameStep from './EnterCustomZoneNameStep';
import WizardHeader from '../WizardHeader';


const WidgetCreationWizard = ({
  showZonesList,
  showZoneTypeSelect,
  showEnterCustomZoneName,
  showAvailableWidgets,
  zones,
  onSelectZone,
  onClickBack,
  zonesListSearchText,
  onChangeZonesListSearchText,
  onChangeCustomZoneName,
  customZoneName,
  onSelectWidget,
  onSelectCustomZoneType,
  onSelectExistingZoneType,
  showZonesListSearchBox,
  onToggleZonesListSearchBoxVisibility,
}) => (
  <Fragment>
    <WizardHeader
      text="Widget creation"
      onClickBack={onClickBack}
    />
    {showZoneTypeSelect
      ? (<ZoneTypeSelectStep
        onSelectCustomZone={onSelectCustomZoneType}
        onSelectExistingZone={onSelectExistingZoneType}
      />)
      : null
    }
    {showZonesList
      ? (<ZonesListStep
        zones={zones}
        onSelectZone={onSelectZone}
        onSearchButtonClick={onToggleZonesListSearchBoxVisibility}
        searchText={zonesListSearchText}
        changeSearchText={onChangeZonesListSearchText}
        showZonesListSearchBox={showZonesListSearchBox}
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
  onSelectCustomZoneType: PropTypes.func.isRequired,
  onSelectExistingZoneType: PropTypes.func.isRequired,
  onClickBack: PropTypes.func.isRequired,
  showZoneTypeSelect: PropTypes.bool.isRequired,
  showZonesList: PropTypes.bool.isRequired,
  showEnterCustomZoneName: PropTypes.bool.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
  zonesListSearchText: PropTypes.string.isRequired,
  onChangeZonesListSearchText: PropTypes.func.isRequired,
  showZonesListSearchBox: PropTypes.bool.isRequired,
  onToggleZonesListSearchBoxVisibility: PropTypes.func.isRequired,
  onChangeCustomZoneName: PropTypes.func.isRequired,
  onSelectWidget: PropTypes.func.isRequired,
  customZoneName: PropTypes.string.isRequired,


};

export default WidgetCreationWizard;
