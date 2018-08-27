import _ from 'lodash';
import { getSubtreeState } from './componentTreeStateStorage';

/** @namespace window.startPageId */

const startZonePrefix = 'start zone ';
const endZonePrefix = 'end zone ';
const startWidgetPrefix = 'start widget ';
const endWidgetPrefix = 'end widget ';

const isZone = val => val.startsWith(startZonePrefix);
const endZone = val => val.startsWith(endZonePrefix);
const isWidget = val => val.startsWith(startWidgetPrefix);
const endWidget = val => val.startsWith(endWidgetPrefix);

/**
 * Parses comment string for backend data
 * @param {string} val - string, containing all entitie's properties
 * @returns {({
 *   isGlobal: boolean,
 *   isRecursive: boolean,
 *   zoneName: string
 * }|{
 *    alias: string,
 *    published: boolean,
 *    title: string,
 *    type: string,
 *    widgetId: number
 * })}
 */
const mapProperties = (val) => {
  const pair = val.match(/(\w+)='([^']+)'/g);
  const initObj = {};
  if (isWidget(val)) {
    initObj.widgetId = Number(val.match(/(\d+)(?!(widget))/g)[0]);
  } else {
    initObj.zoneName = val.match(/zone (\w+)/g)[0].replace('zone ', '');
  }

  return _.reduce(pair, (prev, cur) => {
    const parsed = cur.split('=');
    const key = parsed[0];
    const value = parsed[1].replace(/'/g, '');

    return {
      ...prev,
      [key]: (value === 'true' || value === 'false') ? value === 'true' : value,
    };
  }, initObj);
};

const storedState = getSubtreeState();
const isOpened = (onScreenId) => {
  if (!storedState) { return false; }
  return _.includes(storedState.openedNodes, onScreenId);
};

/**
 * Constructs list element using comment's data.
 * @param {('widget'|'zone')} type
 * @param {string} val - string, containing all entitie's properties
 * @param {string} onScreenId - unique component id
 * @param {string} parentOnScreenId
 * @param {number} nestLevel
 * @returns {{
 *  isSelected: boolean,
 *  isOpened: boolean,
 *  isDisabled: boolean,
 *  type: ('widget'|'zone'),
 *  nestLevel: number,
 *  onScreenId: string,
 *  parentOnScreenId: string,
 *  properties: Object
 *  }}
 */
const constructElement = (type, val, onScreenId, parentOnScreenId, nestLevel) => ({
  onScreenId,
  parentOnScreenId,
  isSelected: false,
  isOpened: isOpened(onScreenId),
  isDisabled: false,
  type,
  nestLevel,
  properties: mapProperties(val),
});

export default function buildFlatListNew() {
  const list = [];
  const hashMap = {};
  const stack = [];

  const mapEl = (node) => {
    const val = node.nodeValue;
    if (isZone(val) || isWidget(val)) {
      let parentOnScreenId = null;
      if (stack.length !== 0) {
        parentOnScreenId = stack[stack.length - 1].onScreenId;
      }

      const type = isZone(val) ? 'zone' : 'widget';
      const el = constructElement(type, val, _.uniqueId(type), parentOnScreenId, stack.length + 1);
      hashMap[el.onScreenId] = el;

      if (type === 'zone') {
        if (el.nestLevel === 1) {
          el.properties.parentAbstractItemId = el.properties.isGlobal
            ? Number(window.startPageId)
            : Number(window.currentPageId);
        } else {
          el.properties.parentAbstractItemId = hashMap[el.parentOnScreenId].properties.widgetId;
        }
      }

      stack.push(el);
    } else if (endZone(val) || endWidget(val)) {
      list.push(stack.pop());
    }
  };

  // const nodeFilter = (node) => {
  //   if (node.nodeType === Node.COMMENT_NODE) {
  //     return NodeFilter.FILTER_ACCEPT;
  //   }
  //
  //   return NodeFilter.FILTER_SKIP;
  // };
  const iterator = document.createNodeIterator(document.body, NodeFilter.SHOW_COMMENT);
  let curNode;
  while (curNode = iterator.nextNode()) {
    mapEl(curNode);
  }

  return list;
}
