import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import ZonesListStep from './ZonesListStep';
import WizardHeader from '../WizardHeader';


const WidgetCreationWizard = ({
  showZonesList,
  zones,
  onSelectZone,
  onSelectCustomZone,
  onClickBack,
  zonesListSearchText,
  onChangeZonesListSearchText,
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
  zonesListSearchText: PropTypes.string.isRequired,
  onChangeZonesListSearchText: PropTypes.func.isRequired,
};

export default WidgetCreationWizard;
