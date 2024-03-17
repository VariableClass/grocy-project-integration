using Octokit;
using Polly;
using Polly.Extensions.Http;
using VariableClass.Integration.GrocyProjects.Configs;
using VariableClass.Integration.GrocyProjects.Services;
using VariableClass.Integration.GrocyProjects.Services.Authentik;
using VariableClass.Integration.GrocyProjects.Services.GitHub;
using VariableClass.Integration.GrocyProjects.Services.Grocy;

namespace VariableClass.Integration.GrocyProjects;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<ChoreExportService>();
    
    return services;
  }

  public static IServiceCollection AddAuthentik(this IServiceCollection services, IConfiguration configuration)
  {
    var configurationSection = configuration.GetSection(AuthentikConfig.Key);
    services.Configure<AuthentikConfig>(configurationSection);

    services.AddHttpClient<IAuthenticationService, Authentik>()
      .AddPolicyHandler(GetRetryPolicy())
      .AddPolicyHandler(GetCircuitBreakerPolicy());
    
    return services;
  }

  public static IServiceCollection AddGrocy(this IServiceCollection services, IConfiguration configuration)
  {
    var configurationSection = configuration.GetSection(GrocyConfig.Key);
    services.Configure<GrocyConfig>(configurationSection);

    services.AddHttpClient<IChoreProvider, Grocy>()
      .AddPolicyHandler(GetRetryPolicy())
      .AddPolicyHandler(GetCircuitBreakerPolicy());

    return services;
  }

  public static IServiceCollection AddGitHub(this IServiceCollection services, IConfiguration configuration)
  {
    var configurationSection = configuration.GetSection(GitHubConfig.Key);
    services.Configure<GitHubConfig>(configurationSection);

    services.AddSingleton(client =>
    {
      var ghConfig = configurationSection.Get<GitHubConfig>()
        ?? throw new NullReferenceException(
          $"Configuration section: '{GitHubConfig.Key}' could not be bound to {nameof(GitHubConfig)}");
      
      return new GitHubClient(new ProductHeaderValue(ghConfig.ClientName))
      {
        Credentials = new Credentials(ghConfig.Credential)
      };
    });

    services.AddTransient<IProjectManagementTool, GitHub>();

    return services;
  }

  private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

  private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
      => HttpPolicyExtensions
          .HandleTransientHttpError()
          .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}