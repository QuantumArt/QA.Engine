import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import List, {
  ListItem,
  ListItemText,
} from 'material-ui/List';
import ExpansionPanel, {
  ExpansionPanelSummary,
  ExpansionPanelDetails,
  ExpansionPanelActions,
} from 'material-ui/ExpansionPanel';
import Button from 'material-ui/Button';
import IconButton from 'material-ui/IconButton';
import PlayArrow from 'material-ui-icons/PlayArrow';
import { blue, teal, deepPurple } from 'material-ui/colors';

const styles = {
  panel: {
    'boxShadow': 'none',
    'borderTop': '1px solid',
    'borderColor': teal[50],
    '&:before': {
      backgroundColor: 'transparent',
    },
    '&:last-child': {
      borderBottom: '1px solid',
      borderBottomColor: teal[50],
    },
  },
  panelExpanded: {
    margin: 0,
  },
  panelDetails: {
    flexDirection: 'column',
    paddingTop: 0,
    paddingBottom: 8,
    paddingLeft: 8,
  },
  panelSummary: {
    paddingRight: 0,
    paddingLeft: 15,
  },
  panelSummaryActive: {
    'paddingRight': 0,
    'paddingLeft': 15,
    'backgroundColor': blue['100'],
    '&:hover': {
      backgroundColor: blue['200'],
    },
  },
  panelSummaryContent: {
    '& > :last-child': {
      paddingRight: 0,
    },
  },
  panelSummaryContentActive: {
    '& > :last-child': {
      paddingRight: 0,
    },
    '& > p': {
      fontWeight: 'bold',
    },
  },
  panelActions: {
    paddintTop: 0,
  },
  caseInfo: {
    fontSize: 16,
  },
  caseFrequency: {
    fontSize: 14,
    marginLeft: 15,
    marginTop: 3,
  },
  caseDescription: {
    fontSize: 12,
    marginLeft: 13,
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    maxWidth: 165,
    minWidth: 165,
    marginTop: 4,
  },
  containersListItem: {
    paddingTop: 6,
    paddingBottom: 6,
  },
  actionTooltip: {
    fontSize: 11,
    width: 90,
  },
  containersListPrimary: {
    fontSize: 15,
  },
  containersListSecondary: {
    fontSize: 13,
  },
  actionButton: {
    'position': 'absolute',
    'right': 0,
    'top': '50%',
    'marginTop': -24,
    'color': deepPurple[400],
    '&:hover': {
      color: deepPurple[700],
    },
  },
};

const TestCaseDetails = (props) => {
  const {
    classes,
    data,
    active,
    // paused,
    // stoped,
    index,
    id,
    setTestCase,
  } = props;
  const handleStartClick = (e) => {
    e.stopPropagation();
    setTestCase(id, index);
  };
  const renderDescription = () => {
    if (data.containers.length === 0) {
      return 'No active actions';
    }

    return data.containers.map(el => el.variantDescription).toString().replace(',', '; ');
  };

  return (
    <ExpansionPanel
      classes={{
        root: classes.panel,
        expanded: classes.panelExpanded,
      }}
    >
      <ExpansionPanelSummary
        className={active ? classes.panelSummaryActive : classes.panelSummary}
        classes={{
          content: active ? classes.panelSummaryContentActive : classes.panelSummaryContent,
        }}
      >
        <Typography className={classes.caseInfo}>{`#${index}`}</Typography>
        <Typography className={classes.caseDescription}>{renderDescription()}</Typography>
        <Typography className={classes.caseFrequency}>{`${data.percent}%`}</Typography>
        {!active &&
          <IconButton onClick={handleStartClick} className={classes.actionButton}>
            <PlayArrow />
          </IconButton>
        }
      </ExpansionPanelSummary>
      <ExpansionPanelDetails className={classes.panelDetails}>
        <List className={classes.containersList}>
          {data.containers.map(container => (
            <ListItem key={container.cid} className={classes.containersListItem}>
              <ListItemText
                primary={container.variantDescription}
                secondary={container.containerDescription}
                classes={{
                  primary: classes.containersListPrimary,
                  secondary: classes.containersListSecondary,
                }}
              />
            </ListItem>
          ))}
        </List>
      </ExpansionPanelDetails>
      <ExpansionPanelActions className={classes.panelActions}>
        <Button
          raised
          dense
          style={{ backgroundColor: teal[500], color: 'white' }}
        >
          Some future action
        </Button>
      </ExpansionPanelActions>
    </ExpansionPanel>
  );
};

TestCaseDetails.propTypes = {
  classes: PropTypes.object.isRequired,
  data: PropTypes.object.isRequired,
  active: PropTypes.bool,
  // paused: PropTypes.bool.isRequired,
  // stoped: PropTypes.bool.isRequired,
  index: PropTypes.number.isRequired,
  setTestCase: PropTypes.func.isRequired,
  id: PropTypes.number.isRequired,
};

TestCaseDetails.defaultProps = {
  active: false,
};

export default withStyles(styles)(TestCaseDetails);
