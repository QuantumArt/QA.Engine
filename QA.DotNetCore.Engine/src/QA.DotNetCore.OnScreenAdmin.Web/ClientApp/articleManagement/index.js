import _ from 'lodash';
import { showQPForm } from '../qpInteraction';

const fieldNameResolver = (contentMetaInfo, columnName) => {
  if (!contentMetaInfo || !contentMetaInfo.contentAttributes) {
    return null;
  }
  const fieldInfo = _.find(contentMetaInfo.contentAttributes, { netName: columnName });
  if (!fieldInfo || !fieldInfo.invariantName) {
    return null;
  }
  return fieldInfo.invariantName;
};

export const editWidget = (widgetId, callback, abstractItemMetaInfo) => {
  const options = {
    id: widgetId,
    contentId: abstractItemMetaInfo.contentId,
    callback,
  };
  showQPForm(options);
};

export const addWidget = (widgetToAdd, zoneToAdd, abstractItemMetaInfo, callback) => {
  const fieldsToSet = [
    { fieldName: fieldNameResolver(abstractItemMetaInfo, 'Parent'), value: zoneToAdd.properties.parentPageId },
    { fieldName: fieldNameResolver(abstractItemMetaInfo, 'ZoneName'), value: zoneToAdd.properties.zoneName },
    { fieldName: fieldNameResolver(abstractItemMetaInfo, 'Discriminator'), value: widgetToAdd.id },
    { fieldName: fieldNameResolver(abstractItemMetaInfo, 'ExtensionId'), value: widgetToAdd.preferredContentId },
    { fieldName: fieldNameResolver(abstractItemMetaInfo, 'IsPage'), value: false },
  ];
  const fieldsToBlock = [
    fieldNameResolver(abstractItemMetaInfo, 'Parent'),
    fieldNameResolver(abstractItemMetaInfo, 'IsPage'),
    fieldNameResolver(abstractItemMetaInfo, 'Discriminator'),
    fieldNameResolver(abstractItemMetaInfo, 'VersionOf'),
    fieldNameResolver(abstractItemMetaInfo, 'ExtensionId'),
  ];
  const fieldsToHide = [
    fieldNameResolver(abstractItemMetaInfo, 'IsInSiteMap'),
  ];
  const options = {
    isCreate: true,
    contentId: abstractItemMetaInfo.contentId,
    fieldsToSet,
    fieldsToBlock,
    fieldsToHide,
    callback,
  };

  showQPForm(options);
};

