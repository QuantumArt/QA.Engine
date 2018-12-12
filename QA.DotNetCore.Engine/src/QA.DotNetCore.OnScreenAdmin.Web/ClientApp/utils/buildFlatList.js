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

export default function buildFlatList() {
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
      const onScreenId = _.uniqueId(type);
      const el = constructElement(type, val, onScreenId, parentOnScreenId, stack.length + 1);
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

      let widgetNesting = 0;
      let zoneNesting = 0;

      const findFirstSubNode = (startNode) => {
        if (startNode.nodeType !== node.COMMENT_NODE) {
          if (startNode.nodeName !== 'SCRIPT') {
            if ((startNode.innerHTML && /\S/.test(startNode.innerHTML)) ||
            (startNode.nodeValue && /\S/.test(startNode.nodeValue))) {
              return startNode;
            }
          }
        } else if ((type === 'widget' && startNode.nodeValue && isWidget(startNode.nodeValue)) ||
          (type === 'zone' && isZone(startNode.nodeValue))) {
          if (type === 'widget') {
            widgetNesting += 1;
          } else {
            zoneNesting += 1;
          }
        } else if ((type === 'widget' && startNode.nodeValue && endWidget(startNode.nodeValue)) ||
          (type === 'zone' && endZone(startNode.nodeValue))) {
          if (type === 'widget' && widgetNesting) {
            widgetNesting -= 1;
          } else if (type === 'zone' && zoneNesting) {
            zoneNesting -= 1;
          } else {
            return null;
          }
        }
        return findFirstSubNode(startNode.nextSibling);
      };
      let coordsRigtht = 0;
      const endNodeValue = type === 'widget' ? `end widget ${el.properties.widgetId}` : `end zone ${el.properties.zoneName}`;

      const findZoneLastSubNode = (startNode) => {
        if (startNode === null) {
          return null;
        }
        if (startNode.offsetWidth && (startNode.getBoundingClientRect().left + startNode.offsetWidth > coordsRigtht)) {
          coordsRigtht = startNode.getBoundingClientRect().left + pageXOffset + startNode.offsetWidth;
        }
        const getLastElemOrText = (prevNode) => {
          if ((prevNode.nodeType !== 8) && ((prevNode.innerHTML && /\S/.test(prevNode.innerHTML)) ||
          (prevNode.nodeValue && /\S/.test(prevNode.nodeValue)))) {
            return prevNode;
          }
          return getLastElemOrText(prevNode.previousSibling);
        };
        if (startNode.nextSibling.nodeType === 8 &&
          startNode.nextSibling.nodeValue.startsWith(endNodeValue)) {
          return getLastElemOrText(startNode);
        }
        return findZoneLastSubNode(startNode.nextSibling);
      };

      const firstSubElem = findFirstSubNode(node.nextSibling);
      const lastSubElem = findZoneLastSubNode(firstSubElem);
      const componentCoords = {};

      // if first node is text, create a div-wrap
      if (firstSubElem && firstSubElem.nodeType === 3) {
        const clonedText = firstSubElem.textContent;
        const tempWrap = document.createElement('div');
        tempWrap.style.visibility = 'hidden';
        tempWrap.style.display = 'inline-block';
        tempWrap.appendChild(firstSubElem.cloneNode(true));
        node.parentNode.insertBefore(tempWrap, firstSubElem);
        firstSubElem.textContent = '';
        componentCoords.top = tempWrap.getBoundingClientRect().top + pageYOffset;
        componentCoords.left = tempWrap.getBoundingClientRect().left + pageXOffset;

        if (firstSubElem === lastSubElem) {
          componentCoords.width = tempWrap.offsetWidth;
          componentCoords.height = tempWrap.offsetHeight;
        }
        node.parentNode.removeChild(tempWrap);
        firstSubElem.textContent = clonedText;
      } else if (firstSubElem) {
        componentCoords.top = firstSubElem.getBoundingClientRect().top + pageYOffset;
        componentCoords.left = firstSubElem.getBoundingClientRect().left + pageXOffset;

        if (firstSubElem === lastSubElem) {
          componentCoords.width = firstSubElem.offsetWidth;
          componentCoords.height = firstSubElem.offsetHeight;
        }
      }

      if (lastSubElem && ((lastSubElem.nodeType === 3 && firstSubElem !== lastSubElem) || !lastSubElem.offsetHeight)) {
        const clonedText = lastSubElem.textContent;
        const tempWrapLast = document.createElement('div');
        tempWrapLast.style.visibility = 'hidden';
        tempWrapLast.style.display = 'inline-block';
        tempWrapLast.appendChild(lastSubElem.cloneNode(true));
        node.parentNode.insertBefore(tempWrapLast, lastSubElem.nextSibling);
        lastSubElem.textContent = '';
        if (tempWrapLast.getBoundingClientRect().left > coordsRigtht) {
          coordsRigtht = tempWrapLast.getBoundingClientRect().left + tempWrapLast.offsetWidth;
        }
        componentCoords.width = coordsRigtht - componentCoords.left; // Width
        componentCoords.height = ((tempWrapLast.getBoundingClientRect().top + pageYOffset) - componentCoords.top)
          + tempWrapLast.offsetHeight; // Height
        node.parentNode.removeChild(tempWrapLast);
        lastSubElem.textContent = clonedText;
      } else if (lastSubElem && firstSubElem !== lastSubElem) {
        if (lastSubElem.getBoundingClientRect().left + lastSubElem.offsetWidth > coordsRigtht) {
          coordsRigtht = lastSubElem.getBoundingClientRect().left + lastSubElem.offsetWidth;
        }
        componentCoords.width = coordsRigtht - componentCoords.left; // Width
        componentCoords.height = ((lastSubElem.getBoundingClientRect().top + pageYOffset) - componentCoords.top)
        + lastSubElem.offsetHeight; // Height
      }

      el.properties.componentCoords = componentCoords;
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
