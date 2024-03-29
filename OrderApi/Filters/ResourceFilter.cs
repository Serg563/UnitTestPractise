using Microsoft.AspNetCore.Mvc.Filters;

namespace OrderApi.Filters
{
    public class ResourceFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine("-------------------------Hello from ResourceFilter----------------------------");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // реализация отсутствует
        }
    }
}
