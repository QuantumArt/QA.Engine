/* eslint-disable */
import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Toolbar from 'material-ui/Toolbar';
import Paper from 'material-ui/Paper';
import ExpansionPanel, {
  ExpansionPanelSummary,
  ExpansionPanelDetails,
  ExpansionPanelActions,
} from 'material-ui/ExpansionPanel';
import Typography from 'material-ui/Typography';
import Button from 'material-ui/Button';
import Tooltip from 'material-ui/Tooltip';
import ExpandMoreIcon from 'material-ui-icons/ExpandMore';
import PlayArrow from 'material-ui-icons/PlayArrow';
import Pause from 'material-ui-icons/Pause';
import Stop from 'material-ui-icons/Stop';
import TestDetails from './TestDetails';

const styles = theme => ({
  toolBar: {
    padding: '25px 5px 10px',
    overflow: 'hidden',
  },
  paper: {
    width: '100%',
  },
  heading: {
    fontSize: '14px',
    fontWeight: theme.typography.fontWeightRegular,
  },
  panelDetails: {
    flexDirection: 'column',
  },
  panelActions: {
    justifyContent: 'space-around',
    paddingLeft: 150,
    paddingRight: 20,
  },
  actionButton: {
    fontSize: '10px',
  },
  actionButtonLabel: {
    marginTop: 1,
    marginRight: 3,
  },
  actionIcon: {
    // width: 15,
    // height: 15,
  },
  actionTooltip: {
    fontSize: '11px',
  }
});

const AbTestingScreen = ({ classes, tests }) => (
  <Toolbar className={classes.toolBar}>
    <Paper className={classes.paper} elevation={0}>
      {tests.map(test => (
        <ExpansionPanel key={test.id} className={classes.panel}>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            <Typography className={classes.heading}>{test.title}</Typography>
          </ExpansionPanelSummary>
          <ExpansionPanelDetails className={classes.panelDetails}>
            <TestDetails {...test} />
          </ExpansionPanelDetails>
          <ExpansionPanelActions className={classes.panelActions}>
            <Tooltip
              id="stopTest"
              placement="top"
              title="Stop test entirely"
              classes={{tooltip: classes.actionTooltip}}
            >
              <Button color="accent" raised fab mini classes={{label: classes.actionButton}}>
                <Stop className={classes.actionIcon} />
              </Button>
            </Tooltip>
            <Tooltip
              id="pauseTest"
              placement="top"
              title="Stop test for session"
              classes={{tooltip: classes.actionTooltip}}
            >
              <Button color="primary" raised fab mini classes={{label: classes.actionButton}}>
                <Pause className={classes.actionIcon} />
              </Button>
            </Tooltip>
            <Tooltip
              id="runTest"
              placement="top"
              title="Launch test for session"
              classes={{tooltip: classes.actionTooltip}}
            >
              <Button color="primary"  fab mini classes={{label: classes.actionButton}}>
                <PlayArrow className={classes.actionIcon} />
              </Button>
            </Tooltip>
          </ExpansionPanelActions>
        </ExpansionPanel>
      ))}
    </Paper>
  </Toolbar>
);

AbTestingScreen.propTypes = {
  tests: PropTypes.array.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
