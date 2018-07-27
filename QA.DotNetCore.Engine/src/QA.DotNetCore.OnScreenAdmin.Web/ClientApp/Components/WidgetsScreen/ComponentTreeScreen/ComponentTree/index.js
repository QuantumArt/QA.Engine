import React from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';


export default function ComponentTree(props) {
  const {
    components,
    selectedComponentId,
    onToggleComponent,
    onToggleSubtree,
    onToggleFullSubtree,
    isMovingWidget,
    onMovingWidgetSelectTargetZone,
    showOnlyWidgets,
  } = props;

  return (
    <List dense>
      {components.map(component => (
        <ComponentItem
          {...component}
          selectedComponentId={selectedComponentId}
          isOpened={component.isOpened}
          key={component.onScreenId}
          onToggleComponent={onToggleComponent}
          onToggleSubtree={onToggleSubtree}
          onToggleFullSubtree={onToggleFullSubtree}
          isMovingWidget={isMovingWidget}
          onMovingWidgetSelectTargetZone={onMovingWidgetSelectTargetZone}
          showOnlyWidgets={showOnlyWidgets}
          itemLevel={1}
        />))}
    </List>
  );
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
  selectedComponentId: PropTypes.string.isRequired,
  onToggleComponent: PropTypes.func.isRequired,
  onToggleSubtree: PropTypes.func.isRequired,
  onToggleFullSubtree: PropTypes.func.isRequired,
  isMovingWidget: PropTypes.bool.isRequired,
  onMovingWidgetSelectTargetZone: PropTypes.func.isRequired,
  showOnlyWidgets: PropTypes.bool.isRequired,
};
