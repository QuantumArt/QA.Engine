import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import { deepPurple, grey } from 'material-ui/colors';
import Button from 'material-ui/Button';

const styles = () => ({
  wrap: {
    'position': 'absolute',
    'border': '1px dashed',
    'width': 'calc(100% + 3px)',
    'height': 'calc(100% + 3px)',
    'margin': -3,
    'borderColor': grey[400],
    'borderRadius': 3,
    'outline': 'none',
    'minHeight': 5,
    '&:hover': {
      borderColor: deepPurple[500],
      borderWidth: 2,
    },
  },
  wrapSelected: {
    extend: 'wrap',
    borderColor: deepPurple[500],
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
    left: 0,
    top: '-40px',
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
      handleToggleClick,
    } = this.props;
    const { isHovered } = this.state;
    const reversedNestLevel = maxNestLevel - nestLevel;

    return (
      <div
        className={
          `${showAllZones ? classes.wrap : ''} ${isSelected ? classes.wrapSelected : ''}`
        }
        role="button"
        tabIndex={0}
        onClick={isSelected ? null : handleToggleClick}
        onMouseEnter={this.mouseEnterHandler}
        onMouseLeave={this.mouseLeaveHandler}
        style={{
          cursor: isSelected ? 'default' : 'pointer',
          pointerEvents: isSelected ? 'none' : 'auto',
          zIndex: isSelected ? 1 : 0,
          width: showAllZones ? `calc(100% + ${reversedNestLevel * 20}px)` : '',
          height: showAllZones ? `calc(100% + ${reversedNestLevel * 20}px)` : '',
          margin: showAllZones ? `${reversedNestLevel * -20}px` : '',
        }}
      >
        {(isSelected || isHovered) &&
          <Button
            raised
            color="primary"
            component="span"
            className={classes.button}
            style={{
              pointerEvents: isSelected ? 'auto' : 'none',
              top: isSelected ? -41 : '',
              left: isSelected ? -1 : '',
            }}
          >
            Edit {type === 'zone'
              ? `Zone ${properties.zoneName}`
              : `Widget ${properties.title}`}
          </Button>
        }
      </div>
    );
  }
}

EditControl.propTypes = {
  type: PropTypes.string.isRequired,
  isSelected: PropTypes.bool.isRequired,
  showAllZones: PropTypes.bool.isRequired,
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
