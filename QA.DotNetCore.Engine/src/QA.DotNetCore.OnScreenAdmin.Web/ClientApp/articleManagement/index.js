import { showQPForm } from '../qpInteraction';

export const editWidget = (widgetId, callback, abstractItemMetaInfo) => {
  const options = {
    id: widgetId,
    contentId: abstractItemMetaInfo.contentId,
    callback,
  };
  showQPForm(options);
};

export const addWidget = () => {
  const options = {};

  showQPForm(options);
};

