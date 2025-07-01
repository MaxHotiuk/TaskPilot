-- ========================================
-- SQL SERVER DATABASE SCHEMA
-- ========================================

-- ====================
-- USERS
-- ====================
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EntraId] NVARCHAR(100) NOT NULL UNIQUE,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(150) NOT NULL UNIQUE,
    [Role] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ====================
-- BOARDS
-- ====================
CREATE TABLE [Boards] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX),
    [OwnerId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Boards_Owner] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

-- ====================
-- STATES (Lookup table)
-- ====================
CREATE TABLE [States] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL UNIQUE
);

-- ====================
-- TASKS
-- ====================
CREATE TABLE [Tasks] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX),
    [StateId] INT NOT NULL,
    [AssigneeId] UNIQUEIDENTIFIER NULL, -- nullable if unassigned
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DueDate] DATETIME2 NULL,
    CONSTRAINT [FK_Tasks_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tasks_State] FOREIGN KEY ([StateId]) REFERENCES [States] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tasks_Assignee] FOREIGN KEY ([AssigneeId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);

-- ====================
-- COMMENTS
-- ====================
CREATE TABLE [Comments] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TaskId] UNIQUEIDENTIFIER NOT NULL,
    [AuthorId] UNIQUEIDENTIFIER NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Comments_Task] FOREIGN KEY ([TaskId]) REFERENCES [Tasks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comments_Author] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

-- ====================
-- BOARD MEMBERS (Multi-user boards)
-- ====================
CREATE TABLE [BoardMembers] (
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Member', -- e.g., Admin, Member
    [JoinedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([BoardId], [UserId]),
    CONSTRAINT [FK_BoardMembers_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BoardMembers_User] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);