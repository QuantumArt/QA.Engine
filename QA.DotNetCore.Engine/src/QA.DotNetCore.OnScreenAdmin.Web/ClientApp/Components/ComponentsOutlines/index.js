import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from 'material-ui/styles';
import Portal from 'Components/Portal';

import buildFlatListNew from 'utils/buildFlatListNew';

const styles = () => ({
  highlightsWrap: {
    position: 'absolute',
    top: 0,
    left: 0,
  },
  highlightsItem: {
    position: 'absolute',
    pointerEvents: 'none',
  },
});

class ComponentsOutlines extends Component {
  state = {
    documentHeight: 0,
    documentWidth: 0,
    isResizeHandled: false,
  }
  componentDidMount = () => {
    const { updateComponents } = this.props;
    const resize = () => {
      const components = buildFlatListNew();
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
            const borderColor = component.type === 'widget' ? '#3f51b5' : '#72e272';
            const nestPadding = (maxNestLevel + 2) - component.nestLevel;
            return (
              <div
                key={component.onScreenId}
                className={`${classes.highlightsItem} component--${component.onScreenId}`}
                style={{
                  top: `${coords.top - nestPadding}px`,
                  left: `${coords.left - nestPadding}px`,
                  width: `${coords.width + (nestPadding * 2)}px`,
                  height: `${coords.height + (nestPadding * 2)}px`,
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
