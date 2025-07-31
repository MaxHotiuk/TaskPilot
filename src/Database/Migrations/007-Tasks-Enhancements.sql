-- ========================================
-- DATABASE ENHANCEMENT SCRIPT
-- Adding Tags and Priority to Tasks
-- ========================================

-- ====================
-- CREATE TAGS TABLE
-- ====================
-- Tags are configurable per board, similar to States but without order
CREATE TABLE [Tags] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Color] NVARCHAR(7) NULL, -- Optional hex color code for UI (e.g., #FF5733)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Tags_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_Tags_Board_Name] UNIQUE ([BoardId], [Name])
);
GO
-- ====================
-- ALTER TASKS TABLE
-- ====================
-- Add TagId and Priority columns to existing Tasks table
ALTER TABLE [Tasks] 
ADD [TagId] INT NULL,
    [Priority] INT NOT NULL DEFAULT 2; -- Default to normal priority (2)
GO
-- Add foreign key constraint for TagId
ALTER TABLE [Tasks]
ADD CONSTRAINT [FK_Tasks_Tag] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE NO ACTION;
GO
-- Add check constraint for Priority values (1-4)
ALTER TABLE [Tasks]
ADD CONSTRAINT [CK_Tasks_Priority] CHECK ([Priority] IN (1, 2, 3, 4));
GO
-- ====================
-- CREATE INDEXES FOR PERFORMANCE
-- ====================
-- Index on BoardId for Tags table
CREATE NONCLUSTERED INDEX [IX_Tags_BoardId] ON [Tags] ([BoardId]);

-- Index on TagId for Tasks table
CREATE NONCLUSTERED INDEX [IX_Tasks_TagId] ON [Tasks] ([TagId]);

-- Index on Priority for Tasks table (for filtering and sorting)
CREATE NONCLUSTERED INDEX [IX_Tasks_Priority] ON [Tasks] ([Priority]);

-- Composite index for common queries (BoardId + Priority)
CREATE NONCLUSTERED INDEX [IX_Tasks_BoardId_Priority] ON [Tasks] ([BoardId], [Priority]);