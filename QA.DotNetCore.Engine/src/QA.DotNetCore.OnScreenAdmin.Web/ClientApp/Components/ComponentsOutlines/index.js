import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Portal from 'Components/Portal';

import buildFlatList from 'utils/buildFlatList';

const styles = () => ({
  highlightsWrap: {
    position: 'absolute',
    top: 0,
    left: 0,
    pointerEvents: 'none',
    zIndex: 999,
  },
  highlightsItem: {
    position: 'absolute',
    pointerEvents: 'none',
    outline: 'none',
  },
});

class ComponentsOutlines extends Component {
  state = {
    documentHeight: 0,
    documentWidth: 0,
    isResizeHandled: false,
  }

  componentWillMount() {
    const { updateComponents } = this.props;
    const resize = () => {
      const components = buildFlatList();
      updateComponents(components);
    };
    if (!this.state.isResizeHandled) {
      window.onresize = resize;
    }
    this.setState({
      documentHeight: document.body.offsetHeight,
      documentWidth: document.body.offsetWidth,
      isResizeHandled: true,
    });
  }

  render() {
    const { components, classes } = this.props;
    return (
      <Portal>
        <div
          className={classes.highlightsWrap}
          style={{
            height: `${document.body.offsetHeight}px`,
            width: `${document.body.offsetWidth}px`,
          }}
        >
          {components.map((component) => {
            const coords = component.properties.componentCoords;
            if (!Object.keys(coords).length) return null;
            const borderWidth = component.isSelected ? '2px' : '0px';
            const borderColor = component.type === 'widget' ? '#29b6f6' : '#66bb6a';
            // const nestPadding = (maxNestLevel / component.nestLevel) - component.nestLevel;
            return (
              <div
                key={component.onScreenId}
                className={`${classes.highlightsItem} component--${component.onScreenId}`}
                role="button"
                tabIndex={0}
                style={{
                  top: `${coords.top}px`,
                  left: `${coords.left}px`,
                  width: `${coords.width}px`,
                  height: `${coords.height}px`,
                  border: `${borderWidth} dashed ${borderColor}`,
                }}
              />
            );
          })
          }
        </div>
      </Portal>
    );
  }
}

ComponentsOutlines.propTypes = {
  components: PropTypes.arrayOf(PropTypes.object).isRequired,
  maxNestLevel: PropTypes.number.isRequired,
  updateComponents: PropTypes.func.isRequired,
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ComponentsOutlines);
