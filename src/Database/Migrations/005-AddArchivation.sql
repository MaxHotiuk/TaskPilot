-- ========================================
-- Add archivation support to TaskPilot database
-- ========================================

-- ====================
-- ADD ARCHIVATION FIELDS TO BOARDS TABLE
-- ====================

-- Add archivation fields to existing Boards table
ALTER TABLE [Boards] ADD 
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [ArchivedAt] DATETIME2 NULL,
    [ArchivalReason] NVARCHAR(500) NULL;
GO

-- Ensure archived boards have required fields
ALTER TABLE [Boards] ADD 
    CONSTRAINT [CK_Boards_Archived_Fields] 
    CHECK (
        ([IsArchived] = 0) OR 
        ([IsArchived] = 1 AND [ArchivedAt] IS NOT NULL)
    );
GO
