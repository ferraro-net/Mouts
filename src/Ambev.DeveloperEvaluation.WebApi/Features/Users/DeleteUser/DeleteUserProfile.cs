using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;

public class DeleteUserProfile : Profile
{
    public DeleteUserProfile()
    {
        CreateMap<Guid, Application.Users.DeleteUser.DeleteUserCommand>()
            .ConstructUsing(id => new Application.Users.DeleteUser.DeleteUserCommand(id));
    }
}
