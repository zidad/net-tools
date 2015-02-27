using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;

namespace Net.Web.Autofac
{
    public class InjectableActionFilterAttribute : ActionFilterAttribute
    {
        private readonly Type filterType;

        public InjectableActionFilterAttribute(Type filterType)
        {
            this.filterType = filterType;
        }

        private static ILifetimeScope Scope
        {
            get
            {
                return (ILifetimeScope)DependencyResolver.Current.GetService(typeof(ILifetimeScope));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (var o = Scope.BeginLifetimeScope())
            {
                var service = o.Resolve(filterType) as IActionFilter;
                if (service != null) service.OnActionExecuting(filterContext);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (var o = Scope.BeginLifetimeScope())
            {
                var service = o.Resolve(filterType) as IActionFilter;
                if (service != null) service.OnActionExecuted(filterContext);
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            using (var o = Scope.BeginLifetimeScope())
            {
                var service = o.Resolve(filterType) as IResultFilter;
                if (service != null) service.OnResultExecuted(filterContext);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            using (var o = Scope.BeginLifetimeScope())
            {
                var service = o.Resolve(filterType) as IResultFilter;
                if (service != null) service.OnResultExecuting(filterContext);
            }
        }
    }
}
