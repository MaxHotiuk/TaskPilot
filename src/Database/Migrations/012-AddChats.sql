-- ========================================
-- CHATS FEATURE
-- ========================================

-- ====================
-- CHATS
-- ====================
CREATE TABLE [Chats] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NULL,
    [Type] INT NOT NULL,
    [CreatedById] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Chats_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Chats_CreatedByUser] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Chats_OrganizationId] ON [Chats] ([OrganizationId]);
CREATE INDEX [IX_Chats_Type] ON [Chats] ([Type]);
CREATE INDEX [IX_Chats_CreatedById] ON [Chats] ([CreatedById]);

-- ====================
-- CHAT MEMBERS
-- ====================
CREATE TABLE [ChatMembers] (
    [ChatId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Role] INT NOT NULL,
    [LastReadAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([ChatId], [UserId]),
    CONSTRAINT [FK_ChatMembers_Chat] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatMembers_User] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ChatMembers_UserId] ON [ChatMembers] ([UserId]);
CREATE INDEX [IX_ChatMembers_ChatId] ON [ChatMembers] ([ChatId]);

-- ====================
-- CHAT MESSAGES
-- ====================
CREATE TABLE [ChatMessages] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ChatId] UNIQUEIDENTIFIER NOT NULL,
    [SenderId] UNIQUEIDENTIFIER NOT NULL,
    [Content] NVARCHAR(2000) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ChatMessages_Chat] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatMessages_User] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_ChatMessages_ChatId] ON [ChatMessages] ([ChatId]);
CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);
