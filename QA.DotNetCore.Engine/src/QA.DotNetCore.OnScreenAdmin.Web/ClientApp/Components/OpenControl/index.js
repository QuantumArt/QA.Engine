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

  stopHanlder = (e, cords) => {
    const buttonCords = document.getElementById('gear').getBoundingClientRect();
    this.props.saveCords(
      cords.lastX,
      cords.lastY,
      buttonCords.left,
      buttonCords.top,
    );
  }

  render() {
    const {
      toggleSidebar,
      classes,
      drawerOpened,
      cords: { componentX, componentY },
    } = this.props;
    const { canClick, preventDrag } = this.state;

    return (
      <Draggable
        onStart={this.enableClick}
        onDrag={this.dragHander}
        bounds="html"
        defaultPosition={{ x: componentX, y: componentY }}
        onStop={this.stopHanlder}
        grid={[25, 25]}
        disabled={preventDrag}
      >
        <div className={classes.wrap}>
          <Button
            variant="fab"
            color="primary"
            onClick={canClick ? toggleSidebar : null}
            className={drawerOpened ? classes.buttonHidden : null}
            id="gear"
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
  saveCords: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
  drawerOpened: PropTypes.bool.isRequired,
  cords: PropTypes.object.isRequired,
};

export default withStyles(styles)(OpenControl);
