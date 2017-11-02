// import _ from 'lodash';

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

const setComponentIds = (component, parentComponent) => {
  if (component.dataset.qaComponentOnScreenId && component.dataset.qaComponentParentOnScreenId) {
    return;
  }

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

const mapComponentProperties = (domElement) => {
  const data = domElement.dataset;
  if (data.qaComponentType === 'zone') {
    return {
      onScreenId: data.qaComponentOnScreenId,
      zoneName: data.qaZoneName,
      isRecursive: JSON.parse(data.qaZoneIsRecursive),
      isGlobal: JSON.parse(data.qaZoneIsGlobal),
    };
  }

  return {
    onScreenId: data.qaComponentOnScreenId,
    widgetId: data.qaWidgetId,
    alias: data.qaWidgetAlias,
    title: data.qaWidgetTitle,
  };
};

const mapComponent = (domElement, parent) => ({
  parent,
  type: domElement.dataset.qaComponentType,
  properties: mapComponentProperties(domElement),
  children: [],
});

const buildTree = () => {
  const root = [];
  const temporary = [];

  const allComponents = document.querySelectorAll('[data-qa-component-type]');
  for (let i = 0; i < allComponents.length; i += 1) {
    const currentElement = allComponents[i];
    const parent = findParentComponent(currentElement);
    setComponentIds(currentElement, parent);

    if (parent == null) {
      const dataObject = mapComponent(currentElement, 'page');
      root.push(dataObject);
      temporary.push({
        element: currentElement,
        dataObject,
      });
    } else {
      const tempParent = temporary.find(item => item.element === parent);
      const dataObject = mapComponent(currentElement, tempParent.dataObject);
      tempParent.dataObject.children.push(dataObject);
      temporary.push({
        element: currentElement,
        dataObject,
      });
    }
  }

  return root;
};

export default buildTree;

//
