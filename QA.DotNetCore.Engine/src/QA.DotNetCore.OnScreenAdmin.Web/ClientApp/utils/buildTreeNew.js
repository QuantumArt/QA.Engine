import _ from 'lodash';

const getWidgetTypeIconSrc = (component, availableWidgets) => {
  if (availableWidgets === null || availableWidgets.length === 0) {
    return null;
  }
  const availableWidget = _.find(availableWidgets, { discriminator: component.properties.type });
  if (availableWidget && availableWidget.iconUrl) {
    return availableWidget.iconUrl;
  }

  return null;
};

export default function buildTreeNew(list, disabledComponents = [], allOpened = false, availableWidgets = []) {
  const _list = _.cloneDeep(list);
  const hashMap = {};
  _.forEach(_list, (component) => {
    component.children = [];
    hashMap[component.onScreenId] = component;
  });

  const tree = [];
  _.forEach(hashMap, (component) => {
    component.isDisabled = _.indexOf(disabledComponents, component.onScreenId) !== -1;
    if (allOpened && component.children.length > 0) {
      component.isOpened = true;
    }
    if (component.type === 'widget') {
      component.properties.widgetTypeIconSrc = getWidgetTypeIconSrc(component, availableWidgets);
    }
    if (hashMap[component.parentOnScreenId]) {
      hashMap[component.parentOnScreenId].children.push(component);
    }
    if (component.parentOnScreenId === null) {
      tree.push(component);
    }
  });

  return tree;
}
