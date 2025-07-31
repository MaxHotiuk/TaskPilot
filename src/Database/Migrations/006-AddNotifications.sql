-- ========================================
-- Add Notifications Table
-- ========================================

CREATE TABLE [Notifications] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Text] NVARCHAR(MAX) NOT NULL,
    [Type] INT NOT NULL CHECK ([Type] IN (0, 1, 2)),
    [BoardId] UNIQUEIDENTIFIER NULL,
    [TaskId] UNIQUEIDENTIFIER NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Notifications_User] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Notifications_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notifications_Task] FOREIGN KEY ([TaskId]) REFERENCES [Tasks]([Id]) ON DELETE CASCADE
);

-- Index for quick lookup of notifications by user
CREATE INDEX IX_Notifications_UserId ON [Notifications]([UserId]);

-- Index for unread notifications
CREATE INDEX IX_Notifications_UserId_IsRead ON [Notifications]([UserId], [IsRead]);
