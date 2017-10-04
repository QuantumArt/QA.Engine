import React from 'react';
import PropTypes from 'prop-types';
import WidgetComponentTreeItem from './widget';
import ZoneComponentTreeItem from './zone';


const ComponentItem = ({onClick, type, properties}) => {
  const item = type === 'widget' 
    ? <WidgetComponentTreeItem onClick={onClick} properties = {properties}/>
    : <ZoneComponentTreeItem onClick={onClick} properties = {properties}/> ;

    return (item)
}

ComponentItem.PropTypes = {
  onClick: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  properties : PropTypes.oneOfType([
    PropTypes.shape({
      widgetId : PropTypes.string.isRequired,
      title: PropTypes.string.isRequired
    }),
    PropTypes.shape({
      zoneName: PropTypes.string.isRequired
    })
  ])  
}


export default ComponentItem;