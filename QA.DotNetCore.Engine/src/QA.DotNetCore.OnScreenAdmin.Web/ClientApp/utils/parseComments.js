const parseWidgetsAndZones = () => {
  const parseComment = (node) => {
    const startZonePrefix = 'start zone ';
    const startWidgetPrefix = 'start widget ';
    const endZonePrefix = 'end zone ';
    const endWidgetPrefix = 'end widget ';

    const constructElement = (isZone, isWidget, val, isStart) =>
      ({ isStart, isZone, isWidget, val: val.trim(), isNothing: false });

    const constructNothing = () => ({ isNothing: true });

    const val = node.nodeValue;
    if (val.startsWith(startZonePrefix)) {
      return constructElement(true, false, val.substr(startZonePrefix.length), true);
    } else if (val.startsWith(startWidgetPrefix)) {
      return constructElement(false, true, val.substr(startWidgetPrefix.length), true);
    } else if (val.startsWith(endZonePrefix)) {
      return constructElement(true, false, val.substr(endZonePrefix.length), false);
    } else if (val.startsWith(endWidgetPrefix)) {
      return constructElement(false, true, val.substr(endWidgetPrefix.length), false);
    }
    return constructNothing();
  };

  const peek = stack => stack[stack.length - 1];

  const findWidgetsAndZones = (root, ctxStack, result) => {
    if (root.hasChildNodes()) {
      for (let i = 0; i < root.childNodes.length; i += 1) {
        const node = root.childNodes[i];
        if (node.nodeType === 8) {
          const current = parseComment(node);
          if (current.isNothing) return;
          if (current.isStart) {
            const el =
              { isZone: current.isZone, isWidget: current.isWidget, val: current.val, node, children: new Array(0) };
            if (ctxStack.length === 0) {
              result.push(el);
            } else {
              peek(ctxStack).children.push(el);
            }

            ctxStack.push(el);
          } else {
            ctxStack.pop();
          }
        } else {
          findWidgetsAndZones(node, ctxStack, result);
        }
      }
    }
  };

  const ctxStack = new Array(0);
  const result = new Array(0);
  findWidgetsAndZones(document.body, ctxStack, result);

  return result;
};

export default parseWidgetsAndZones;
