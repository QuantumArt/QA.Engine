import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import EditComponentTree from 'containers/WidgetsScreen/editComponentTree';
import MoveWidgetScreen from 'containers/WidgetsScreen/moveWidgetsScreen';
import ComponentHighlightToolbar from 'containers/WidgetsScreen/componentHighlightToolbar';
import ComponentTreeScreen from './ComponentTreeScreen';
import AvailableWidgetsScreen from './AvailableWidgetsScreen';


const WidgetsScreen = ({ showComponentTree, showAvailableWidgets, showMoveWidgetScreen }) => (
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
    {showMoveWidgetScreen
      ? (<MoveWidgetScreen />)
      : null
    }
    <EditComponentTree />
  </Fragment>
);

WidgetsScreen.propTypes = {
  showComponentTree: PropTypes.bool.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
  showMoveWidgetScreen: PropTypes.bool.isRequired,
};

export default WidgetsScreen;
