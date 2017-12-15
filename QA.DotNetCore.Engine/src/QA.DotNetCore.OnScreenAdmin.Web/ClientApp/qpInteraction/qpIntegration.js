import { getNewUid, getHostId, getParent } from './utils';

export const getHostUID = () => getHostId();

// options : {id, contentId, callback, isInTab, isCreate, fieldsToSet, fieldsToBlock, fieldsToHide, action}
export const showQPForm = (options) => {
  const uid = getNewUid();
  let actionCode = options.action || 'edit_article';
  let entityTypeCode = 'article';

  if (actionCode === 'view_archive_article') { entityTypeCode = 'archive_article'; }

  const observer = new window.Quantumart.QP8.Interaction.BackendEventObserver(`observer${uid}`, ((eventType, args) => {
    console.log(eventType);
    console.log(args);
    if (args.actionUID !== uid) {
      console.log(`not matched: ${uid}`);
      return;
    }

    options.callback(eventType, args);
  }));

  if (options.isCreate) { actionCode = 'new_article'; }

  const executeOptions = new window.Quantumart.QP8.Interaction.ExecuteActionOtions();
  executeOptions.actionCode = actionCode;
  executeOptions.entityTypeCode = entityTypeCode;
  executeOptions.entityId = options.id;

  if (options.isCreate) {
    executeOptions.entityId = 0;
  }

  executeOptions.parentEntityId = options.contentId;
  executeOptions.actionUID = uid;
  executeOptions.callerCallback = observer.callbackProcName;
  executeOptions.options = new window.Quantumart.QP8.Interaction.ArticleFormState();

  if (options.fieldsToSet) {
    executeOptions.options.initFieldValues = options.fieldsToSet;
  }

  if (options.fieldsToBlock) {
    executeOptions.options.disabledFields = options.fieldsToBlock;
  }

  if (options.fieldsToHide) {
    executeOptions.options.hideFields = options.fieldsToHide;
  }

  // убираем save
  // executeOptions.options.disabledActionCodes = ['update_article'];

  const hostId = getHostId();

  console.log(`hostId: ${hostId}`);

  if (!options.isInTab) {
    executeOptions.isWindow = true;
    executeOptions.changeCurrentTab = false;
  } else {
    executeOptions.isWindow = false;
    executeOptions.changeCurrentTab = false;
  }

  const backendWnd = getParent();

  console.log(executeOptions);
  window.Quantumart.QP8.Interaction.executeBackendAction(executeOptions, hostId, backendWnd);
};
