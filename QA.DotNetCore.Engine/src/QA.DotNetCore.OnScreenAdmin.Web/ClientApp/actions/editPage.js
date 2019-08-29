
import {
  EDIT_PAGE_ACTIONS,
} from './actionTypes';

export default function editPage(currentPageId) {
  return { type: EDIT_PAGE_ACTIONS.EDIT_PAGE, currentPageId };
}
