import _ from 'lodash';
/* eslint-disable no-param-reassign */

const findParentComponent = (component) => {
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
  }

  return {
    widgetId: data.qaWidgetId,
    alias: data.qaWidgetAlias,
    title: data.qaWidgetTitle,
  };
};

const mapComponent = domElement => ({
  type: domElement.dataset.qaComponentType,
  onScreenId: domElement.dataset.qaComponentOnScreenId,
  parentOnScreenId: domElement.dataset.qaComponentParentOnScreenId === '-1'
    ? null
    : domElement.dataset.qaComponentParentOnScreenId,
  properties: mapComponentProperties(domElement),
});


const getComponentOnScreenId = (component, parentComponent) => {
  let thisComponentPart = '';
  const data = component.dataset;
  if (data.qaComponentType === 'zone') {
    thisComponentPart += `;zone-${data.qaZoneName}`;
    if (JSON.parse(data.qaZoneIsRecursive)) {
      thisComponentPart += '|recursive';
    }
    if (JSON.parse(data.qaZoneIsGlobal)) {
      thisComponentPart += '|global';
    }
  } else {
    thisComponentPart += `;widget-${data.qaWidgetId}|alias-${data.qaWidgetAlias}|title-${data.qaWidgetTitle}`;
  }
  const parentComponentOnScreenId = parentComponent === null
    ? 'page'
    : parentComponent.dataset.qaComponentOnScreenId;

  return parentComponentOnScreenId + thisComponentPart;
};

const setComponentIds = (component) => {
  if (component.dataset.qaComponentOnScreenId && component.dataset.qaComponentParentOnScreenId) {
    return;
  }
  const parentComponent = findParentComponent(component);
  if (!parentComponent) {
    component.dataset.qaComponentParentOnScreenId = 'page';
    component.dataset.qaComponentOnScreenId = getComponentOnScreenId(component, null);
  } else if (parentComponent.dataset.qaComponentOnScreenId) {
    component.dataset.qaComponentParentOnScreenId = parentComponent.dataset.qaComponentOnScreenId;
    component.dataset.qaComponentOnScreenId = getComponentOnScreenId(component, parentComponent);
  } else {
    setComponentIds(parentComponent);
  }
};

const buildList = () => {
  const allComponents = document.querySelectorAll('[data-qa-component-type]');

  _.forEach(allComponents, (component) => {
    setComponentIds(component);
  });

  const components = _.map(allComponents, mapComponent);

  return components;
};

export default buildList;
