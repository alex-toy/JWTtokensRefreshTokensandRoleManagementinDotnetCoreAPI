using MovieAPI.DTO;

namespace MovieAPI.Services.AuthenticationService.Handlers
{
    public interface IHandler<Req> where Req : class
    {
        IHandler<Req> SetNext(IHandler<Req> handler);
        Task<StatusDto> HandleAsync(Req request);
    }
}
