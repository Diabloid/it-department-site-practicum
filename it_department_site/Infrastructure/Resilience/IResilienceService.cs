namespace it_department_site.Infrastructure.Resilience
{
    public interface IResilienceService
    {
        Task<T> ExecuteWithResilienceAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
    }
}
