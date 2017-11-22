import { showQPForm } from '../qpInteraction';

const editWidget = (widgetId, callback, abstractItemMetaInfo) => {
  const options = {
    id: widgetId,
    contentId: abstractItemMetaInfo.contentId,
    callback,
  };
  showQPForm(options);
};


export default editWidget;
