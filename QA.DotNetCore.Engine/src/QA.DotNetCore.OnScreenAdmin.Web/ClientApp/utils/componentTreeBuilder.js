import _ from 'lodash';
/* eslint-disable no-param-reassign */

const findParentComponent = (component) => {
  console.log(component);
  let currentElement = component;
  while (currentElement && currentElement.parentNode) {
    currentElement = currentElement.parentNode;
    if (currentElement.dataset && currentElement.dataset.qaComponentType) {
      return currentElement;
    }
  }
  return null;
};

const mapComponentProperties = (domElement) => {
  const data = domElement.dataset;
  if (data.qaComponentType === 'zone') {
    return {
      zoneName: data.qaZoneName,
      isRecursive: JSON.parse(data.qaZoneIsRecursive),
      isGlobal: JSON.parse(data.qaZoneIsGlobal),
    };
  } else if (data.qaComponentType === 'widget') {
    return {
      widgetId: data.qaWidgetId,
      alias: data.qaWidgetAlias,
      title: data.qaWidgetTitle,
    };
  }

  return null;
};

const mapComponent = domElement => ({
  type: domElement.dataset.qaComponentType,
  onScreenId: domElement.dataset.qaComponentOnScreenId,
  parentOnScreenId: domElement.dataset.qaComponentParentOnScreenId === '-1' ? null : domElement.dataset.qaComponentParentOnScreenId,
  childComponents: [],
  properties: mapComponentProperties(domElement),
});

// var findChildComponents = function(currentComponent, allComponents){
//   var directChildren = _.filter(allComponents, function(c){
//     return c.parentOnScreenId === currentComponent.onScreenId;
//   });
//   _.forEach(directChildren, function(child){
//     child.childComponents = findChildComponents(child, allComponents);
//   });

//   return directChildren;
// };

const getComponentOnScreenId = (component, parentComponent) => {
  let thisComponentPart = '';
  const data = component.dataset;
  if (data.qaComponentType === 'zone') {
    thisComponentPart += `;zone-${data.qaZoneName}`;
    if (JSON.parse(data.qaZoneIsRecursive)) { thisComponentPart += '|recursive'; }

    if (JSON.parse(data.qaZoneIsGlobal)) { thisComponentPart += '|global'; }
  } else if (data.qaComponentType === 'widget') {
    thisComponentPart += `;widget-${data.qaWidgetId}|alias-${data.qaWidgetAlias}|title-${data.qaWidgetTitle}`;
  }
  const parentComponentOnScreenId = parentComponent === null ? 'page' : parentComponent.dataset.qaComponentOnScreenId;
  return parentComponentOnScreenId + thisComponentPart;
};

const fillComponentIds = (component) => {
  if (component.dataset.qaComponentOnScreenId && component.dataset.qaComponentParentOnScreenId) { return; }
  const parentComponent = findParentComponent(component);
  if (!parentComponent) {
    component.dataset.qaComponentParentOnScreenId = 'page';
    component.dataset.qaComponentOnScreenId = getComponentOnScreenId(component, null);
  } else if (parentComponent.dataset.qaComponentOnScreenId) {
    component.dataset.qaComponentParentOnScreenId = parentComponent.dataset.qaComponentOnScreenId;
    component.dataset.qaComponentOnScreenId = getComponentOnScreenId(component, parentComponent);
  } else {
    fillComponentIds(parentComponent);
  }
};

const buildTree = () => {
  console.log('build tree fired');
  // var tree = {
  //   components: []
  // };

  const allComponents = document.querySelectorAll('[data-qa-component-type]');
  // const counter = 0;

  _.forEach(allComponents, (component) => {
    fillComponentIds(component);
  });

  // _.forEach(allComponents, function(component){
  //   component.dataset.qaComponentOnScreenId = counter++;
  // });

  // _.forEach(allComponents, function(component){
  //     component.dataset.qaComponentParentOnScreenId = findParentComponent(component);
  //   }
  // );

  const components = _.map(allComponents, mapComponent);
  // возвращаем плоский список (для использования в store - лучше плоский)
  return components;

  // var rootComponents = _.filter(components, function(c){return c.parentOnScreenId == null});
  // _.forEach(rootComponents, function(component){
  //   component.childComponents = findChildComponents(component, components);
  //   tree.components.push(component);
  // });

  // return tree;
};

export default buildTree;
