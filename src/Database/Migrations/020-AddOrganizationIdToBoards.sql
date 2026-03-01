-- ========================================
-- ADD ORGANIZATION TO BOARDS
-- ========================================

-- ====================
-- ADD ORGANIZATIONID TO BOARDS TABLE
-- ====================

-- Add OrganizationId column to Boards table
ALTER TABLE [Boards]
ADD [OrganizationId] UNIQUEIDENTIFIER NULL;

GO;

-- Populate OrganizationId based on board owner's organization
-- This script assumes each board owner belongs to at least one organization
UPDATE [Boards]
SET [OrganizationId] = (
    SELECT TOP 1 [OrganizationId]
    FROM [OrganizationMembers]
    WHERE [OrganizationMembers].[UserId] = [Boards].[OwnerId]
    ORDER BY [OrganizationMembers].[CreatedAt] ASC
)
WHERE [OrganizationId] IS NULL;

GO;
-- Add foreign key constraint
ALTER TABLE [Boards]
ADD CONSTRAINT [FK_Boards_Organization]
    FOREIGN KEY ([OrganizationId])
    REFERENCES [Organizations] ([Id])
    ON DELETE NO ACTION;

GO;

-- Add index for better query performance
CREATE INDEX [IX_Boards_OrganizationId]
ON [Boards] ([OrganizationId]);

-- ====================
-- MIGRATION NOTES
-- ====================
-- This migration adds:
-- 1. OrganizationId column to Boards table
-- 2. Foreign key relationship between Boards and Organizations
-- 3. Index on OrganizationId for efficient filtering
--
-- Breaking Changes:
-- - POST /api/boards - Now requires organizationId in request body
-- - GET /api/boards/user/search - Now requires organizationId query parameter
-- - GET /api/boards/owner/search - Now requires organizationId query parameter
-- - GET /api/boards/member/search - Now requires organizationId query parameter
--
-- Business Rules:
-- - Users must select which organization they're creating a board for
-- - Board searches are now scoped to a specific organization
-- - Boards cannot be created without an organization
-- - Board owner must be a member of the selected organization
-- - Guest users cannot create boards
