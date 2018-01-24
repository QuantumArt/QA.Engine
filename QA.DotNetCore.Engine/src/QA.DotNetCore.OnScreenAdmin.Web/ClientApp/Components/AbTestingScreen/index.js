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
import Person from 'material-ui-icons/Person';
import Stop from 'material-ui-icons/Stop';
import { green } from 'material-ui/colors';
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
  activeButtonColor: {
    'backgroundColor': green[500],
    '&:hover': {
      backgroundColor: green[700],
    },
  },
});

const AbTestingScreen = (props) => {
  const {
    classes,
    tests,
    launchTest,
    launchSessionTest,
    stopTest,
    stopSessionTest,
    setTestCase,
  } = props;

  const renderGlobalLaunchButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="primary"
      classes={{
        label: classes.actionButton,
        raisedPrimary: classes.activeButtonColor,
      }}
      onClick={() => { launchTest(testId); }}
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
        raisedPrimary: classes.activeButtonColor,
      }}
      onClick={() => { launchSessionTest(testId); }}
    >
        Start test for session
      <Person className={classes.actionIcon} />
    </Button>
  );
  const renderGlobalStopButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="secondary"
      classes={{ label: classes.actionButton }}
      onClick={() => { stopTest(testId); }}
    >
        Stop test
      <Stop className={classes.actionIcon} />
    </Button>
  );
  const renderSessionStopButton = testId => (
    <Button
      raised
      dense
      key={v4()}
      color="secondary"
      classes={{
        label: classes.actionButton,
      }}
      onClick={() => { stopSessionTest(testId); }}
    >
      Stop test for session
      <Person className={classes.actionIcon} />
    </Button>
  );
  const renderSummaryText = (test) => {
    if (test.choice !== null) {
      if (test.globalActive) return `Test active, case # ${test.choice}`;
      if (test.sessionActive) return `Test active for session, case # ${test.choice}`;
    } else {
      if (test.paused) return 'Test stopped for session';
      if (test.stopped) return 'Test stopped';
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
                renderSessionStopButton(test.id),
                renderGlobalStopButton(test.id),
              ]}
              {test.sessionActive && [
                renderSessionStopButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
              {test.paused && [
                renderGlobalStopButton(test.id),
                renderSessionLaunchButton(test.id),
              ]}
              {test.stopped && [
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
  launchTest: PropTypes.func.isRequired,
  launchSessionTest: PropTypes.func.isRequired,
  stopTest: PropTypes.func.isRequired,
  stopSessionTest: PropTypes.func.isRequired,
  setTestCase: PropTypes.func.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
