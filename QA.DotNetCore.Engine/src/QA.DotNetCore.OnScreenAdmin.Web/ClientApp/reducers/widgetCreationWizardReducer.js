import _ from 'lodash';
import {
  BEGIN_WIDGET_CREATION,
  SELECT_CUSTOM_ZONE,
  SELECT_TARGET_ZONE,
  SELECT_WIDGET_TYPE,
  GO_TO_PREV_STEP,
  // SHOW_QP_FORM,
} from 'actions/widgetCreation/actionTypes';
import { WIDGET_CREATION_MODE, WIDGET_CREATION_STEPS } from 'constants/widgetCreation';

const initialState = {
  isActive: false,
  creationMode: null, // WIDGET_CREATION_MODE
  parentOnScreenId: null, // 'page' || widgetOnScreenId || widgetOnScreenId (если добавляем в конкретную зону - находим ее парента (страница/виджет))
  targetZoneName: null, // название зоны
  isCustomTargetZone: false, // Добавляем в кастомную зону
  selectedWidgetId: null, // айди выбранного типа виджета для добавления
  availableWidgetsLoaded: false, // получили ли инфу о доступных для добавления виджетах
  currentStep: WIDGET_CREATION_STEPS.INACTIVE,
};

const steps = [
  WIDGET_CREATION_STEPS.INACTIVE,
  WIDGET_CREATION_STEPS.ZONES_LIST,
  WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER,
  WIDGET_CREATION_STEPS.SHOW_AVAILABLE_WIDGETS,
  WIDGET_CREATION_STEPS.SHOW_QP_FORM,
  WIDGET_CREATION_STEPS.FINISH,
];

const getDisabledSteps = (creationMode, isCustomZone) => {
  switch (creationMode) {
    case WIDGET_CREATION_MODE.PAGE_CHILD:
      return isCustomZone ? [] : [WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER];
    case WIDGET_CREATION_MODE.WIDGET_CHILD:
      return isCustomZone ? [] : [WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER];
    case WIDGET_CREATION_MODE.SPECIFIC_ZONE:
      return [WIDGET_CREATION_STEPS.ZONES_LIST, WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER];
    default:
      return [];
  }
};

const getEffectiveSteps = (creationMode, isCustomZone) => {
  const disabledSteps = getDisabledSteps(creationMode, isCustomZone);
  return _.filter(steps, disabledSteps);
};

const getPreviousStep = (currentState) => {
  const effectiveSteps = getEffectiveSteps(currentState.creationMode, currentState.isCustomTargetZone);
  const currentStepIndex = _.findIndex(effectiveSteps, currentState.currentStep);
  return effectiveSteps[currentStepIndex - 1];
};

const getNextStep = (creationMode, isCustomZone, currentStep) => {
  const effectiveSteps = getEffectiveSteps(creationMode, isCustomZone);
  const currentStepIndex = _.findIndex(effectiveSteps, currentStep);
  return effectiveSteps[currentStepIndex + 1];
};

const goToPrevStep = (currentState) => {
  const prevStep = getPreviousStep(currentState);
  switch (prevStep) {
    case WIDGET_CREATION_STEPS.INACTIVE:
      return initialState;
    case WIDGET_CREATION_STEPS.ZONES_LIST:
      return {
        ...currentState,
        availableWidgetsLoaded: initialState.availableWidgetsLoaded,
        selectedWidgetId: initialState.selectedWidgetId,
        isCustomTargetZone: initialState.isCustomTargetZone,
        targetZoneName: initialState.targetZoneName,
        currentStep: WIDGET_CREATION_STEPS.ZONES_LIST,
      };
    case WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER:
      return {
        ...currentState,
        availableWidgetsLoaded: initialState.availableWidgetsLoaded,
        selectedWidgetId: initialState.selectedWidgetId,
        targetZoneName: initialState.targetZoneName,
        currentStep: WIDGET_CREATION_STEPS.CUSTOM_ZONE_NAME_ENTER,
      };
    case WIDGET_CREATION_STEPS.SHOW_AVAILABLE_WIDGETS:
      return {
        ...currentState,
        selectedWidgetId: initialState.selectedWidgetId,
        currentStep: WIDGET_CREATION_STEPS.SHOW_AVAILABLE_WIDGETS,
      };

    default:
      return currentState;
  }
};

export default function widgetCreationWizardReducer(state = initialState, action) {
  switch (action.type) {
    case BEGIN_WIDGET_CREATION:
      return {
        ...state,
        isActive: true,
        creationMode: action.payload.creationMode,
        parentOnScreenId: action.payload.creationMode,
        targetZoneName: action.payload.targetZoneName,
        currentStep: getNextStep(action.payload.creationMode, false, WIDGET_CREATION_STEPS.INACTIVE),
      };
    case SELECT_CUSTOM_ZONE:
      return {
        ...state,
        isCustomTargetZone: true,
        currentStep: getNextStep(state.creationMode, true, WIDGET_CREATION_STEPS.ZONES_LIST),
      };
    case SELECT_TARGET_ZONE:
      return {
        ...state,
        targetZoneName: action.payload.targetZoneName,
        currentStep: getNextStep(state.creationMode, state.isCustomTargetZone, WIDGET_CREATION_STEPS.ZONES_LIST),
      };
    case SELECT_WIDGET_TYPE:
      return {
        ...state,
        selectedWidgetId: action.payload.selectedWidgetId,
        // currentStep: getNextStep(state.creationMode, state.isCustomTargetZone, WIDGET_CREATION_STEPS.SHOW_AVAILABLE_WIDGETS),
      };


    default:
      return state;
  }
}
