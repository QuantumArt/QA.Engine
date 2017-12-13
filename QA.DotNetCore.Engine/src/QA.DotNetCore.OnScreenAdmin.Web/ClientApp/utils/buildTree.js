import _ from 'lodash';

const findChildComponents = (currentComponent, allComponents, allOpened) => {
  const directChildren = _.filter(allComponents, c => c.parentOnScreenId === currentComponent.onScreenId);
  _.forEach(directChildren, (child) => {
    child.children = findChildComponents(child, allComponents);
    if (allOpened && _.size(child.children) > 0) {
      child.isOpened = true;
    }
  });

  return directChildren;
};

const buildTree = (componentsList, allOpened = false) => {
  const tree = [];
  const componentsClone = _.cloneDeep(componentsList);
  const rootComponents = _.filter(componentsClone, c => c.parentOnScreenId === 'page');
  _.forEach(rootComponents, (component) => {
    component.children = findChildComponents(component, componentsClone, allOpened);
    if (allOpened && _.size(component.children) > 0) {
      component.isOpened = true;
    }
    tree.push(component);
  });

  return tree;
};


export default buildTree;

