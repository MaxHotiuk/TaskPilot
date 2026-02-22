-- ========================================
-- INVITATION SYSTEM
-- ========================================

-- ====================
-- BOARD INVITATIONS
-- ====================
CREATE TABLE [BoardInvitations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [BoardId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [InvitedBy] UNIQUEIDENTIFIER NOT NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Member',
    [Status] INT NOT NULL DEFAULT 0, -- 0 = Pending, 1 = Accepted, 2 = Rejected
    [RespondedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [FK_BoardInvitations_Board] 
        FOREIGN KEY ([BoardId]) 
        REFERENCES [Boards] ([Id]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_BoardInvitations_User] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION,

    CONSTRAINT [FK_BoardInvitations_Inviter] 
        FOREIGN KEY ([InvitedBy]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION
);

-- Create indexes for better query performance
CREATE INDEX [IX_BoardInvitations_BoardId_UserId_Status] 
    ON [BoardInvitations] ([BoardId], [UserId], [Status]);

CREATE INDEX [IX_BoardInvitations_UserId_Status] 
    ON [BoardInvitations] ([UserId], [Status]);

CREATE INDEX [IX_BoardInvitations_Status] 
    ON [BoardInvitations] ([Status]);

-- ====================
-- ORGANIZATION INVITATIONS
-- ====================
CREATE TABLE [OrganizationInvitations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [InvitedBy] UNIQUEIDENTIFIER NOT NULL,
    [Role] INT NOT NULL DEFAULT 0, -- 0 = Guest, 1 = Member, 2 = Manager
    [Status] INT NOT NULL DEFAULT 0, -- 0 = Pending, 1 = Accepted, 2 = Rejected
    [RespondedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [FK_OrganizationInvitations_Organization] 
        FOREIGN KEY ([OrganizationId]) 
        REFERENCES [Organizations] ([Id]) 
        ON DELETE CASCADE,

    CONSTRAINT [FK_OrganizationInvitations_User] 
        FOREIGN KEY ([UserId]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION,

    CONSTRAINT [FK_OrganizationInvitations_Inviter] 
        FOREIGN KEY ([InvitedBy]) 
        REFERENCES [Users] ([Id]) 
        ON DELETE NO ACTION
);

-- Create indexes for better query performance
CREATE INDEX [IX_OrganizationInvitations_OrganizationId_UserId_Status] 
    ON [OrganizationInvitations] ([OrganizationId], [UserId], [Status]);

CREATE INDEX [IX_OrganizationInvitations_UserId_Status] 
    ON [OrganizationInvitations] ([UserId], [Status]);

CREATE INDEX [IX_OrganizationInvitations_Status] 
    ON [OrganizationInvitations] ([Status]);

-- ====================
-- MIGRATION NOTES
-- ====================
-- This migration adds:
-- 1. BoardInvitations table for tracking board membership invitations
--    - Users receive email invitations when added to boards
--    - Must accept/reject before becoming board members
--    - Status: Pending (0), Accepted (1), Rejected (2)
--
-- 2. OrganizationInvitations table for tracking organization membership invitations
--    - Users receive email invitations when added as guests to organizations
--    - Must accept/reject before becoming organization members
--    - Role: Guest (0), Member (1), Manager (2)
--    - Status: Pending (0), Accepted (1), Rejected (2)
--
-- 3. Proper foreign keys with cascading deletes for board/organization cleanup
--    - Prevents delete on user tables to maintain audit trail
--
-- 4. Composite indexes for efficient querying:
--    - Lookup pending invitations by user
--    - Check for duplicate invitations
--    - Query invitations by status
--
-- Behavior Changes:
-- - POST /api/boards/{boardId}/members - Now creates invitation + sends email
-- - POST /api/organizations/{organizationId}/guests - Now creates invitation + sends email
--
-- New Endpoints:
-- - GET /api/invitations/pending - Get all pending invitations
-- - POST /api/invitations/boards/{invitationId}/accept - Accept board invitation
-- - POST /api/invitations/boards/{invitationId}/reject - Reject board invitation
-- - POST /api/invitations/organizations/{invitationId}/accept - Accept organization invitation
-- - POST /api/invitations/organizations/{invitationId}/reject - Reject organization invitation
