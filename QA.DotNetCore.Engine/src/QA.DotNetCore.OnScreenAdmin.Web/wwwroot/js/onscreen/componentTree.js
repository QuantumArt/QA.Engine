  var QA = QA || {};
  QA.OnScreen = QA.OnScreen || {};
  QA.OnScreen.ComponentTree = QA.OnScreen.ComponentTree || (function(_) {
  
  var _tree = null;
  var _observer = null;
  var _initialized = false;
  

  var init = function(){
    if(!_initialized) {
      console.log('initializing');
      buildTree();
      _observer = new MutationSummary({
        callback: _.throttle(mutationCallBack, 1000),
        queries: [{
          element: '[data-qa-component-type]'
        }]
      });
    }
  };

  var mutationCallBack = function(summaries) {
    console.log('mutation callback fired', summaries);
    buildTree();
  }

  var buildTree = function(){
    console.log('build tree fired');
    _tree = {
      components: []
    };

    var allComponents = document.querySelectorAll("[data-qa-component-type]");
    var counter = 0;
    _.forEach(allComponents, function(component){
      component.dataset.qaComponentOnScreenId = counter++;
    });
  
    _.forEach(allComponents, function(component){
        component.dataset.qaComponentParentOnScreenId = findParentComponent(component);
      }
    );
  
    var components = _.map(allComponents, mapComponent);
    var rootComponents = _.filter(components, function(c){return c.parentOnScreenId == null});
    _.forEach(rootComponents, function(component){
      component.childComponents = findChildComponents(component, components);
      _tree.components.push(component);
    });
  }

  var getTree = function(){
    return _.cloneDeep(_tree);
  }
    
  var findParentComponent = function(component) {
    var currentElement = component;
    while(currentElement && currentElement.parentNode){
      currentElement = currentElement.parentNode;
      if(currentElement.dataset && currentElement.dataset.qaComponentType){
        return currentElement.dataset.qaComponentOnScreenId;
      }
    }
    return -1;
  }

  var mapComponent = function(domElement){
    return {
      type: domElement.dataset.qaComponentType,
      onScreenId: domElement.dataset.qaComponentOnScreenId,
      parentOnScreenId: domElement.dataset.qaComponentParentOnScreenId == -1 ? null : domElement.dataset.qaComponentParentOnScreenId,
      childComponents: [],
      properties: mapComponentProperties(domElement)
    }
  }

  var mapComponentProperties = function(domElement){
    var data = domElement.dataset;
    if(data.qaComponentType == 'zone'){
      return {
        zoneName: data.qaZoneName,
        isRecursive: data.qaZoneIsRecursive,
        isGlobal: data.qaZoneIsGlobal
      };
    }
    else if(data.qaComponentType == 'widget'){
      return {
        widgetId : data.qaWidgetId,
        alias: data.qaWidgetAlias,
        title: data.qaWidgetTitle
      }
    }

    return null;
  }

  var findChildComponents = function(currentComponent, allComponents){
    var directChildren = _.filter(allComponents, function(c){
      return c.parentOnScreenId == currentComponent.onScreenId;
    });
    _.forEach(directChildren, function(child){
      child.childComponents = findChildComponents(child, allComponents);
    });

    return directChildren;
  };
  

  return {
    buildTree : buildTree,
    getTree: getTree, 
    init: init
  }


})(_);

