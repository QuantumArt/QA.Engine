import React, { Fragment } from 'react';
import AvailableWidgetsList from '../../../containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsList';
import AvailableWidgetsSearch from '../../../containers/WidgetsScreen/AvailableWidgetsScreen/availableWidgetsSearch';


const AvailableWidgetsScreen = () => (
  <Fragment>
    <AvailableWidgetsSearch />
    <AvailableWidgetsList />
  </Fragment>
);

export default AvailableWidgetsScreen;
