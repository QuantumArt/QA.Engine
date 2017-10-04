import _ from 'lodash';



var findParentComponent = function(component) {
  var currentElement = component;
  while(currentElement && currentElement.parentNode){
    currentElement = currentElement.parentNode;
    if(currentElement.dataset && currentElement.dataset.qaComponentType){
      return currentElement.dataset.qaComponentOnScreenId;
    }
  }
  return -1;
}

var mapComponent = function(domElement){
  return {
    type: domElement.dataset.qaComponentType,
    onScreenId: domElement.dataset.qaComponentOnScreenId,
    parentOnScreenId: domElement.dataset.qaComponentParentOnScreenId === '-1' ? null : domElement.dataset.qaComponentParentOnScreenId,
    childComponents: [],
    properties: mapComponentProperties(domElement)
  }
}

var mapComponentProperties = function(domElement){
  var data = domElement.dataset;
  if(data.qaComponentType === 'zone'){
    return {
      zoneName: data.qaZoneName,
      isRecursive: data.qaZoneIsRecursive,
      isGlobal: data.qaZoneIsGlobal
    };
  }
  else if(data.qaComponentType === 'widget'){
    return {
      widgetId : data.qaWidgetId,
      alias: data.qaWidgetAlias,
      title: data.qaWidgetTitle
    }
  }

  return null;
}

var findChildComponents = function(currentComponent, allComponents){
  var directChildren = _.filter(allComponents, function(c){
    return c.parentOnScreenId === currentComponent.onScreenId;
  });
  _.forEach(directChildren, function(child){
    child.childComponents = findChildComponents(child, allComponents);
  });

  return directChildren;
};


const buildTree = () => {
  console.log('build tree fired');
  var tree = {
    components: []
  };

  var allComponents = document.querySelectorAll("[data-qa-component-type]");
  var counter = 0;
  _.forEach(allComponents, function(component){
    component.dataset.qaComponentOnScreenId = counter++;
  });

  _.forEach(allComponents, function(component){
      component.dataset.qaComponentParentOnScreenId = findParentComponent(component);
    }
  );

  var components = _.map(allComponents, mapComponent);
  //возвращаем плоский список (для использования в store - лучше плоский)
  return components;

  // var rootComponents = _.filter(components, function(c){return c.parentOnScreenId == null});
  // _.forEach(rootComponents, function(component){
  //   component.childComponents = findChildComponents(component, components);
  //   tree.components.push(component);
  // });

  // return tree;
  
}

export default buildTree;
