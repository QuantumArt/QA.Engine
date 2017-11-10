import React from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';

const ComponentTree = ({ components, selectedComponentId, onToggleComponent, showAllZones }) => (
  <List>
    {components.map(component => (
      <ComponentItem
        {...component}
        selectedComponentId={selectedComponentId}
        key={component.onScreenId}
        onToggleComponent={onToggleComponent}
        showAllZones={showAllZones}
      />))}
  </List>
);

ComponentTree.propTypes = {
  components: PropTypes.arrayOf(
    PropTypes.shape({
      type: PropTypes.string.isRequired,
      onScreenId: PropTypes.string.isRequired,
      properties: PropTypes.object.isRequired,
      children: PropTypes.array.isRequired,
    }).isRequired,
  ).isRequired,
  selectedComponentId: PropTypes.string.isRequired,
  onToggleComponent: PropTypes.func.isRequired,
  showAllZones: PropTypes.bool.isRequired,
};

export default ComponentTree;
