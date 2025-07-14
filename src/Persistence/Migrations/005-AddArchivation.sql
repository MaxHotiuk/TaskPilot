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

-- ====================
-- CREATE ARCHIVAL JOBS TRACKING TABLE
-- ====================

-- Table to track archival job processing status
CREATE TABLE [ArchivalJobs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [JobType] NVARCHAR(50) NOT NULL DEFAULT 'BoardArchival', -- Future: TaskArchival, etc.
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed, Failed
    [StartedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CompletedAt] DATETIME2 NULL,
    [ProcessedAt] DATETIME2 NULL, -- When HangFire job picked it up
    [BlobPath] NVARCHAR(500) NULL, -- Path to archived data in blob storage
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [ProcessedBy] NVARCHAR(100) NULL, -- Service/function that processed the job
    [Metadata] NVARCHAR(MAX) NULL, -- JSON metadata about the archival process
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_ArchivalJobs_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE
);

-- ====================
-- CREATE ARCHIVAL QUEUE TABLE (Service Bus backup)
-- ====================

-- Table to track messages sent to Service Bus (for monitoring/debugging)
CREATE TABLE [ArchivalQueue] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [MessageId] NVARCHAR(100) NULL, -- Service Bus message ID
    [MessageBody] NVARCHAR(MAX) NOT NULL,
    [QueueName] NVARCHAR(100) NOT NULL DEFAULT 'project-archival-queue',
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Queued', -- Queued, Processed, Failed
    [SentAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ProcessedAt] DATETIME2 NULL,
    [RetryCount] INT NOT NULL DEFAULT 0,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_ArchivalQueue_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE
);

-- ====================
-- CREATE ARCHIVAL SETTINGS TABLE
-- ====================

-- Configuration table for archival settings
CREATE TABLE [ArchivalSettings] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SettingKey] NVARCHAR(100) NOT NULL UNIQUE,
    [SettingValue] NVARCHAR(500) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ====================
-- ADD PERFORMANCE INDEXES FOR ARCHIVATION
-- ====================

-- Index for finding archived boards
CREATE NONCLUSTERED INDEX [IX_Boards_IsArchived] 
ON [Boards] ([IsArchived]);

-- Index for finding archived boards with dates
CREATE NONCLUSTERED INDEX [IX_Boards_Archived_Date] 
ON [Boards] ([IsArchived], [ArchivedAt])
WHERE [IsArchived] = 1;

-- Index for archival jobs by status
CREATE NONCLUSTERED INDEX [IX_ArchivalJobs_Status] 
ON [ArchivalJobs] ([Status]);

-- Index for archival jobs by board
CREATE NONCLUSTERED INDEX [IX_ArchivalJobs_BoardId] 
ON [ArchivalJobs] ([BoardId]);

-- Index for pending archival jobs (for HangFire processing)
CREATE NONCLUSTERED INDEX [IX_ArchivalJobs_Pending] 
ON [ArchivalJobs] ([Status], [CreatedAt])
WHERE [Status] = 'Pending';

-- Index for archival queue status
CREATE NONCLUSTERED INDEX [IX_ArchivalQueue_Status] 
ON [ArchivalQueue] ([Status]);

-- Index for archival queue by board
CREATE NONCLUSTERED INDEX [IX_ArchivalQueue_BoardId] 
ON [ArchivalQueue] ([BoardId]);

-- Index for failed archival queue items (for retry processing)
CREATE NONCLUSTERED INDEX [IX_ArchivalQueue_Failed] 
ON [ArchivalQueue] ([Status], [RetryCount], [SentAt])
WHERE [Status] = 'Failed';

-- ====================
-- INSERT DEFAULT ARCHIVAL SETTINGS
-- ====================

-- Insert default archival configuration
INSERT INTO [ArchivalSettings] ([SettingKey], [SettingValue], [Description]) VALUES 
('ArchivalDelayDays', '7', 'Number of days to wait before processing archived boards'),
('MaxRetryAttempts', '3', 'Maximum number of retry attempts for failed archival jobs'),
('BlobStorageContainer', 'archived-boards', 'Azure Blob Storage container name for archived data'),
('ServiceBusQueueName', 'board-archival-queue', 'Service Bus queue name for archival processing'),
('ArchivalBatchSize', '10', 'Number of boards to process in each HangFire batch'),
('EnableArchivalProcessing', 'true', 'Master switch to enable/disable archival processing'),
('ArchivalRetentionDays', '2555', 'Number of days to retain archived data (7 years)'),
('CosmosDbDatabase', 'TaskPilot', 'CosmosDB database name for archival tracking'),
('CosmosDbContainer', 'ArchivalJobs', 'CosmosDB container name for archival job tracking'),
('BlobLifecycleCoolDays', '30', 'Days after which blob storage moves to cool tier'),
('BlobLifecycleArchiveDays', '90', 'Days after which blob storage moves to archive tier');

-- ====================
-- ADD CHECK CONSTRAINTS
-- ====================

-- Ensure archived boards have required fields
ALTER TABLE [Boards] ADD 
    CONSTRAINT [CK_Boards_Archived_Fields] 
    CHECK (
        ([IsArchived] = 0) OR 
        ([IsArchived] = 1 AND [ArchivedAt] IS NOT NULL)
    );

-- Ensure archival jobs have valid status
ALTER TABLE [ArchivalJobs] ADD 
    CONSTRAINT [CK_ArchivalJobs_Status] 
    CHECK ([Status] IN ('Pending', 'InProgress', 'Completed', 'Failed', 'Cancelled'));

-- Ensure archival jobs have valid job type
ALTER TABLE [ArchivalJobs] ADD 
    CONSTRAINT [CK_ArchivalJobs_JobType] 
    CHECK ([JobType] IN ('BoardArchival', 'TaskArchival', 'UserArchival'));

-- Ensure archival queue has valid status
ALTER TABLE [ArchivalQueue] ADD 
    CONSTRAINT [CK_ArchivalQueue_Status] 
    CHECK ([Status] IN ('Queued', 'Processed', 'Failed', 'Cancelled'));

-- Ensure completed jobs have completion time
ALTER TABLE [ArchivalJobs] ADD 
    CONSTRAINT [CK_ArchivalJobs_Completed] 
    CHECK (
        ([Status] NOT IN ('Completed', 'Failed', 'Cancelled')) OR 
        ([Status] IN ('Completed', 'Failed', 'Cancelled') AND [CompletedAt] IS NOT NULL)
    );