import React from 'react';
import { v4 } from 'uuid';
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
import ExpandMoreIcon from 'material-ui-icons/ExpandMore';
import PlayArrow from 'material-ui-icons/PlayArrow';
import Pause from 'material-ui-icons/Pause';
import Stop from 'material-ui-icons/Stop';
import StatusIcon from './StatusIcon';
import TestDetails from './TestDetails';

const styles = theme => ({
  toolBar: {
    padding: '25px 5px 10px',
    overflow: 'hidden',
  },
  paper: {
    width: '100%',
  },
  headingPaper: {
    marginLeft: 30,
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
  panelDetails: {
    flexDirection: 'column',
  },
  panelActions: {

  },
  actionButton: {
    fontSize: 10,
  },
  actionIcon: {
    width: 15,
    height: 15,
    marginLeft: 5,
  },
});

const AbTestingScreen = (props) => {
  const {
    classes,
    tests,
    launchSessionTest,
    pauseTest,
    setTestCase,
  } = props;

  const renderPauseButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="primary"
      classes={{
        label: classes.actionButton,
      }}
      onClick={() => { pauseTest(testId); }}
    >
      Stop test for session
      <Pause className={classes.actionIcon} />
    </Button>
  );
  const renderStopButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="secondary"
      classes={{ label: classes.actionButton }}
      onClick={() => { console.log(testId); }}
    >
        Stop test
      <Stop className={classes.actionIcon} />
    </Button>
  );
  const renderGlobalLaunchButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="primary"
      classes={{ label: classes.actionButton }}
      onClick={() => { console.log(testId); }}
    >
        Start test
      <PlayArrow className={classes.actionIcon} />
    </Button>
  );
  const renderSessionLaunchButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="primary"
      classes={{
        label: classes.actionButton,
      }}
      onClick={() => { launchSessionTest(testId); }}
    >
        Start test for session
      <PlayArrow className={classes.actionIcon} />
    </Button>
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
              <StatusIcon {...test} />
              <Paper className={classes.headingPaper} elevation={0}>
                <Typography type="title" className={classes.heading}>{test.title}</Typography>
                <Typography type="subheading" className={classes.subHeading}>
                  {renderSummaryText(test)}
                </Typography>
              </Paper>
            </ExpansionPanelSummary>
            <ExpansionPanelDetails className={classes.panelDetails}>
              <TestDetails
                setTestCase={setTestCase}
                {...test}
              />
            </ExpansionPanelDetails>
            <ExpansionPanelActions className={classes.panelActions}>
              {test.globalActive && [
                renderStopButton(test.id),
                renderPauseButton(test.id),
              ]}
              {test.sessionActive && [
                renderPauseButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
              {test.paused && [
                renderSessionLaunchButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
              {test.stoped && [
                renderSessionLaunchButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
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
  setTestCase: PropTypes.func.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
