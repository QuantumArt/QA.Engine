/* eslint-disable no-unused-vars */
import React, { Component } from 'react';
import { findDOMNode } from 'react-dom';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Typography from 'material-ui/Typography';
import List, {
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
} from 'material-ui/List';
import Popover from 'material-ui/Popover';
import Tooltip from 'material-ui/Tooltip';
import IconButton from 'material-ui/IconButton';
import PlayArrow from 'material-ui-icons/PlayArrow';
import { deepPurple } from 'material-ui/colors';

const styles = theme => ({
  lisItemActive: {
    'backgroundColor': deepPurple['500'],
    '&:hover': {
      backgroundColor: deepPurple['800'],
    },
  },
  listText: {
    fontSize: theme.typography.fontSize,
  },
  listTextActive: {
    fontSize: theme.typography.fontSize,
    fontWeight: 'bold',
    color: 'white',
  },
  actionTooltip: {
    fontSize: 11,
    width: 90,
  },
  containersList: {
  },
  containersTitle: {
    fontSize: 18,
    textAlign: 'center',
    paddingTop: 15,
  },
  containersListText: {
    fontSize: 15,
  },
  containersListTextRoot: {
    '&:first-child': {
      paddingLeft: 'inherit',
    },
  },
  popover: {
    width: 302,
  },
});

class TestCaseDetails extends Component {
  static propTypes = {
    classes: PropTypes.object.isRequired,
    data: PropTypes.object.isRequired,
    active: PropTypes.bool,
    paused: PropTypes.bool.isRequired,
    stoped: PropTypes.bool.isRequired,
    index: PropTypes.number.isRequired,
    setTestCase: PropTypes.func.isRequired,
    id: PropTypes.number.isRequired,
  }

  static defaultProps = {
    active: false,
  }

  state = {
    open: false,
    anchorEl: null,
  }

  handleCaseInfoClick = (e) => {
    this.setState({
      anchorEl: findDOMNode(e.currentTarget),
      open: !this.state.open,
    });
  }

  render() {
    const {
      classes,
      data,
      active,
      paused,
      stoped,
      index,
      id,
      setTestCase,
    } = this.props;
    const { open, anchorEl } = this.state;

    return (
      <ListItem
        button
        className={active ? classes.lisItemActive : ''}
        ref={(node) => { this.node = node; }}
        onClick={this.handleCaseInfoClick}
      >
        <ListItemText
          primary={`# ${index} - ${data.percent}%`}
          classes={{ text: active ? classes.listTextActive : classes.listText }}
        />
        <Popover
          anchorEl={anchorEl}
          anchorReference="anchorEl"
          anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
          transformOrigin={{ vertical: 'top', horizontal: 'center' }}
          open={open}
          marginThreshold={8}
          classes={{ paper: classes.popover }}
          onClose={(e) => { console.log(e); }}
        >
          <Typography type="title" className={classes.containersTitle} >
            {data.containers.length > 0
              ? 'Active actions'
              : 'No active actions'}
          </Typography>
          <List className={classes.containersList}>
            {data.containers.map(container => (
              <ListItem key={container.cid}>
                <ListItemText
                  primary={container.variantDescription}
                  secondary={container.containerDescription}
                  classes={{
                    text: classes.containersListText,
                    root: classes.containersListTextRoot,
                  }}
                />
              </ListItem>
            ))}
          </List>
        </Popover>
        {!active &&
          <ListItemSecondaryAction>
            <Tooltip
              id="runCase"
              placement="left"
              title="Turn this case"
              classes={{ tooltip: classes.actionTooltip }}
            >
              <IconButton onClick={() => { setTestCase(id, index); }}>
                <PlayArrow />
              </IconButton>
            </Tooltip>
          </ListItemSecondaryAction>
        }
      </ListItem>
    );
  }
}

export default withStyles(styles)(TestCaseDetails);
