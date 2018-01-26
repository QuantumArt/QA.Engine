import { CONTENT_META_INFO_ACTION } from '../actions/actionTypes';

const initialState = {
  availableWidgets: null,
  abstractItemMetaInfo: null,
  isLoading: false,
  error: null,
};

export default function metaInfoReducer(state = initialState, action) {
  switch (action.type) {
    case CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_REQUESTED:
      return {
        ...state,
        isLoading: true,
      };
    case CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        abstractItemMetaInfo: action.info,
      };

    case CONTENT_META_INFO_ACTION.GET_ABSTRACT_ITEM_INFO_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.error,
      };
    case CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_REQUESTED:
      return {
        ...state,
        isLoading: true,
      };
    case CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null,
        availableWidgets: action.availableWidgets,
      };
    case CONTENT_META_INFO_ACTION.GET_AVAILABLE_WIDGETS_FAIL:
      return {
        ...state,
        isLoading: false,
        error: action.error,
      };
    default:
      return state;
  }
}
