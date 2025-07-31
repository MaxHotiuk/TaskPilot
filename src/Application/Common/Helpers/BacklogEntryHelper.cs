using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Application.Common.Helpers;

public static class BacklogEntryHelper
{
    public static Backlog CreateBacklogForBoardChange(Board oldBoard, Board newBoard)
    {
        var changes = new List<string>();

        if (oldBoard.Name != newBoard.Name)
        {
            changes.Add($"renamed from '{oldBoard.Name}' to '{newBoard.Name}'");
        }
        if (oldBoard.Description != newBoard.Description)
        {
            changes.Add("description updated");
        }

        string description;
        if (changes.Count > 0)
        {
            description = $"Board '{newBoard.Name}' was " + string.Join(", ", changes) + ".";
        }
        else
        {
            description = $"Board '{newBoard.Name}' was updated.";
        }
        return new Backlog
        {
            BoardId = newBoard.Id,
            Description = description
        };
    }

    public static Backlog CreateBacklogForTaskCreate(TaskItem newTask, IStateRepository stateRepository, IUserRepository userRepository)
    {
        var changes = new List<string>();

        var stateName = stateRepository.GetByIdAsync(newTask.StateId).Result?.Name ?? newTask.StateId.ToString();
        changes.Add($"created in state '{stateName}'");

        if (newTask.Title != null)
        {
            changes.Add($"titled '{newTask.Title}'");
        }
        if (newTask.AssigneeId != null)
        {
            var assigneeName = userRepository.GetByIdAsync(newTask.AssigneeId.Value).Result?.Username ?? "Unassigned";
            changes.Add($"assigned to '{assigneeName}'");
        }

        string description;
        if (changes.Count > 0)
        {
            description = $"Task '{newTask.Title}' was created with " + string.Join(", ", changes) + ".";
        }
        else
        {
            description = $"Task '{newTask.Title}' was created.";
        }
        return new Backlog
        {
            BoardId = newTask.BoardId,
            Description = description
        };
    }

    public static Backlog CreateBacklogForTaskChange(TaskItem oldTask, TaskItem newTask, IStateRepository stateRepository, IUserRepository userRepository)
    {
        var changes = new List<string>();

        if (oldTask.StateId != newTask.StateId)
        {
            var prevState = stateRepository.GetByIdAsync(oldTask.StateId).Result?.Name ?? oldTask.StateId.ToString();
            var newState = stateRepository.GetByIdAsync(newTask.StateId).Result?.Name ?? newTask.StateId.ToString();
            changes.Add($"moved from '{prevState}' to '{newState}'");
        }
        if (oldTask.Title != newTask.Title)
        {
            changes.Add($"renamed from '{oldTask.Title}' to '{newTask.Title}'");
        }
        if (oldTask.AssigneeId != newTask.AssigneeId)
        {
            if (oldTask.AssigneeId == null)
            {
                if (newTask.AssigneeId != null)
                {
                    var newAssignee = userRepository.GetByIdAsync(newTask.AssigneeId.Value).Result?.Username ?? "Unassigned";
                    changes.Add($"assigned to '{newAssignee}'");
                }
            }
            else
            {
                if (newTask.AssigneeId == null)
                {
                    var prevAssignee = userRepository.GetByIdAsync(oldTask.AssigneeId.Value).Result?.Username ?? "Unassigned";
                    changes.Add($"unassigned from '{prevAssignee}'");
                }
                else
                {
                    var prevAssignee = userRepository.GetByIdAsync(oldTask.AssigneeId.Value).Result?.Username ?? "Unassigned";
                    var newAssignee = userRepository.GetByIdAsync(newTask.AssigneeId.Value).Result?.Username ?? "Unassigned";
                    changes.Add($"changed assignee from '{prevAssignee}' to '{newAssignee}'");
                }
            }
        }
        string description;
        if (changes.Count > 0)
        {
            description = $"Task '{newTask.Title}' was " + string.Join(", ", changes) + ".";
        }
        else
        {
            description = $"Task '{newTask.Title}' was updated.";
        }
        return new Backlog
        {
            BoardId = newTask.BoardId,
            Description = description
        };
    }
}
