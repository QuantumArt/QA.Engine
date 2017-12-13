import React, { Fragment } from 'react';
import EditComponentTree from '../../containers/editComponentTree';
import ComponentHighlightToolbar from '../../containers/componentHighlightToolbar';
import ComponentTreeScreen from '../ComponentTreeScreen';


const WidgetsScreen = () => (
  <Fragment>
    <ComponentHighlightToolbar />
    <ComponentTreeScreen />
    <EditComponentTree />
  </Fragment>
);


export default WidgetsScreen;
