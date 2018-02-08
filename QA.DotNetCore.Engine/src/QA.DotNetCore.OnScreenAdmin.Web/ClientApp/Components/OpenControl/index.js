import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Draggable from 'react-draggable';
import { withStyles } from 'material-ui/styles';
import Button from 'material-ui/Button';
import Settings from 'material-ui-icons/Settings';

const styles = {
  wrap: {
    position: 'fixed',
    top: 20,
    left: 20,
    zIndex: 1299,
  },
  popover: {
    backgroundColor: 'transparent',
    boxShadow: 'none',
    padding: 5,
  },
  buttonSmall: {
    width: 36,
    height: 36,
  },
  buttonHidden: {
    display: 'none',
  },
};

class OpenControl extends Component {
  state = {
    canClick: true,
    preventDrag: false,
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

  render() {
    const {
      toggleSidebar,
      classes,
      drawerOpened,
    } = this.props;
    const { canClick, preventDrag } = this.state;

    return (
      <Draggable
        onStart={this.enableClick}
        onDrag={this.dragHander}
        bounds="html"
        defaultPosition={{ x: 10, y: 10 }}
        grid={[25, 25]}
        disabled={preventDrag}
      >
        <div className={classes.wrap} ref={(el) => { this.wrap = el; }}>
          <Button
            variant="fab"
            color="primary"
            onClick={canClick ? toggleSidebar : null}
            className={drawerOpened ? classes.buttonHidden : null}
          >
            <Settings />
          </Button>
        </div>
      </Draggable>
    );
  }
}

OpenControl.propTypes = {
  toggleSidebar: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  drawerOpened: PropTypes.bool.isRequired,
};

export default withStyles(styles)(OpenControl);
