import { connect } from 'react-redux';
import _ from 'lodash';
import {
  toggleComponent,
  toggleSubtree,
  toggleFullSubtree,
  selectWidgetToAdd,
  hideAvailableWidgets,
} from '../actions/componentTreeActions';
import ComponentTree from '../Components/ComponentTree';
import { getComponentTree, getMaxNestLevel, getSelectedComponentId } from '../selectors/componentTree';

const filterAvailableWidgets = (widgets, searchText) => {
  const lowerSearchText = _.toLower(searchText);

  return _.filter(widgets, w =>
    _.includes(_.toLower(w.title), lowerSearchText) || _.includes(_.toLower(w.description), lowerSearchText),
  );
};

const mapStateToProps = state => ({
  components: getComponentTree(state),
  maxNestLevel: getMaxNestLevel(state),
  availableWidgets: filterAvailableWidgets(state.metaInfo.availableWidgets, state.sidebar.widgetScreenSearchText),
  selectedComponentId: getSelectedComponentId(state),
  showAvailableWidgets: state.componentTree.showAvailableWidgets,
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
  onSelectWidgetToAdd: (id) => {
    dispatch(selectWidgetToAdd(id));
  },
  onCancelAddWidget: () => {
    dispatch(hideAvailableWidgets());
  },
});

const ComponentTreeContainer = connect(
  mapStateToProps,
  mapDispatchToProps,
)(ComponentTree);

export default ComponentTreeContainer;
