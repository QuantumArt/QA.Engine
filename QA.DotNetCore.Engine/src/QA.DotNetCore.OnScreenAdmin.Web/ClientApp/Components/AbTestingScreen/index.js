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
import IconButton from 'material-ui/IconButton';
import Tooltip from 'material-ui/Tooltip';
import Icon from 'material-ui/Icon';
import ExpandMoreIcon from 'material-ui-icons/ExpandMore';
import PlayArrow from 'material-ui-icons/PlayArrow';
import Pause from 'material-ui-icons/Pause';
import Stop from 'material-ui-icons/Stop';
import TestDetails from './TestDetails';
import { green } from 'material-ui/colors';

const styles = theme => ({
  toolBar: {
    padding: '25px 5px 10px',
    overflow: 'hidden',
  },
  paper: {
    width: '100%',
  },
  heading: {
    fontSize: 16,
    fontWeight: theme.typography.fontWeightMedium,
  },
  headingDetails: {
    fontSize: 14,
    fontWeight: theme.typography.fontWeightMedium,
    marginLeft: 16,
    lineHeight: 1.4,
  },
  statusIcon: {
    width: 16,
    height: 16,
    marginRight: 24,
  },
  panelDetails: {
    flexDirection: 'column',
  },
  actionsWrap: {
    justifyContent: 'space-around',
  },
  actionButton: {
    fontSize: '10px',
  },
  actionButtonLabel: {
    marginTop: 1,
    marginRight: 3,
  },
  actionIcon: {
    width: 23,
    height: 23,
  },
  actionTooltip: {
    fontSize: '11px',
  },
  sessionColor: {
    color: green[400],
  }
});

const AbTestingScreen = (props) => {
  const {classes, tests} = props;
  const renderPauseButton = () => (
    <Tooltip
      id="pauseTest"
      placement="top"
      title="Stop test for session"
      classes={{tooltip: classes.actionTooltip}}
    >
      <IconButton
        color="primary"
        classes={{
          label: classes.actionButton,
          colorPrimary: classes.sessionColor,
        }}
      >
        <Pause className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderStopButton = () => (
    <Tooltip
      id="stopTest"
      placement="top"
      title="Stop test entirely"
      classes={{tooltip: classes.actionTooltip}}
    >
      <IconButton color="accent" classes={{label: classes.actionButton}}>
        <Stop className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderGlobalLaunchButton = () => (
    <Tooltip
      id="runTestforSession"
      placement="top"
      title="Launch test"
      classes={{tooltip: classes.actionTooltip}}
    >
      <IconButton
        color="primary"
        classes={{label: classes.actionButton}}
      >
        <PlayArrow className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderSessionLaunchButton = () => (
    <Tooltip
      id="runTest"
      placement="top"
      title="Launch test for session"
      classes={{tooltip: classes.actionTooltip}}
    >
      <IconButton
        color="primary"
        classes={{
          label: classes.actionButton,
          colorPrimary: classes.sessionColor,
        }}
      >
        <PlayArrow className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  console.log(tests);

  return (
    <Toolbar className={classes.toolBar}>
      <Paper className={classes.paper} elevation={0}>
        {tests.map(test => (
          <ExpansionPanel key={test.id} className={classes.panel}>
            <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
              {test.globalActive &&
                <PlayArrow color="primary" className={classes.statusIcon} />
              }
              {test.sessionActive &&
                <PlayArrow style={{color: green[400]}} className={classes.statusIcon} />
              }
              {test.paused &&
                <Pause style={{color: green[400]}} className={classes.statusIcon} />
              }
              {test.stoped &&
                <Stop color="accent" className={classes.statusIcon} />
              }
              <Typography type="title" className={classes.heading} >{test.title}</Typography>
              <Typography type="title" className={classes.headingDetails}>
                {test.choice !== null ? `# ${test.choice}` : ''}
              </Typography>
            </ExpansionPanelSummary>
            <ExpansionPanelDetails className={classes.panelDetails}>
              <TestDetails {...test} />
            </ExpansionPanelDetails>
            <ExpansionPanelActions>
              <Paper elevation={0}>
                {test.globalActive && [
                  renderStopButton(),
                  renderPauseButton(),
                  renderSessionLaunchButton(),
                ]}
                {test.sessionActive && [
                  renderStopButton(),
                  renderPauseButton(),
                  renderGlobalLaunchButton(),
                ]}
                {test.paused && [
                  renderStopButton(),
                  renderSessionLaunchButton(),
                  renderGlobalLaunchButton(),
                ]}
                {test.stoped && [
                  renderSessionLaunchButton(),
                  renderGlobalLaunchButton(),
                ]}
              </Paper>
            </ExpansionPanelActions>
          </ExpansionPanel>
        ))}
      </Paper>
    </Toolbar>
  )
};

AbTestingScreen.propTypes = {
  tests: PropTypes.array.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
