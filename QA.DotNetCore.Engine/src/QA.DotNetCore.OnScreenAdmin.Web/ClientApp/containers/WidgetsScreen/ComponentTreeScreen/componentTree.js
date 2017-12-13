import { connect } from 'react-redux';
import {
  toggleComponent,
  toggleSubtree,
  toggleFullSubtree,
} from '../../../actions/componentTreeActions';
import ComponentTree from '../../../Components/WidgetsScreen/ComponentTreeScreen/ComponentTree';
import { getMaxNestLevel, getSelectedComponentId, filteredComponentTree } from '../../../selectors/componentTree';

const mapStateToProps = state => ({
  components: filteredComponentTree(state),
  maxNestLevel: getMaxNestLevel(state),
  selectedComponentId: getSelectedComponentId(state),
  searchText: state.sidebar.widgetScreenSearchText,
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
});

const ComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTree);

export default ComponentTreeContainer;
