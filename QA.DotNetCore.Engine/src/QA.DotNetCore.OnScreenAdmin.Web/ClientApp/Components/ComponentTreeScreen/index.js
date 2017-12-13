import React, { Fragment } from 'react';
import ComponentTree from '../../containers/componentTree';
import ComponentTreeSearch from '../../containers/componentTreeSearch';

const ComponentTreeScreen = () => (
  <Fragment>
    <ComponentTreeSearch />
    <ComponentTree />
  </Fragment>
);

export default ComponentTreeScreen;
