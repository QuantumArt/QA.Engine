import React from 'react';
import PropTypes from 'prop-types';
import SearchToolBar from '../../SearchToolBar';

const AvailableWidgetsSearch = ({ searchText, changeSearchText }) => (
  <SearchToolBar
    searchText={searchText}
    changeSearchText={changeSearchText}
  />
);

AvailableWidgetsSearch.propTypes = {
  searchText: PropTypes.string.isRequired,
  changeSearchText: PropTypes.func.isRequired,
};

export default AvailableWidgetsSearch;
