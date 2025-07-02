using System.Reflection;
using WebApi.Endpoints;

namespace WebApi.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => 
                !t.IsAbstract && 
                !t.IsInterface && 
                typeof(EndpointBase).IsAssignableFrom(t))
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            if (Activator.CreateInstance(endpointType) is EndpointBase endpoint)
            {
                endpoint.MapEndpoint(app);
            }
        }

        return app;
    }
}
