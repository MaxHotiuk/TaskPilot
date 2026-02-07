-- ========================================
-- ORGANIZATIONS FEATURE
-- ========================================

-- ====================
-- ORGANIZATIONS
-- ====================
CREATE TABLE [Organizations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Domain] NVARCHAR(255) NOT NULL UNIQUE,
    [Name] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ====================
-- ORGANIZATION MEMBERS
-- ====================
CREATE TABLE [OrganizationMembers] (
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [IsInvited] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([OrganizationId], [UserId]),
    CONSTRAINT [FK_OrganizationMembers_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrganizationMembers_User] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX [IX_OrganizationMembers_UserId] ON [OrganizationMembers] ([UserId]);
CREATE INDEX [IX_OrganizationMembers_OrganizationId] ON [OrganizationMembers] ([OrganizationId]);
