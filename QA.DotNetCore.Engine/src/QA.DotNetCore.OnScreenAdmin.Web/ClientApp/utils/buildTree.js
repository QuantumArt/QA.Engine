import _ from 'lodash';


const findChildComponents = (currentComponent, allComponents) => {
  const directChildren = _.filter(allComponents, c => c.parentOnScreenId === currentComponent.onScreenId);
  _.forEach(directChildren, (child) => {
    child.children = findChildComponents(child, allComponents);
  });

  return directChildren;
};

const buildTree = (componentsList) => {
  const tree = [];
  const componentsClone = _.cloneDeep(componentsList);
  const rootComponents = _.filter(componentsClone, c => c.parentOnScreenId === 'page');
  _.forEach(rootComponents, (component) => {
    component.children = findChildComponents(component, componentsList);
    tree.push(component);
  });

  return tree;
};


export default buildTree;

