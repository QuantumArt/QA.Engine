import React from 'react';
import Button from 'material-ui/Button';
import List from 'material-ui/List';
import ComponentItem from '../ComponentItem';
import EditComponent from '../EditComponent';

/* eslint-disable */
const ComponentTree = ({ components, onSelectComponent }) => (
  <List>
    {components.map(component => (
      <div key={`${component.onScreenId}-wrap`}>
        <ComponentItem
          {...component}
          key={`${component.onScreenId}-item`}
          onClick={() => onSelectComponent(component.onScreenId)}
        />
        <EditComponent
          {...component}
          key={`${component.onScreenId}-edit`}
          onClick={() => onSelectComponent(component.onScreenId)}
        >
          <Button
            key={`${component.onScreenId}-button`}
            onClick={() => onSelectComponent(component.onScreenId)}
          >
            Edit
          </Button>
        </EditComponent>
      </div>
    ))}
  </List>
);

export default ComponentTree;
