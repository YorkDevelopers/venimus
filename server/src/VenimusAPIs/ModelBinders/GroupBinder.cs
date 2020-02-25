using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VenimusAPIs.Mongo;

namespace VenimusAPIs.ModelBinders
{
    public class GroupBinder : IModelBinder
    {
        private readonly GroupStore _groupStore;

        public GroupBinder(GroupStore groupStore)
        {
            _groupStore = groupStore;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var groupSlug = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(groupSlug))
            {
                return;
            }

            var model = await _groupStore.RetrieveGroupBySlug(groupSlug).ConfigureAwait(false);
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
