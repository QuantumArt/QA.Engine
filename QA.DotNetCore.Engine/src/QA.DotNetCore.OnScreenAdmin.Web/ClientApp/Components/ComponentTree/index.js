import React from 'react';
import ComponentItem from '../ComponentItem';
import EditComponent from '../EditComponentButton';
import EditComponentButton from '../EditComponentButton/button';

/* eslint-disable */
const ComponentTree = ({ components, onSelectComponent }) => (
  <ul>
    {components.map(component => (
      <ComponentItem
        {...component}
        key={component.onScreenId}
        onClick={() => onSelectComponent(component.onScreenId)}
      />
    ))}
    {components.map(component => (
      <EditComponent
        {...component}
        key={component.onScreenId}
        onClick={() => onSelectComponent(component.onScreenId)}
      >
        <EditComponentButton
          {...component}
          key={component.onScreenId}
          onClick={() => onSelectComponent(component.onScreenId)}
        />
      </EditComponent>
    ))}
  </ul>
);

export default ComponentTree;
