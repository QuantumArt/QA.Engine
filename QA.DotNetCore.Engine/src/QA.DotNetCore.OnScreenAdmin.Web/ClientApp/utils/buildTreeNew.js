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

export default function buildTreeNew(
  list,
  disabledComponents = [],
  allOpened = false,
  availableWidgets = [],
  showOnlyWidgets = false,
) {
  const _list = _.cloneDeep(list);
  const hashMap = {};
  _.forEach(_list, (component) => {
    component.children = [];
    hashMap[component.onScreenId] = component;
  });
  // if (showOnlyWidgets) {
  //   const shiftParentOnscreenID = () => _.forEach(hashMap, (component) => {
  //     function findParentWidget(parentID) {
  //       if (hashMap[parentID] && !(hashMap[parentID].type === 'widget' || parentID === null)) {
  //         return findParentWidget(hashMap[parentID].parentOnScreenId);
  //       }
  //       return parentID;
  //     }
  //
  //     if (component.parentOnScreenId !== null) {
  //       component.parentOnScreenId = findParentWidget(component.parentOnScreenId);
  //     }
  //   });
  //   shiftParentOnscreenID();
  //   _.filter(hashMap, c => c.type === 'widget');
  // }
  const tree = [];
  _.forEach(hashMap, (component) => {
    component.isDisabled = _.indexOf(disabledComponents, component.onScreenId) !== -1;
    if (allOpened && component.children.length > 0) {
      component.isOpened = true;
    }
    if (component.type === 'widget') {
      component.properties.widgetTypeIconSrc = getWidgetTypeIconSrc(component, availableWidgets);
    }

    const parent = hashMap[component.parentOnScreenId];

    if (showOnlyWidgets && parent && component.type === 'widget') {
      component.parentOnScreenId = parent.parentOnScreenId;
    }

    if (parent) {
      if (showOnlyWidgets) {
        if (component.type === 'widget') {
          parent.children.push(component);
        }
      } else {
        parent.children.push(component);
      }
    }
    if (component.parentOnScreenId === null) {
      if (showOnlyWidgets) {
        if (component.type === 'widget') {
          tree.push(component);
        }
      } else {
        tree.push(component);
      }
    }
  });
  return tree;
}
