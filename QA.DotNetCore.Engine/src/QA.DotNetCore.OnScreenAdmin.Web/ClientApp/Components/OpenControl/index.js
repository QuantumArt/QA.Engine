import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Draggable from 'react-draggable';
import { withStyles } from 'material-ui/styles';
import Popover from 'material-ui/Popover';
import Button from 'material-ui/Button';
import IconButton from 'material-ui/IconButton';
import Settings from 'material-ui-icons/Settings';
import BorderLeft from 'material-ui-icons/BorderLeft';
import BorderRight from 'material-ui-icons/BorderRight';

const styles = {
  wrap: {
    position: 'fixed',
    top: 20,
    left: 20,
  },
  buttonHidden: {
    display: 'none',
  },
};

class OpenControl extends Component {
  state = {
    canClick: true,
    popoverOpened: false,
  };

  disableClick = () => {
    this.setState({ canClick: false });
  }

  enableClick = () => {
    this.setState({ canClick: true });
  }

  dragHander = () => {
    this.disableClick();
  }

  handleDoubleClick = (e) => {
    e.preventDefault();
    this.setState({ popoverOpened: !this.state.popoverOpened });
  }

  handlePopoverClose = () => {
    this.setState({ popoverOpened: !this.state.popoverOpened });
  }

  render() {
    /* eslint-disable */
    const { onClick, classes, drawerOpened } = this.props;
    const { canClick } = this.state;

    return (
      <Draggable
        onStart={this.enableClick}
        onDrag={this.dragHander}
        grid={[25, 25]}
      >
        <div className={classes.wrap} ref={el => this.wrap = el}>
          <Button
            fab
            color="primary"
            onClick={canClick ? onClick : null}
            className={drawerOpened ? classes.buttonHidden : null}
            onContextMenu={this.handleDoubleClick}
          >
            <Settings />
          </Button>
          <Popover
            open={this.state.popoverOpened}
            onRequestClose={this.handlePopoverClose}
            anchorEl={this.wrap}
            anchorOrigin={{vertical: 'bottom', horizontal: 'center'}}
            transformOrigin={{vertical: 'top', horizontal: 'center'}}
          >
            <IconButton>
              <BorderLeft />
            </IconButton>
            <IconButton>
              <BorderRight />
            </IconButton>
          </Popover>
        </div>
      </Draggable>
    );
  }
}

OpenControl.propTypes = {
  onClick: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  drawerOpened: PropTypes.bool.isRequired,
};

export default withStyles(styles)(OpenControl);
