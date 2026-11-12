namespace TaskFlow.Application;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensiones de registro de servicios de la capa Application.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registra los servicios de la capa Application en el contenedor de DI.
    /// Incluye TimeProvider.System como implementación predeterminada para referencias
    /// temporales en UTC, reemplazable en pruebas mediante FakeTimeProvider.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este método registra únicamente los servicios propios de la capa Application.
    /// La implementación de <see cref="TaskFlow.Application.Abstractions.ITaskRepository"/>
    /// es responsabilidad de la capa Infrastructure y debe registrarse por separado
    /// mediante <c>AddInfrastructureServices()</c> (tarea 4.x).
    /// </para>
    /// <para>
    /// Orden de registro recomendado en <c>Program.cs</c>:
    /// <code>
    /// builder.Services.AddApplicationServices();
    /// builder.Services.AddInfrastructureServices(builder.Configuration);
    /// </code>
    /// </para>
    /// </remarks>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // TimeProvider: referencia temporal inyectable, determinista en pruebas.
        // FakeTimeProvider (Microsoft.Extensions.TimeProvider.Testing) se usa en Application.UnitTests.
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}

