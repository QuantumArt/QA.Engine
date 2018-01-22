import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import AvailableWidgetsList from 'containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsList';
import AvailableWidgetsSearch from 'containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsSearch';


const AvailableWidgetsScreen = ({ onSelectWidget }) => (
  <Fragment>
    <AvailableWidgetsSearch />
    <AvailableWidgetsList
      onSelectWidget={onSelectWidget}
    />
  </Fragment>
);

AvailableWidgetsScreen.propTypes = {
  onSelectWidget: PropTypes.func.isRequired,
};

export default AvailableWidgetsScreen;
