-- ========================================
-- ORGANIZATION MANAGER ROLES & REQUESTS
-- ========================================

-- ====================
-- ADD ROLE TO ORGANIZATION MEMBERS
-- ====================
ALTER TABLE [OrganizationMembers]
ADD [Role] NVARCHAR(50) NOT NULL DEFAULT 'Member';

-- Create index for role-based queries
CREATE INDEX [IX_OrganizationMembers_Role] ON [OrganizationMembers] ([Role]);

-- Update default role constraint to support Guest, Member, Manager
-- Guest = 0, Member = 1, Manager = 2

-- ====================
-- ORGANIZATION MANAGER REQUESTS
-- ====================
CREATE TABLE [OrganizationManagerRequests] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [Message] NVARCHAR(1000) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [ReviewedBy] UNIQUEIDENTIFIER NULL,
    [ReviewedAt] DATETIME2 NULL,
    [ReviewNotes] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [FK_OrganizationManagerRequests_User] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION,

    CONSTRAINT [FK_OrganizationManagerRequests_Organization] 
        FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations] ([Id]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_OrganizationManagerRequests_Reviewer] 
        FOREIGN KEY ([ReviewedBy]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION
);

-- Create indexes for better query performance
CREATE INDEX [IX_OrganizationManagerRequests_UserId_OrganizationId_CreatedAt] 
    ON [OrganizationManagerRequests] ([UserId], [OrganizationId], [CreatedAt]);

CREATE INDEX [IX_OrganizationManagerRequests_Status] 
    ON [OrganizationManagerRequests] ([Status]);

CREATE INDEX [IX_OrganizationManagerRequests_OrganizationId] 
    ON [OrganizationManagerRequests] ([OrganizationId]);

-- ====================
-- MIGRATION NOTES
-- ====================
-- This migration adds:
-- 1. Role column to OrganizationMembers (Guest/Member/Manager)
--    - Guest (0): Limited permissions, cannot create boards/group chats, cannot become manager
--    - Member (1): Standard organization member with full collaboration rights
--    - Manager (2): Can manage organization members and permissions
-- 2. OrganizationManagerRequests table for tracking manager role requests
-- 3. Support for pending, approved, and rejected request statuses
-- 4. Proper foreign keys with cascading deletes for organization cleanup
-- 5. Indexes for efficient querying of requests and role-based lookups
