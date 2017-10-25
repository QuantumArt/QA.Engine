import * as types from '../actions/actionTypes';

const initialState = {opened: false};

export default function sidebarReducer(state=initialState, action) {
  switch(action.type){
    case types.TOGGLE_OPEN_STATE: 
      return {...state, opened: !state.opened};
    default:
      return state;
  }
}