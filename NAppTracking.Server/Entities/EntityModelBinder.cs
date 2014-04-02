namespace NAppTracking.Server.Entities
{
    using System;
    using System.Web.Mvc;

    public class EntityModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            // Get the EF context using an IoC container
            var modelContext = DependencyResolver.Current.GetService<EntitiesContext>();

            var set = modelContext.Set(modelType);

            if (set != null)
            {
                // create fails if modelType is not part of our modelContext
                // so just catch the exception and delegate work to the default model binder
                try
                {
                    return set.Create(modelType);
                }                
                catch (InvalidOperationException)
                {
                }
            }

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}