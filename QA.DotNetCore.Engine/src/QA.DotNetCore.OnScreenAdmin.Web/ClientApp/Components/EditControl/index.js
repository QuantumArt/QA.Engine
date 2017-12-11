/* eslint-disable */
import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import { lightBlue, green } from 'material-ui/colors';
import IconButton from 'material-ui/IconButton';
import ModeEdit from 'material-ui-icons/ModeEdit';
import Add from 'material-ui-icons/Add';

const styles = () => ({
  wrap: {
    'position': 'absolute',
    'border': '1px dashed',
    'width': 'calc(100% + 3px)',
    'height': 'calc(100% + 3px)',
    'margin': -3,
    'borderRadius': 3,
    'outline': 'none',
    'minHeight': 5,
    'zIndex': 10,
    '&:hover': {
      borderWidth: 2,
    },
  },
  wrapSelected: {
    extend: 'wrap',
    borderWidth: 2,
  },
  bg: {
    position: 'fixed',
    width: '100%',
    height: '100%',
    top: 0,
    left: 0,
    backgroundColor: 'rgba(0, 0, 0, 0.3)',
  },
  button: {
    position: 'absolute',
    top: -10,
  },
});

class EditControl extends Component {
  state = {
    isHovered: false,
  }

  mouseEnterHandler = () => {
    this.setState({ isHovered: true });
  }

  mouseLeaveHandler = () => {
    this.setState({ isHovered: false });
  }

  render() {
    const {
      classes,
      properties,
      nestLevel,
      maxNestLevel,
      type,
      isSelected,
      showAllZones,
      showAllWidgets,
      side,
      handleToggleClick,
    } = this.props;
    const { isHovered } = this.state;
    const reversedNestLevel = maxNestLevel - nestLevel;

    if (type === 'zone' && showAllZones) {
      return (
        <div
          className={
            `${classes.wrap} ${isSelected ? classes.wrapSelected : ''}`
          }
          role="button"
          tabIndex={0}
          onClick={isSelected ? null : handleToggleClick}
          onMouseEnter={this.mouseEnterHandler}
          onMouseLeave={this.mouseLeaveHandler}
          style={{
            cursor: isSelected ? 'default' : 'pointer',
            pointerEvents: isSelected ? 'none' : 'auto',
            width: `calc(100% + ${reversedNestLevel * 3}px)`,
            height: `calc(100% + ${reversedNestLevel * 3}px)`,
            margin: `${reversedNestLevel * -3}px`,
            borderColor: green[400],
          }}
        >
          {(isSelected || isHovered) &&
            <Fragment>
            <IconButton
              color="primary"
              className={classes.button}
              style={{
                pointerEvents: isSelected ? 'auto' : 'none',
                left: side === 'left' ? '100%' : 'auto',
                right: side === 'right' ? '100%' : 'auto',
              }}
            >
              <ModeEdit />
            </IconButton>
            <IconButton
            color="primary"
            className={classes.button}
            style={{
              pointerEvents: isSelected ? 'auto' : 'none',
              left: side === 'left' ? '100%' : 'auto',
              right: side === 'right' ? '100%' : 'auto',
              top: 30,
            }}
          >
            <Add />
          </IconButton>
          </Fragment>
          }
        </div>
      );
    }

    if (type === 'widget' && showAllWidgets) {
      return (
        <div
          className={
            `${classes.wrap} ${isSelected ? classes.wrapSelected : ''}`
          }
          role="button"
          tabIndex={0}
          onClick={isSelected ? null : handleToggleClick}
          onMouseEnter={this.mouseEnterHandler}
          onMouseLeave={this.mouseLeaveHandler}
          style={{
            cursor: isSelected ? 'default' : 'pointer',
            pointerEvents: isSelected ? 'none' : 'auto',
            width: `calc(100% + ${reversedNestLevel * 3}px)`,
            height: `calc(100% + ${reversedNestLevel * 3}px)`,
            margin: `${reversedNestLevel * -3}px`,
            borderColor: lightBlue[400],
          }}
        >
          {(isSelected || isHovered) &&
            <IconButton
              color="primary"
              className={classes.button}
              style={{
                pointerEvents: isSelected ? 'auto' : 'none',
                left: side === 'left' ? '100%' : 'auto',
                right: side === 'right' ? '100%' : 'auto',
              }}
            >
              <ModeEdit />
            </IconButton>
          }
        </div>
      );
    }

    return null;
  }
}

EditControl.propTypes = {
  type: PropTypes.string.isRequired,
  isSelected: PropTypes.bool.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  nestLevel: PropTypes.number.isRequired,
  maxNestLevel: PropTypes.number.isRequired,
  properties: PropTypes.oneOfType([
    PropTypes.shape({
      widgetId: PropTypes.string.isRequired,
      title: PropTypes.string.isRequired,
      alias: PropTypes.string.isRequired,
    }),
    PropTypes.shape({
      zoneName: PropTypes.string.isRequired,
      isRecursive: PropTypes.bool.isRequired,
      isGlobal: PropTypes.bool.isRequired,
    }),
  ]).isRequired,
  classes: PropTypes.object.isRequired,
  handleToggleClick: PropTypes.func.isRequired,
};

export default withStyles(styles)(EditControl);
