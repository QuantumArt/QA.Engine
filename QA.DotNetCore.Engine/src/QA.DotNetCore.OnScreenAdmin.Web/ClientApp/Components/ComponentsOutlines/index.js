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
  },
  highlightsItem: {
    position: 'absolute',
    pointerEvents: 'auto',
    cursor: 'pointer',
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

  clickHandler() {
    console.log(1);
  }

  render() {
    const { components, classes, maxNestLevel } = this.props;
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
            const borderWidth = component.isSelected ? '2px' : '1px';
            const borderColor = component.type === 'widget' ? '#29b6f6' : '#66bb6a';
            const nestPadding = (maxNestLevel / component.nestLevel) - component.nestLevel;
            const paddingFactor = 2;
            return (
              <div
                key={component.onScreenId}
                className={`${classes.highlightsItem} component--${component.onScreenId}`}
                onClick={this.clickHandler}
                role="button"
                tabIndex={0}
                style={{
                  top: `${coords.top - nestPadding - paddingFactor}px`,
                  left: `${coords.left - nestPadding - paddingFactor}px`,
                  width: `${coords.width + (nestPadding * 2) + (paddingFactor * 2)}px`,
                  height: `${coords.height + (nestPadding * 2) + (paddingFactor * 2)}px`,
                  border: `${borderWidth} dashed ${borderColor}`,
                }}
              >
                <span style={{ position: 'absolute', left: '30%' }}>{component.nestLevel}</span>
              </div>
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
