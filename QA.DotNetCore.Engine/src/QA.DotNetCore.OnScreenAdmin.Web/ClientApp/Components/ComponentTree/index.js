import React from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';

const ComponentTree = ({
  components,
  selectedComponentId,
  onToggleComponent,
  onToggleSubtree,
  onToggleFullSubtree,
  onEditWidget,
  onAddWidgetToZone,
  showAllZones,
  showAvailableWidgets,
}) => (
  <List>
    {components.map(component => (
      <ComponentItem
        {...component}
        selectedComponentId={selectedComponentId}
        isOpened={component.isOpened}
        key={component.onScreenId}
        onToggleComponent={onToggleComponent}
        onToggleSubtree={onToggleSubtree}
        onToggleFullSubtree={onToggleFullSubtree}
        onEditWidget={onEditWidget}
        onAddWidget={onAddWidgetToZone}
        showAllZones={showAllZones}
        showListItem={!showAvailableWidgets}
      />))}
  </List>
);

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
  onEditWidget: PropTypes.func.isRequired,
  onAddWidgetToZone: PropTypes.func.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  showAvailableWidgets: PropTypes.bool.isRequired,
};

export default ComponentTree;
