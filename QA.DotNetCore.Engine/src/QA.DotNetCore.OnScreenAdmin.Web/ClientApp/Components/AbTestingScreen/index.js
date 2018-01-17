import React from 'react';
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
// import Button from 'material-ui/Button';
import IconButton from 'material-ui/IconButton';
import Tooltip from 'material-ui/Tooltip';
import ExpandMoreIcon from 'material-ui-icons/ExpandMore';
import PlayArrow from 'material-ui-icons/PlayArrow';
import Pause from 'material-ui-icons/Pause';
import Stop from 'material-ui-icons/Stop';
import { green } from 'material-ui/colors';
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
    fontSize: 16,
    fontWeight: theme.typography.fontWeightMedium,
  },
  subHeading: {
    fontSize: 11,
    marginLeft: 1,
    marginTop: 3,
    fontStyle: 'italic',
  },
  statusIcon: {
    width: 18,
    height: 18,
    marginRight: 24,
    marginTop: 9,
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
  },
});

const AbTestingScreen = (props) => {
  const {
    classes,
    tests,
    launchSessionTest,
    pauseTest,
  } = props;

  const renderPauseButton = (testId, key) => (
    <Tooltip
      id="pauseTest"
      placement="top"
      title="Stop test for session"
      classes={{ tooltip: classes.actionTooltip }}
      key={key}
    >
      <IconButton
        color="primary"
        classes={{
          label: classes.actionButton,
          colorPrimary: classes.sessionColor,
        }}
        onClick={() => { pauseTest(testId); }}
      >
        <Pause className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderStopButton = (testId, key) => (
    <Tooltip
      id="stopTest"
      placement="top"
      title="Stop test entirely"
      classes={{ tooltip: classes.actionTooltip }}
      key={key}
    >
      <IconButton color="accent" classes={{ label: classes.actionButton }}>
        <Stop className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderGlobalLaunchButton = (testId, key) => (
    <Tooltip
      id="runTestforSession"
      placement="top"
      title="Launch test"
      classes={{ tooltip: classes.actionTooltip }}
      key={key}
    >
      <IconButton
        color="primary"
        classes={{ label: classes.actionButton }}
      >
        <PlayArrow className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderSessionLaunchButton = (testId, key) => (
    <Tooltip
      id="runTest"
      placement="top"
      title="Launch test for session"
      classes={{ tooltip: classes.actionTooltip }}
      key={key}
    >
      <IconButton
        color="primary"
        classes={{
          label: classes.actionButton,
          colorPrimary: classes.sessionColor,
        }}
        onClick={() => { launchSessionTest(testId); }}
      >
        <PlayArrow className={classes.actionIcon} />
      </IconButton>
    </Tooltip>
  );
  const renderSummaryText = (test) => {
    if (test.choice !== null) {
      if (test.globalActive) return `Test active, case # ${test.choice}`;
      if (test.sessionActive) return `Test active for session, case # ${test.choice}`;
    } else {
      if (test.paused) return 'Test paused';
      if (test.stoped) return 'Test stoped';
    }

    return '';
  };

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
                <PlayArrow style={{ color: green[400] }} className={classes.statusIcon} />
              }
              {test.paused &&
                <Pause style={{ color: green[400] }} className={classes.statusIcon} />
              }
              {test.stoped &&
                <Stop color="accent" className={classes.statusIcon} />
              }
              <Paper className={classes.headingPaper} elevation={0}>
                <Typography type="title" className={classes.heading}>{test.title}</Typography>
                <Typography type="subheading" className={classes.subHeading}>
                  {renderSummaryText(test)}
                </Typography>
              </Paper>
            </ExpansionPanelSummary>
            <ExpansionPanelDetails className={classes.panelDetails}>
              <TestDetails {...test} />
            </ExpansionPanelDetails>
            <ExpansionPanelActions>
              <Paper elevation={0}>
                {test.globalActive && [
                  renderStopButton(test.id, 1),
                  renderPauseButton(test.id, 2),
                ]}
                {test.sessionActive && [
                  renderStopButton(test.id, 1),
                  renderPauseButton(test.id, 2),
                  renderGlobalLaunchButton(test.id, 3),
                ]}
                {test.paused && [
                  renderStopButton(test.id, 1),
                  renderSessionLaunchButton(test.id, 2),
                  renderGlobalLaunchButton(test.id, 3),
                ]}
                {test.stoped && [
                  renderSessionLaunchButton(test.id, 1),
                  renderGlobalLaunchButton(test.id, 2),
                ]}
              </Paper>
            </ExpansionPanelActions>
          </ExpansionPanel>
        ))}
      </Paper>
    </Toolbar>
  );
};

AbTestingScreen.propTypes = {
  classes: PropTypes.object.isRequired,
  tests: PropTypes.array.isRequired,
  launchSessionTest: PropTypes.func.isRequired,
  pauseTest: PropTypes.func.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
