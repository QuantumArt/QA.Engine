import { connect } from 'react-redux';
import { getComponentTree, getMaxNestLevel, getSelectedComponentId } from '../selectors/componentTree';
import { getShowAllZones, getShowAllWidgets } from '../selectors/componentsHighlight';
import { getSidebarSide } from '../selectors/sidebar';
import EditComponentTree from '../Components/EditComponentTree';
import onScreenToggleComponent from '../actions/editComponentTreeActions';

const mapStateToProps = state => ({
  components: getComponentTree(state),
  showAllZones: getShowAllZones(state),
  showAllWidgets: getShowAllWidgets(state),
  maxNestLevel: getMaxNestLevel(state),
  selectedComponentId: getSelectedComponentId(state),
  side: getSidebarSide(state),
});

const mapDispatchToProps = dispatch => ({
  toggleComponent: (onScreenId) => {
    dispatch(onScreenToggleComponent(onScreenId));
  },
});

const EditComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(EditComponentTree);

export default EditComponentTreeContainer;
