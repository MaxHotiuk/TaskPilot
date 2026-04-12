namespace Application.Abstractions.Archivation;

public interface IArchivalBackgroundJob
{
    Task ProcessArchivedBoardsAsync();
    Task EnqueueStaleBoardsAsync();
}
