using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HeadlineHub.Application.Common.Extensions;

public static class DecoratorExtensions
{
    public static void Decorate<TInterface, TDecorator>(this IServiceCollection services)
        where TInterface : class
        where TDecorator : class, TInterface
    {
        var wrappedDescriptor = services.FirstOrDefault(
            s => s.ServiceType == typeof(TInterface));

        if (wrappedDescriptor == null)
        {
            throw new InvalidOperationException($"{typeof(TInterface).Name} is not registered");
        }

        var objectFactory = ActivatorUtilities.CreateFactory(
            typeof(TDecorator),
            new[] { typeof(TInterface) });

        services.Replace(ServiceDescriptor.Describe(
            typeof(TInterface),
            s => (TInterface)objectFactory(s, new[] { s.CreateInstance(wrappedDescriptor) }),
            wrappedDescriptor.Lifetime)
        );
    }

    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        return descriptor.ImplementationFactory != null
            ? descriptor.ImplementationFactory(services)
            : ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType!);
    }
}