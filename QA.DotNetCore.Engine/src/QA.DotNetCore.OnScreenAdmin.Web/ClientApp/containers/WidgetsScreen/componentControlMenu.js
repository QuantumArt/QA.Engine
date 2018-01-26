import { connect } from 'react-redux';
import _ from 'lodash';
import {
  editWidget,
  // addWidgetToZone,
  moveWidget,
} from 'actions/componentControlMenuActions';
import { WIDGET_CREATION_MODE } from 'constants/widgetCreation';
import { beginWidgetCreation } from 'actions/widgetCreation/actions';
import ComponentControlMenu from 'Components/WidgetsScreen/ComponentTreeScreen/ComponentControlMenu';

const mapStateToProps = (state, ownProps) => {
  const component = _.find(state.componentTree.components, { onScreenId: ownProps.onScreenId });
  const type = (component == null) ? '' : component.type;
  const zoneName = (type === 'zone') ? component.properties.zoneName : '';
  return {
    type,
    zoneName,
  };
};

const mapDispatchToProps = dispatch => ({
  onEditWidget: (id) => {
    dispatch(editWidget(id));
  },
  onAddWidgetToZone: (onScreenId, zoneName) => {
    const payload = {
      creationMode: WIDGET_CREATION_MODE.SPECIFIC_ZONE,
      parentOnScreenId: onScreenId,
      targetZoneName: zoneName,
    };
    dispatch(beginWidgetCreation(payload));
  },
  onAddChildWidget: (onScreenId) => {
    const payload = {
      creationMode: WIDGET_CREATION_MODE.WIDGET_CHILD,
      parentOnScreenId: onScreenId,
    };
    dispatch(beginWidgetCreation(payload));
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
