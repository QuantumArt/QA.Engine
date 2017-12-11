import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import EditPortalItem from '../EditPortalItem';

const EditPortalTree = ({
  components,
  maxNestLevel,
  selectedComponentId,
  showAllWidgets,
  showAllZones,
  side,
  toggleComponent,
}) => (
  <Fragment>
    {components.map(component => (
      <EditPortalItem
        {...component}
        maxNestLevel={maxNestLevel}
        selectedComponentId={selectedComponentId}
        key={component.onScreenId}
        showAllWidgets={showAllWidgets}
        showAllZones={showAllZones}
        side={side}
        toggleComponent={toggleComponent}
      />

    ))}
  </Fragment>
);

EditPortalTree.propTypes = {
  components: PropTypes.arrayOf(
    PropTypes.shape({
      type: PropTypes.string.isRequired,
      onScreenId: PropTypes.string.isRequired,
      isOpened: PropTypes.bool,
      properties: PropTypes.object.isRequired,
      children: PropTypes.array.isRequired,
    }).isRequired,
  ).isRequired,
  maxNestLevel: PropTypes.number.isRequired,
  selectedComponentId: PropTypes.string.isRequired,
  showAllZones: PropTypes.bool.isRequired,
  showAllWidgets: PropTypes.bool.isRequired,
  side: PropTypes.string.isRequired,
  toggleComponent: PropTypes.func.isRequired,
};

export default EditPortalTree;
