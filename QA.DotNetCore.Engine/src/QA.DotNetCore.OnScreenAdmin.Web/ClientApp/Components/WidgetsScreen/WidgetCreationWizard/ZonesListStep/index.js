
import React, { Fragment } from 'react';

import PropTypes from 'prop-types';

import List from 'material-ui/List';
import SearchToolbar from 'Components/SearchToolBar';
import ZoneListItem from './zoneListItem';
import WizardSubheader from '../../WizardSubheader';


const ZonesListStep = ({ zones, onSelectZone, onSelectCustomZone, searchText, changeSearchText }) => (
  <Fragment>
    <WizardSubheader text="Select target zone for new widget" />
    <SearchToolbar
      searchText={searchText}
      changeSearchText={changeSearchText}
    />
    <List>
      {zones.map(zone => (
        <ZoneListItem
          key={zone.onScreenId}
          zoneName={zone.properties.zoneName}
          onSelectZone={onSelectZone}
        />
      ))}
      <ZoneListItem
        key="custom_zone"
        zoneName="Custom zone"
        onSelectZone={onSelectCustomZone}
      />
    </List>
  </Fragment>
);

ZonesListStep.propTypes = {
  zones: PropTypes.arrayOf(
    PropTypes.shape({
      onScreenId: PropTypes.string.isRequired,
      properties: PropTypes.object.isRequired,
    }).isRequired,
  ).isRequired,
  onSelectZone: PropTypes.func.isRequired,
  onSelectCustomZone: PropTypes.func.isRequired,
  searchText: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
};

export default ZonesListStep;
