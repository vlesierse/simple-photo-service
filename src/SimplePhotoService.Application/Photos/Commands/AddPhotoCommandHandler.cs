using Mediator;

namespace SimplePhotoService.Application.Photos.Commands;

public class AddPhotoCommandHandler : ICommandHandler<AddPhotoCommand>
{
    public ValueTask<Unit> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}