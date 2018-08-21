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
    if (hashMap[el.parentId]) {
      hashMap[el.parentId].children.push(el);
    }
    if (!el.parentId) {
      tree.push(el);
    }
  });

  return tree;
}
