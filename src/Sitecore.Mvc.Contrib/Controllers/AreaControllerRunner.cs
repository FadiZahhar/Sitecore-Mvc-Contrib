﻿using Sitecore.Mvc.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Mvc.Contrib.Controllers
{
    public class AreaControllerRunner : ControllerRunner, IControllerRunner
    {
        private readonly IPageContext _pageContext;
        private readonly IRouteData _routeData;
        private readonly IViewContextProvider _viewContextProvider;

        public AreaControllerRunner(IPageContext pageContext, IRouteData routeData, IViewContextProvider viewContextProvider, AreaRouteData areaRouteData)
            : base(areaRouteData.Controller, areaRouteData.Action)
        {
            _pageContext = pageContext;
            _routeData = routeData;
            _viewContextProvider = viewContextProvider;
            Area = areaRouteData.Area;
            UseChildActionBehaviour = areaRouteData.UseChildActionBehaviour;
        }

        public AreaControllerRunner(AreaRouteData areaRouteData) : this(
            new PageContextWrapper(PageContext.Current), 
            new RouteDataWrapper(PageContext.Current.RequestContext.RouteData), 
            new ViewContextProvider(),
            areaRouteData)
        {

        }

        public string Area { get; set; }
        public bool UseChildActionBehaviour { get; set; }

        protected override void ExecuteController(Controller controller)
        {
            RequestContext requestContext = _pageContext.RequestContext;
            object controllerValue = _routeData.Values["controller"];
            object actionValue = _routeData.Values["action"];
            object areaValue = _routeData.DataTokens["area"];
            object parentActionViewContext = _routeData.DataTokens["ParentActionViewContext"];
        
            try
            {
                _routeData.Values["controller"] = ActualControllerName;
                _routeData.Values["action"] = ActionName;
                _routeData.DataTokens["area"] = Area;

                if (UseChildActionBehaviour)
                {
                    _routeData.DataTokens["ParentActionViewContext"] = _viewContextProvider.GetCurrentViewContext();
                }

                ((IController)controller).Execute(_pageContext.RequestContext);

            }
            finally
            {
                _routeData.Values["controller"] = controllerValue;
                _routeData.Values["action"] = actionValue;
                _routeData.DataTokens["area"] = areaValue;

                if (UseChildActionBehaviour)
                {
                    if (parentActionViewContext == null)
                        _routeData.DataTokens.Remove("ParentActionViewContext");
                    else
                        _routeData.DataTokens["ParentActionViewContext"] = parentActionViewContext;
                }
            }
        }
    }
}
