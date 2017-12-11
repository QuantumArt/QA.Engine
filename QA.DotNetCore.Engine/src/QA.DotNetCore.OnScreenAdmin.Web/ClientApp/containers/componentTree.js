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
import buildTree from '../utils/buildTree';

const filterAvailableWidgets = (widgets, searchText) => {
  const lowerSearchText = _.toLower(searchText);

  return _.filter(widgets, w =>
    _.includes(_.toLower(w.title), lowerSearchText) || _.includes(_.toLower(w.description), lowerSearchText),
  );
};

const mapStateToProps = state => ({
  components: buildTree(state.componentTree.components),
  maxNestLevel: state.componentTree.maxNestLevel,
  availableWidgets: filterAvailableWidgets(state.metaInfo.availableWidgets, state.sidebar.widgetScreenSearchText),
  selectedComponentId: state.componentTree.selectedComponentId,
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
