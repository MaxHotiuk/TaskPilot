-- ========================================
-- CHAT MESSAGES MESSAGE TYPE
-- ========================================

IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE Name = N'MessageType'
      AND Object_ID = Object_ID(N'[ChatMessages]')
)
BEGIN
    ALTER TABLE [ChatMessages]
        ADD [MessageType] NVARCHAR(20) NOT NULL CONSTRAINT [DF_ChatMessages_MessageType] DEFAULT 'Text';
END
