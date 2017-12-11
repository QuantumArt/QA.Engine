import { connect } from 'react-redux';
import _ from 'lodash';
import {
  editWidget,
  addWidgetToZone,
} from '../actions/componentControlMenuActions';

import ComponentControlMenu from '../Components/ComponentControlMenu';

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
});

const ComponentControlMenuContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentControlMenu);

export default ComponentControlMenuContainer;
