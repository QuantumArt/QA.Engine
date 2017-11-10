import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Button from 'material-ui/Button';

const styles = () => ({
  wrap: {
    position: 'absolute',
    width: 'calc(100% + 6px)',
    height: 'calc(100% + 6px)',
    margin: -3,
    border: '1px dashed red',
    borderRadius: 3,
    outline: 'none',
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
    console.log('leave');
  }

  render() {
    const {
      classes,
      properties,
      type,
      isSelected,
      showAllZones,
      handleToggleClick,
    } = this.props;
    const { isHovered } = this.state;

    return (
      <div
        className={showAllZones || isSelected ? classes.wrap : ''}
        role="button"
        tabIndex={0}
        onClick={isSelected ? null : handleToggleClick}
        onMouseEnter={this.mouseEnterHandler}
        onMouseLeave={this.mouseLeaveHandler}
        style={{ cursor: isSelected ? 'default' : 'pointer' }}
      >
        {(isSelected || isHovered) &&
          <Button
            raised
            color="primary"
            component="span"
            className={classes.button}
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
