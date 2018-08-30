import { CHANGE_AVAILABLE_WIDGETS_SEARCH_TEXT } from '../actions/actionTypes';

const initialState = {
  searchText: '',
};

export default function availableWidgetsReducer(state = initialState, action) {
  switch (action.type) {
    case CHANGE_AVAILABLE_WIDGETS_SEARCH_TEXT:
      return {
        ...state,
        searchText: action.value,
      };
    default:
      return state;
  }
}
