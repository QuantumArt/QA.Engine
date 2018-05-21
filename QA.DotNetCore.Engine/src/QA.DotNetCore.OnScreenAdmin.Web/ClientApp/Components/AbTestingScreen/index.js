import React from 'react';
import { v4 } from 'uuid';
import PropTypes from 'prop-types';
import classNames from 'classnames';
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
import moment from 'moment';
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
    fontWeight: theme.typography.fontWeightRegular,
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
  testPassed: {
    opacity: 0.6,
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
      variant="raised"
      dense="true"
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
      variant="raised"
      dense="true"
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
      variant="raised"
      dense="true"
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
      variant="raised"
      dense="true"
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

  const isTestPast = date => moment(date).isBefore(moment());
  const isTestFuture = date => moment(date).isAfter(moment());
  const isTestDataActive = (startMoment, endMoment) => {
    if (startMoment && endMoment) {
      return (moment(startMoment).isBefore(moment()) && moment(endMoment).isAfter(moment()));
    }
    if (startMoment) {
      return (moment(startMoment).isBefore(moment()));
    }
    return moment(endMoment).isAfter(moment());
  };

  const renderSummaryText = (test) => {
    if (test.choice !== null) {
      if (test.globalActive && isTestPast(test.endDate)) {
        return `The test did finished ${moment(test.endDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.globalActive && isTestFuture(test.startDate)) {
        return `The test will begin ${moment(test.startDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.globalActive) {
        return `Test active, case # ${test.choice}`;
      }
      if (test.sessionActive) return `Test active for session, case # ${test.choice}`;
    } else {
      if (test.sessionStopped && isTestPast(test.endDate)) {
        return `The test did finished ${moment(test.endDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.sessionStopped && isTestFuture(test.startDate)) {
        return `The test will begin ${moment(test.startDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.sessionStopped) return 'Test stopped for session';
      if (test.globalStopped && isTestPast(test.endDate)) {
        return `The test did finished ${moment(test.endDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.globalStopped && isTestFuture(test.startDate)) {
        return `The test will begin ${moment(test.startDate).format('MMMM Do YYYY, h:mm a').toString()}`;
      }
      if (test.globalStopped) return 'Test stopped';
      if (test.globalStopped && test.sessionStopped) return 'Test stopped';
    }
    return '';
  };

  return (
    <Toolbar className={classes.toolBar}>
      <Paper className={classes.paper} elevation={0}>
        {tests.map(test => (
          <ExpansionPanel
            key={test.id}
            className={classNames(classes.panel, {
              [classes.testPassed]: !isTestDataActive(test.startDate, test.endDate),
            })}
          >
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
              {test.globalActive && isTestDataActive(test.startDate, test.endDate) && [
                renderSessionStopButton(test.id),
                renderGlobalStopButton(test.id),
              ]}
              {test.globalActive && isTestFuture(test.startDate) && [
                renderSessionLaunchButton(test.id),
                renderGlobalStopButton(test.id),
              ]}
              {test.globalActive && isTestPast(test.endDate) && [
                renderSessionLaunchButton(test.id),
              ]}
              {test.sessionActive && isTestDataActive(test.startDate, test.endDate) && [
                renderSessionStopButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
              {test.sessionActive && !isTestDataActive(test.startDate, test.endDate) && [
                renderSessionStopButton(test.id),
              ]}
              {test.sessionStopped && isTestDataActive(test.startDate, test.endDate) && [
                renderGlobalStopButton(test.id),
                renderSessionLaunchButton(test.id),
              ]}
              {test.sessionStopped && !isTestDataActive(test.startDate, test.endDate) && [
                renderSessionLaunchButton(test.id),
              ]}
              {test.sessionStopped && isTestFuture(test.startDate) && [
                renderGlobalLaunchButton(test.id),
              ]}
              {test.globalStopped && isTestDataActive(test.startDate, test.endDate) && [
                renderSessionLaunchButton(test.id),
                renderGlobalLaunchButton(test.id),
              ]}
              {test.globalStopped && isTestPast(test.endDate) && [
                renderSessionLaunchButton(test.id),
              ]}
              {test.globalStopped && isTestFuture(test.startDate) && [
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
