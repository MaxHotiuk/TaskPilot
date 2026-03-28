-- ========================================
-- BOARD CHAT SUPPORT
-- ========================================

ALTER TABLE [Chats]
    ADD [BoardId] UNIQUEIDENTIFIER NULL;

ALTER TABLE [Chats]
    ADD CONSTRAINT [FK_Chats_Board] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE;

CREATE INDEX [IX_Chats_BoardId] ON [Chats] ([BoardId]);
