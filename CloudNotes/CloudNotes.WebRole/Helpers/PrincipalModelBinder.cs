using System;
using System.Web.Mvc;

namespace CloudNotes.WebRole.Helpers
{
    /// <summary>
    /// Custom Binder for binding the current logged user to a IPrincipal parameter in the actions
    /// </summary>
    public class PrincipalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            return controllerContext.HttpContext.User;
        }
    }
}