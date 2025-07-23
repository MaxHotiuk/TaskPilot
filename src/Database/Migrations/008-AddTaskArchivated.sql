-- ====================
-- ALTER TASKS TABLE
-- ====================
-- Add IsArchived column to existing Tasks table
ALTER TABLE [Tasks] 
ADD [IsArchived] BIT NOT NULL DEFAULT 0;
GO

-- ====================
-- CREATE INDEXES FOR PERFORMANCE
-- ====================
-- Index on IsArchived for quick filtering of archived tasks
CREATE NONCLUSTERED INDEX [IX_Tasks_IsArchived] ON [Tasks] ([IsArchived]);