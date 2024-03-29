using Microsoft.AspNetCore.Mvc.Filters;

namespace OrderApi.Filters
{
    public class CustomActionFilter : ActionFilterAttribute
    {
      
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("--------------------------CAF-------------------------");
        }
    }
    }
