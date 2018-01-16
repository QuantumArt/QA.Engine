import _ from 'lodash';
import { getSubtreeState } from './componentTreeStateStorage';
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

const getZoneParentPageId = (zoneData) => {
  const isGlobal = JSON.parse(zoneData.qaZoneIsGlobal);
  const zoneParentPageId = isGlobal ? window.startPageId : window.currentPageId;
  return zoneParentPageId;
};

const mapComponentProperties = (domElement) => {
  const data = domElement.dataset;
  if (data.qaComponentType === 'zone') {
    const isGlobal = JSON.parse(data.qaZoneIsGlobal);
    const zoneParentPageId = getZoneParentPageId;
    return {
      zoneName: data.qaZoneName,
      isRecursive: JSON.parse(data.qaZoneIsRecursive),
      isGlobal,
      parentPageId: zoneParentPageId,
      parentAbstractItemId: data.qaComponentParentAbstractItemId,
    };
  }

  return {
    widgetId: data.qaWidgetId,
    alias: data.qaWidgetAlias,
    title: data.qaWidgetTitle,
  };
};

const storedState = getSubtreeState();

const getIsSelected = (onScreenId) => {
  if (!storedState) { return false; }
  return _.includes(storedState.openedNodes, onScreenId);
};

const mapComponent = domElement => ({
  isSelected: false,
  isOpened: getIsSelected(domElement.dataset.qaComponentOnScreenId),
  type: domElement.dataset.qaComponentType,
  onScreenId: domElement.dataset.qaComponentOnScreenId,
  parentOnScreenId: domElement.dataset.qaComponentParentOnScreenId === '-1'
    ? null
    : domElement.dataset.qaComponentParentOnScreenId,
  nestLevel: domElement.dataset.qaComponentOnScreenId.split(';').length - 1,
  properties: mapComponentProperties(domElement),
  isDisabled: false,
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

// const getComponentParentAbstractItemId = () => '';

const getComponentParentAbstractItemId = (component) => {
  const data = component.dataset;
  let parentComponent = findParentComponent(component);
  if (data.qaComponentType === 'zone') {
    while (parentComponent && parentComponent.dataset.qaComponentType !== 'widget') {
      parentComponent = findParentComponent(parentComponent);
    }
    if (!parentComponent) { // зона от страницы
      return getZoneParentPageId(data);
    }
    // нашли виджет
    return parentComponent.dataset.qaWidgetId;
  }
  // пока возвращаем только для зон, при необходимости - допилить
  return '';
};

const setComponentIds = (component) => {
  if (component.dataset.qaComponentOnScreenId && component.dataset.qaComponentParentOnScreenId) {
    return;
  }
  const parentComponent = findParentComponent(component);
  component.dataset.qaComponentParentAbstractItemId = getComponentParentAbstractItemId(component);
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
