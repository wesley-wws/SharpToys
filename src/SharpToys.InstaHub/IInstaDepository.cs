namespace SharpToys.InstaHub;

public interface IInstaDepository
{
    Task<TInstance?> GetAsync<TInstance>(string key) where TInstance : class;

    Task<TInstance> GetRequiredAsync<TInstance>(string key) where TInstance : class;

    //Task<IReadOnlyDictionary<string, TInstance>> GetAsync();
}

