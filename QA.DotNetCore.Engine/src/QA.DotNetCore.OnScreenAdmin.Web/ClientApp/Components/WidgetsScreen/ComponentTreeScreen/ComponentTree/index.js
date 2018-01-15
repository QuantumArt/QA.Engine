import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';


class ComponentTree extends Component {
  renderComponentsList = () => {
    const {
      components,
      maxNestLevel,
      selectedComponentId,
      onToggleComponent,
      onToggleSubtree,
      onToggleFullSubtree,
      isMovingWidget,
      onMovingWidgetSelectTargetZone,
    } = this.props;
    console.log(components);
    return (
      <List dense >
        {components.map(component => (
          <ComponentItem
            {...component}
            maxNestLevel={maxNestLevel}
            selectedComponentId={selectedComponentId}
            isOpened={component.isOpened}
            key={component.onScreenId}
            onToggleComponent={onToggleComponent}
            onToggleSubtree={onToggleSubtree}
            onToggleFullSubtree={onToggleFullSubtree}
            isMovingWidget={isMovingWidget}
            onMovingWidgetSelectTargetZone={onMovingWidgetSelectTargetZone}

          />))}
      </List>
    );
  }


  render() {
    return (
      <Fragment>
        { this.renderComponentsList() }
      </Fragment>
    );
  }
}

ComponentTree.propTypes = {
  components: PropTypes.arrayOf(
    PropTypes.shape({
      type: PropTypes.string.isRequired,
      onScreenId: PropTypes.string.isRequired,
      isOpened: PropTypes.bool,
      properties: PropTypes.object.isRequired,
      children: PropTypes.array.isRequired,
    }).isRequired,
  ).isRequired,
  maxNestLevel: PropTypes.number.isRequired,
  selectedComponentId: PropTypes.string.isRequired,
  onToggleComponent: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  onToggleFullSubtree: PropTypes.func.isRequired,
  disabledComponents: PropTypes.array.isRequired,
  isMovingWidget: PropTypes.bool.isRequired,
  onMovingWidgetSelectTargetZone: PropTypes.func.isRequired,
};

export default ComponentTree;
