-- ========================================
-- CHAT MESSAGES ASSIGNEE LINK
-- ========================================

IF COL_LENGTH('ChatMessages', 'AssigneeId') IS NULL
BEGIN
    ALTER TABLE [ChatMessages]
        ADD [AssigneeId] UNIQUEIDENTIFIER NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ChatMessages_Assignee'
)
BEGIN
    ALTER TABLE [ChatMessages]
        ADD CONSTRAINT [FK_ChatMessages_Assignee] FOREIGN KEY ([AssigneeId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ChatMessages_AssigneeId'
        AND object_id = OBJECT_ID('ChatMessages')
)
BEGIN
    CREATE INDEX [IX_ChatMessages_AssigneeId] ON [ChatMessages] ([AssigneeId]);
END
