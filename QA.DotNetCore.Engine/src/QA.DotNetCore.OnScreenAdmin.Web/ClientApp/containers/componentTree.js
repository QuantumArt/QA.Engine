import { connect } from 'react-redux';
import {
  toggleComponent,
  toggleSubtree,
  toggleFullSubtree,
  editWidget,
  addWidgetToZone,
} from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';
import buildTree from '../utils/buildTree';

const mapStateToProps = state => ({
  components: buildTree(state.componentTree.components),
  selectedComponentId: state.componentTree.selectedComponentId,
  showAllZones: state.sidebar.showAllZones,
  showAvailableWidgets: state.componentTree.showAvailableWidgets,
});

const mapDispatchToProps = dispatch => ({
  onToggleComponent: (id) => {
    dispatch(toggleComponent(id));
  },
  onToggleSubtree: (id) => {
    dispatch(toggleSubtree(id));
  },
  onToggleFullSubtree: (id) => {
    dispatch(toggleFullSubtree(id));
  },
  onEditWidget: (id) => {
    dispatch(editWidget(id));
  },
  onAddWidgetToZone: (id) => {
    dispatch(addWidgetToZone(id));
  },
});

const ComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTree);

export default ComponentTreeContainer;
