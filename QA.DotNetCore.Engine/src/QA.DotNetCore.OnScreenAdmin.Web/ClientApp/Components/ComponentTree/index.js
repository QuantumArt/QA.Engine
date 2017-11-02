import React from 'react';
import PropTypes from 'prop-types';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';

const ComponentTree = ({ components, onSelectComponent }) => (
  <List>
    {components.map(component => (
      <ComponentItem
        {...component}
        key={component.properties.onScreenId}
        onSelectComponent={onSelectComponent}
      />))}
  </List>
);

ComponentTree.propTypes = {
  components: PropTypes.arrayOf(
    PropTypes.shape({
      parent: PropTypes.string.isRequired,
      type: PropTypes.string.isRequired,
      properties: PropTypes.object.isRequired,
      children: PropTypes.array.isRequired,
    }).isRequired,
  ).isRequired,
  onSelectComponent: PropTypes.func.isRequired,
};

export default ComponentTree;
