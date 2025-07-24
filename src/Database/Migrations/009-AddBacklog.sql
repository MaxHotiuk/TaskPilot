-- ========================================
-- BACKLOG TABLE FOR BOARD ACTIVITY TRACKING
-- ========================================

-- ====================
-- BACKLOG (Activity/Audit Log)
-- ====================
CREATE TABLE [BackLog] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(500) NOT NULL, -- Description of the action performed
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Foreign key constraints
    CONSTRAINT [FK_BackLog_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE,
);
