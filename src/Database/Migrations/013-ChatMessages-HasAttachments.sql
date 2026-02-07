-- ========================================
-- CHAT MESSAGES ATTACHMENT FLAG
-- ========================================

IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE Name = N'HasAttachments'
      AND Object_ID = Object_ID(N'[ChatMessages]')
)
BEGIN
    ALTER TABLE [ChatMessages]
        ADD [HasAttachments] BIT NOT NULL CONSTRAINT [DF_ChatMessages_HasAttachments] DEFAULT 0;
END
