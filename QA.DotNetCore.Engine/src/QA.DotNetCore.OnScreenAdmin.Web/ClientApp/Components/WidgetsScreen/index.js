import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import x from '';
import EditComponentTree from 'containers/WidgetsScreen/editComponentTree';
import ComponentHighlightToolbar from 'containers/WidgetsScreen/componentHighlightToolbar';
import ComponentTreeScreen from './ComponentTreeScreen';
import AvailableWidgetsScreen from './AvailableWidgetsScreen';


const WidgetsScreen = ({ showComponentTree, showAvailableWidgets }) => (
  <Fragment>
    <ComponentHighlightToolbar />
    {showComponentTree
      ? (<ComponentTreeScreen />)
      : null
    }
    {showAvailableWidgets
      ? (<AvailableWidgetsScreen />)
      : null
    }
    <EditComponentTree />
  </Fragment>
);

WidgetsScreen.propTypes = {
  showComponentTree: PropTypes.bool.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
};

export default WidgetsScreen;
