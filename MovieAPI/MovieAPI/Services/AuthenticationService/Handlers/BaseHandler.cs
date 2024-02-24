using MovieAPI.DTO;

namespace MovieAPI.Services.AuthenticationService.Handlers
{
    public abstract class BaseHandler<Req> : IHandler<Req> where Req : class
    {
        protected IHandler<Req>? _successor;

        public abstract Task<StatusDto> HandleAsync(Req request);

        public IHandler<Req> SetNext(IHandler<Req> handler)
        {
            _successor = handler;
            return _successor;
        }

        protected async Task<StatusDto> Proceed(Req request)
        {
            if (_successor is not null)
            {
                return await _successor.HandleAsync(request);
            }

            StatusDto status = new StatusDto();
            status.StatusCode = 0;
            status.Message = "could not proceed";
            return status;
        }
    }
}
