import React, { Fragment } from 'react';
import ComponentTree from '../../containers/WidgetsScreen/ComponentTreeScreen/componentTree';
import ComponentTreeSearch from '../../containers/WidgetsScreen/ComponentTreeScreen/componentTreeSearch';

const ComponentTreeScreen = () => (
  <Fragment>
    <ComponentTreeSearch />
    <ComponentTree />
  </Fragment>
);

export default ComponentTreeScreen;
