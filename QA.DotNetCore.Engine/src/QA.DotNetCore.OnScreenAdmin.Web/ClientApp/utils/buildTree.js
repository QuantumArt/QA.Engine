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

const mapComponentProperties = (domElement) => {
  const data = domElement.dataset;
  if (data.qaComponentType === 'zone') {
    return {
      reduxAlias: `${data.qaComponentType}-${data.qaZoneName}`,
      zoneName: data.qaZoneName,
      isRecursive: JSON.parse(data.qaZoneIsRecursive),
      isGlobal: JSON.parse(data.qaZoneIsGlobal),
    };
  }

  return {
    reduxAlias: `${data.qaComponentType}-${data.qaWidgetId}`,
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
