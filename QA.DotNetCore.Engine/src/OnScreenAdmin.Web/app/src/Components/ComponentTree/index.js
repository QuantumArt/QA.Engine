import React from 'react';
import ComponentItem from '../ComponentItem'
import EditComponent from '../EditComponentButton'
import EditComponentButton from '../EditComponentButton/button'


const ComponentTree = ({components, onSelectComponent}) => (
  <ul>
      {components.map(component => (
        <ComponentItem key={component.onScreenId} {...component} onClick={() => onSelectComponent(component.onScreenId)} />        
      ))}
      {components.map(component => (
        <EditComponent key={component.onScreenId} {...component} onClick={() => onSelectComponent(component.onScreenId)}>
          <EditComponentButton key={component.onScreenId} {...component} onClick={() => onSelectComponent(component.onScreenId)}/>
        </EditComponent>
      ))}
  </ul>
)

export default ComponentTree;