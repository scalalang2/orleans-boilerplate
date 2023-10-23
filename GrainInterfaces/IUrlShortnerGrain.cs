using Orleans;

namespace GrainInterfaces;

public interface IUrlShortnerGrain : IGrainWithStringKey
{
    Task SetUrl(string fullUrl);
    Task<string> GetUrl();
}