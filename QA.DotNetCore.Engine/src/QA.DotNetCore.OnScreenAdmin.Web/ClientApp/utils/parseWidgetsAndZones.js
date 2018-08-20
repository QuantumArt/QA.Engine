/* eslint-disable no-unused-vars */
import _ from 'lodash';
import { Queue } from './structures';


const startZonePrefix = 'start zone ';
const endZonePrefix = 'end zone ';
const startWidgetPrefix = 'start widget ';
const endWidgetPrefix = 'end widget ';

const isZone = val => val.startsWith(startZonePrefix);
const endZone = val => val.startsWith(endZonePrefix);
const isWidget = val => val.startsWith(startWidgetPrefix);
const endWidget = val => val.startsWith(endWidgetPrefix);

const mapProperties = (val) => {
  const pair = val.match(/(\w+)='([^']+)'/g);
  const initObj = {};
  if (isWidget(val)) {
    initObj.widgetId = Number(val.match(/(\d+)(?!(widget))/g)[0]);
  } else {
    initObj.title = val.match(/zone (\w+)/g)[0];
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
const constructElement = (type, val, id, parentId, nestLevel) => ({
  isSelected: false,
  isOpened: false,
  isDisabled: false,
  type,
  nestLevel,
  id,
  parentId,
  properties: mapProperties(val),
});

const parseWidgetsAndZones = () => {
  const result = [];
  const ctx = new Queue();
  const mapEl = (node) => {
    const val = node.nodeValue;
    if (isZone(val) || isWidget(val)) {
      let parentId = null;
      if (ctx.length !== 0) {
        parentId = ctx.peekLast().id;
      }
      const type = isZone(val) ? 'zone' : 'widget';

      ctx.enqueue(
        constructElement(type, val, _.uniqueId(type), parentId, ctx.length + 1),
      );
    } else if (endZone(val) || endWidget(val)) {
      result.push(ctx.peek());
      ctx.dequeue();
    }
  };

  const nodeFilter = (node) => {
    if (node.nodeType === Node.COMMENT_NODE) {
      return NodeFilter.FILTER_ACCEPT;
    }

    return NodeFilter.FILTER_SKIP;
  };
  const iterator = document.createTreeWalker(document.body, NodeFilter.SHOW_ALL, nodeFilter, false);
  let curNode;
  while (curNode = iterator.nextNode()) {
    mapEl(curNode);
  }

  return _.compact(result);
};

export default parseWidgetsAndZones;
