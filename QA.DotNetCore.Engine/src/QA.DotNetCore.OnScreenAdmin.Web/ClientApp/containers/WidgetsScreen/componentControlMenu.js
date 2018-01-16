import { connect } from 'react-redux';
import _ from 'lodash/core';
import {
  editWidget,
  addWidgetToZone,
  moveWidget,
} from '../../actions/componentControlMenuActions';

import ComponentControlMenu from '../../Components/WidgetsScreen/ComponentTreeScreen/ComponentControlMenu';

const mapStateToProps = (state, ownProps) => {
  const component = _.find(state.componentTree.components, { onScreenId: ownProps.onScreenId });
  const type = (component == null) ? '' : component.type;
  return {
    type,
  };
};

const mapDispatchToProps = dispatch => ({
  onEditWidget: (id) => {
    dispatch(editWidget(id));
  },
  onAddWidget: (id) => {
    dispatch(addWidgetToZone(id));
  },
  onMoveWidget: (id) => {
    dispatch(moveWidget(id));
  },
});

const ComponentControlMenuContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentControlMenu);

export default ComponentControlMenuContainer;
