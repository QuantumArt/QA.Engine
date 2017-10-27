import MutationSummary from 'mutation-summary';
import _ from 'lodash';
import { loadedComponentTree } from './actions/componentTreeActions';
import buildTree from './componentTreeBuilder';

function MutationWatcher(store) {
  const mutationCallback = (summaries) => {
    console.log('mutation callback fired', summaries);
    const newComponentTree = buildTree();
    store.dispatch(loadedComponentTree(newComponentTree));
  };

  new MutationSummary({
    callback: _.debounce(mutationCallback, 1000),
    queries: [{
      element: '[data-qa-component-type]',
    }],
  });
}

export default MutationWatcher;

