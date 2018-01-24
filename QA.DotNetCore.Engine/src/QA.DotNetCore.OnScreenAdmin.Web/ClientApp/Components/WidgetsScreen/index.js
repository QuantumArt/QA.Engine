import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Button from 'material-ui/Button';
import Toolbar from 'material-ui/Toolbar';
import EditComponentTree from 'containers/WidgetsScreen/editComponentTree';
import MoveWidgetScreen from 'containers/WidgetsScreen/moveWidgetsScreen';
import WidgetCreationWizard from 'containers/WidgetsScreen/widgetCreationWizard';
import ComponentHighlightToolbar from 'containers/WidgetsScreen/componentHighlightToolbar';

import ComponentTreeScreen from './ComponentTreeScreen';


// import AvailableWidgetsScreen from './AvailableWidgetsScreen';
const styles = {
  root: {
    width: '100%',
  },
  flex: {
    flex: 1,
  },
  menuButton: {
    marginLeft: -12,
    marginRight: 20,
  },
};

const WidgetsScreen = ({
  showComponentTree,
  showWidgetCreationWizard,
  showMoveWidgetScreen,
  addWidgetToPage,
  classes,
}) => (
  <Fragment>
    {showComponentTree
      ? (
        <Fragment>
          <ComponentHighlightToolbar />
          <Toolbar>
            <Button raised onClick={addWidgetToPage} classes={classes.menuButton}>Add widget</Button>
          </Toolbar>
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
  addWidgetToPage: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WidgetsScreen);
