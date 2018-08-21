import _ from 'lodash';

export default function buildTreeNew(list) {
  const _list = _.cloneDeep(list);
  const hashMap = {};
  _.forEach(_list, (el) => {
    el.children = [];
    hashMap[el.id] = el;
  });

  const tree = [];
  _.forEach(hashMap, (el) => {
    // fill children
    if (hashMap[el.parentId]) {
      hashMap[el.parentId].children.push(el);
    }
    // fill parentAbstractId
    if (el.type === 'zone') {
      if (el.nestLevel === 1) {
        el.properties.parentAbstractItemId = el.properties.isGlobal ? window.startPageId : window.currentPageId;
      } else {
        el.properties.parentAbstractItemId = hashMap[el.parentId].properties.widgetId; // TBD
      }
    }
    // form a tree
    if (!el.parentId) {
      tree.push(el);
    }
  });

  return tree;
}
