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
import ExpandMoreIcon from 'material-ui-icons/ExpandMore';
import Playarrow from 'material-ui-icons/Playarrow';
import TestDetails from './TestDetails';

const styles = theme => ({
  toolBar: {
    paddingLeft: 5,
    paddingRight: 5,
    paddingTop: 25,
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
  actionButton: {
    fontSize: '11px',
  }
});

const AbTestingScreen = ({ testsList, classes }) => (
  <Toolbar className={classes.toolBar}>
    <Paper className={classes.paper} elevation={0}>
      {testsList.map(test => (
        <ExpansionPanel key={test.id} className={classes.panel}>
          <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
            <Typography className={classes.heading}>{test.title}</Typography>
          </ExpansionPanelSummary>
          <ExpansionPanelDetails className={classes.panelDetails}>
            <TestDetails {...test} />
          </ExpansionPanelDetails>
          <ExpansionPanelActions>
            <Button color="primary" raised dense classes={{label: classes.actionButton}}>Launch</Button>
          </ExpansionPanelActions>
        </ExpansionPanel>
      ))}
    </Paper>
  </Toolbar>
);

AbTestingScreen.propTypes = {
  testsList: PropTypes.array.isRequired,
};

export default withStyles(styles)(AbTestingScreen);
