/* eslint-disable */
import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import Paper from 'material-ui/Paper';
import { CircularProgress } from 'material-ui/Progress';
import { withStyles } from 'material-ui/styles';
import EditComponentTree from 'containers/WidgetsScreen/editComponentTree';
import MoveWidgetScreen from 'containers/WidgetsScreen/moveWidgetsScreen';
import WidgetCreationWizard from 'containers/WidgetsScreen/widgetCreationWizard';
import ComponentTreeScreen from 'containers/WidgetsScreen/ComponentTreeScreen';

const styles = theme => ({
  loadingIndicatorPaper: theme.mixins.gutters({
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    marginTop: '45%',
  }),
});


const WidgetsScreen = ({
  showLoadingIndicator,
  showComponentTree,
  showWidgetCreationWizard,
  showMoveWidgetScreen,
  classes,
}) => (
  <Fragment>
    {showLoadingIndicator &&
      <Paper elevation={0} className={classes.loadingIndicatorPaper}>
        <CircularProgress />
      </Paper>
    }
    {showComponentTree &&
      <ComponentTreeScreen />
    }
    {/* {showWidgetCreationWizard && */}
    {/* <WidgetCreationWizard /> */}
    {/* } */}
    {/* {showMoveWidgetScreen && */}
    {/* <MoveWidgetScreen /> */}
    {/* } */}
    {/* <EditComponentTree /> */}
  </Fragment>
);

WidgetsScreen.propTypes = {
  showComponentTree: PropTypes.bool.isRequired,
  showWidgetCreationWizard: PropTypes.bool.isRequired,
  showMoveWidgetScreen: PropTypes.bool.isRequired,
  showLoadingIndicator: PropTypes.bool.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(WidgetsScreen);
