import { connect } from 'react-redux';
import {
  editWidget,
  addWidgetToZone,
} from '../actions/componentControlMenuActions';

import ComponentControlMenu from '../Components/ComponentControlMenu';

const mapDispatchToProps = dispatch => ({
  onEditWidget: (id) => {
    dispatch(editWidget(id));
  },
  onAddWidget: (id) => {
    dispatch(addWidgetToZone(id));
  },
});

const ComponentControlMenuContainer = connect(
  null,
  mapDispatchToProps,
)(ComponentControlMenu);

export default ComponentControlMenuContainer;
