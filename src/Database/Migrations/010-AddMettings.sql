-- ========================================
-- MEETINGS TABLES
-- ========================================

-- ====================
-- MEETINGS
-- ====================
CREATE TABLE [Meetings] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Link] NVARCHAR(500) NULL, -- Meeting link (Teams, Zoom, etc.)
    [Description] NVARCHAR(MAX) NULL,
    [ScheduledAt] DATETIME2 NULL, -- When the meeting is scheduled
    [Duration] INT NULL, -- Duration in minutes
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Scheduled', -- Scheduled, InProgress, Completed, Cancelled
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL, -- Who created the meeting
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Meetings_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Meetings_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [CHK_Meetings_Status] CHECK ([Status] IN ('Scheduled', 'InProgress', 'Completed', 'Cancelled'))
);

-- ====================
-- MEETING MEMBERS
-- ====================
CREATE TABLE [MeetingMembers] (
    [MeetingId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Invited', -- Invited, Accepted, Declined, Tentative, Attended
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([MeetingId], [UserId]),
    CONSTRAINT [FK_MeetingMembers_Meeting] FOREIGN KEY ([MeetingId]) REFERENCES [Meetings] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MeetingMembers_User] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [CHK_MeetingMembers_Status] CHECK ([Status] IN ('Invited', 'Accepted', 'Declined', 'Tentative', 'Attended'))
);

-- ====================
-- INDEXES FOR PERFORMANCE
-- ====================
CREATE NONCLUSTERED INDEX [IX_Meetings_BoardId] ON [Meetings] ([BoardId]);
CREATE NONCLUSTERED INDEX [IX_Meetings_ScheduledAt] ON [Meetings] ([ScheduledAt]);
CREATE NONCLUSTERED INDEX [IX_Meetings_Status] ON [Meetings] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Meetings_CreatedBy] ON [Meetings] ([CreatedBy]);

CREATE NONCLUSTERED INDEX [IX_MeetingMembers_UserId] ON [MeetingMembers] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_MeetingMembers_Status] ON [MeetingMembers] ([Status]);