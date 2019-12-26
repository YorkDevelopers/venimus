using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YorkDeveloperEvents
{
    public abstract class BasePageModel : PageModel
    {
        protected ActionResult AddProblemsToModelState(ValidationProblemDetails validationProblemDetails, string modelName)
        {
            foreach (var item in validationProblemDetails.Errors)
            {
                foreach (var msg in item.Value)
                {
                    if (string.IsNullOrWhiteSpace(modelName))
                        ModelState.AddModelError($"{item.Key}", msg);
                    else
                        ModelState.AddModelError($"{modelName}.{item.Key}", msg);
                }
            }

            return Page();
        }
    }
}