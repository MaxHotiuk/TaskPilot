-- ========================================
-- CHAT MESSAGES TASK LINK
-- ========================================

IF COL_LENGTH('ChatMessages', 'TaskId') IS NULL
BEGIN
    ALTER TABLE [ChatMessages]
        ADD [TaskId] UNIQUEIDENTIFIER NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ChatMessages_Task'
)
BEGIN
    ALTER TABLE [ChatMessages]
        ADD CONSTRAINT [FK_ChatMessages_Task] FOREIGN KEY ([TaskId]) REFERENCES [Tasks] ([Id]) ON DELETE NO ACTION;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ChatMessages_TaskId'
        AND object_id = OBJECT_ID('ChatMessages')
)
BEGIN
    CREATE INDEX [IX_ChatMessages_TaskId] ON [ChatMessages] ([TaskId]);
END
