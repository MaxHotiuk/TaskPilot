-- ========================================
-- Performance indexes for TaskPilot database
-- ========================================

-- ====================
-- USERS TABLE INDEXES
-- ====================

-- Index for EntraId lookups (authentication)
CREATE NONCLUSTERED INDEX [IX_Users_EntraId] 
ON [Users] ([EntraId]);

-- Index for email lookups (login, user search)
CREATE NONCLUSTERED INDEX [IX_Users_Email] 
ON [Users] ([Email]);

-- Index for username searches
CREATE NONCLUSTERED INDEX [IX_Users_Username] 
ON [Users] ([Username]);

-- Index for role-based queries
CREATE NONCLUSTERED INDEX [IX_Users_Role] 
ON [Users] ([Role]);

-- ====================
-- BOARDS TABLE INDEXES
-- ====================

-- Index for owner-based board queries
CREATE NONCLUSTERED INDEX [IX_Boards_OwnerId] 
ON [Boards] ([OwnerId]);

-- Index for board name searches
CREATE NONCLUSTERED INDEX [IX_Boards_Name] 
ON [Boards] ([Name]);

-- Composite index for owner's boards ordered by creation date
CREATE NONCLUSTERED INDEX [IX_Boards_Owner_CreatedAt] 
ON [Boards] ([OwnerId], [CreatedAt] DESC);

-- ====================
-- TASKS TABLE INDEXES
-- ====================

-- Index for board-specific task queries
CREATE NONCLUSTERED INDEX [IX_Tasks_BoardId] 
ON [Tasks] ([BoardId]);

-- Index for assignee-based task queries
CREATE NONCLUSTERED INDEX [IX_Tasks_AssigneeId] 
ON [Tasks] ([AssigneeId]) 
WHERE [AssigneeId] IS NOT NULL;

-- Index for state-based filtering
CREATE NONCLUSTERED INDEX [IX_Tasks_StateId] 
ON [Tasks] ([StateId]);

-- Composite index for board tasks by state
CREATE NONCLUSTERED INDEX [IX_Tasks_Board_State] 
ON [Tasks] ([BoardId], [StateId]);

-- Composite index for assignee tasks by state
CREATE NONCLUSTERED INDEX [IX_Tasks_Assignee_State] 
ON [Tasks] ([AssigneeId], [StateId]) 
WHERE [AssigneeId] IS NOT NULL;

-- Index for due date queries
CREATE NONCLUSTERED INDEX [IX_Tasks_DueDate] 
ON [Tasks] ([DueDate]) 
WHERE [DueDate] IS NOT NULL;

-- Composite index for overdue tasks
CREATE NONCLUSTERED INDEX [IX_Tasks_Board_DueDate] 
ON [Tasks] ([BoardId], [DueDate]) 
WHERE [DueDate] IS NOT NULL;

-- Index for task title searches
CREATE NONCLUSTERED INDEX [IX_Tasks_Title] 
ON [Tasks] ([Title]);

-- Composite index for board tasks ordered by creation date
CREATE NONCLUSTERED INDEX [IX_Tasks_Board_CreatedAt] 
ON [Tasks] ([BoardId], [CreatedAt] DESC);

-- ====================
-- COMMENTS TABLE INDEXES
-- ====================

-- Index for task-specific comments
CREATE NONCLUSTERED INDEX [IX_Comments_TaskId] 
ON [Comments] ([TaskId]);

-- Index for author-based comment queries
CREATE NONCLUSTERED INDEX [IX_Comments_AuthorId] 
ON [Comments] ([AuthorId]);

-- Composite index for task comments ordered by creation date
CREATE NONCLUSTERED INDEX [IX_Comments_Task_CreatedAt] 
ON [Comments] ([TaskId], [CreatedAt]);

-- Composite index for author's comments ordered by creation date
CREATE NONCLUSTERED INDEX [IX_Comments_Author_CreatedAt] 
ON [Comments] ([AuthorId], [CreatedAt] DESC);

-- ====================
-- BOARD MEMBERS TABLE INDEXES
-- ====================

-- Index for user-specific board memberships
CREATE NONCLUSTERED INDEX [IX_BoardMembers_UserId] 
ON [BoardMembers] ([UserId]);

-- Index for board member role queries
CREATE NONCLUSTERED INDEX [IX_BoardMembers_Role] 
ON [BoardMembers] ([Role]);

-- Composite index for user's boards by role
CREATE NONCLUSTERED INDEX [IX_BoardMembers_User_Role] 
ON [BoardMembers] ([UserId], [Role]);

-- Composite index for board members ordered by join date
CREATE NONCLUSTERED INDEX [IX_BoardMembers_Board_JoinedAt] 
ON [BoardMembers] ([BoardId], [JoinedAt] DESC);

-- ====================
-- STATES TABLE INDEXES
-- ====================

-- Index for board-specific state queries
CREATE NONCLUSTERED INDEX [IX_States_BoardId] 
ON [States] ([BoardId]);

-- Index for state name lookups within a board
CREATE NONCLUSTERED INDEX [IX_States_Board_Name] 
ON [States] ([BoardId], [Name]);

-- Index for state order within a board
CREATE NONCLUSTERED INDEX [IX_States_Board_Order] 
ON [States] ([BoardId], [Order]);