import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import EditComponentTree from 'containers/WidgetsScreen/editComponentTree';
import MoveWidgetScreen from 'containers/WidgetsScreen/moveWidgetsScreen';
import WidgetCreationWizard from 'containers/WidgetsScreen/widgetCreationWizard';
// import ComponentHighlightToolbar from 'containers/WidgetsScreen/componentHighlightToolbar';

import ComponentTreeScreen from './ComponentTreeScreen';


const WidgetsScreen = ({
  showComponentTree,
  showWidgetCreationWizard,
  showMoveWidgetScreen,

}) => (
  <Fragment>
    {showComponentTree
      ? (
        <Fragment>

          <ComponentTreeScreen />
        </Fragment>
      )
      : null
    }
    {showWidgetCreationWizard
      ? (<WidgetCreationWizard />)
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
  showWidgetCreationWizard: PropTypes.bool.isRequired,
  showMoveWidgetScreen: PropTypes.bool.isRequired,

};

export default WidgetsScreen;
