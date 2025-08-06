CREATE TABLE [UserProfiles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL UNIQUE,
    
    -- Basic Profile Information
    [Bio] NVARCHAR(500) NULL,
    [JobTitle] NVARCHAR(100) NULL,
    [Department] NVARCHAR(100) NULL,
    [Location] NVARCHAR(100) NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    
    -- Privacy Settings
    [AddToBoardAutomatically] BIT NOT NULL DEFAULT 0,
    [ShowEmail] BIT NOT NULL DEFAULT 0,
    [ShowPhoneNumber] BIT NOT NULL DEFAULT 0,
    
    -- Metadata
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Constraints
    CONSTRAINT [FK_UserProfiles_User] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- Indexes for UserProfiles table
CREATE INDEX [IX_UserProfiles_UserId] ON [UserProfiles] ([UserId]);
CREATE INDEX [IX_UserProfiles_Department] ON [UserProfiles] ([Department]);
CREATE INDEX [IX_UserProfiles_Location] ON [UserProfiles] ([Location]);
CREATE INDEX [IX_UserProfiles_CreatedAt] ON [UserProfiles] ([CreatedAt]);
CREATE INDEX [IX_UserProfiles_UpdatedAt] ON [UserProfiles] ([UpdatedAt]);