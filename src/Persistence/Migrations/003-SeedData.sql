-- ========================================
-- Initial seed data for TaskPilot database
-- ========================================

-- ====================
-- SEED DEMO USERS
-- ====================

-- Insert demo users for development/testing
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Users] ([Id], [EntraId], [Username], [Email], [Role]) VALUES 
(@AdminUserId, 'entra-admin-001', 'admin', 'admin@taskpilot.dev', 'Admin'),
(@User1Id, 'entra-user-001', 'john.doe', 'john.doe@taskpilot.dev', 'User'),
(@User2Id, 'entra-user-002', 'jane.smith', 'jane.smith@taskpilot.dev', 'User'),
(@User3Id, 'entra-user-003', 'bob.wilson', 'bob.wilson@taskpilot.dev', 'User');

-- ====================
-- SEED DEMO BOARDS
-- ====================

-- Insert demo boards
DECLARE @Board1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Board2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Board3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Boards] ([Id], [Name], [Description], [OwnerId]) VALUES 
(@Board1Id, 'TaskPilot Development', 'Main development board for TaskPilot project features and bug fixes', @AdminUserId),
(@Board2Id, 'Marketing Campaign Q3', 'Marketing initiatives and campaigns for Q3 2025', @User1Id),
(@Board3Id, 'Customer Support Issues', 'Track and resolve customer support tickets and feature requests', @User2Id);

-- ====================
-- SEED STATES (Per Board)
-- ====================

-- Insert default task states for each board with proper ordering
-- Board 1: TaskPilot Development
INSERT INTO [States] ([BoardId], [Name], [Order]) VALUES 
(@Board1Id, 'To Do', 1),
(@Board1Id, 'In Progress', 2),
(@Board1Id, 'Review', 3),
(@Board1Id, 'Done', 4),
(@Board1Id, 'Blocked', 5),
(@Board1Id, 'Cancelled', 6);

-- Board 2: Marketing Campaign Q3
INSERT INTO [States] ([BoardId], [Name], [Order]) VALUES 
(@Board2Id, 'To Do', 1),
(@Board2Id, 'In Progress', 2),
(@Board2Id, 'Review', 3),
(@Board2Id, 'Done', 4);

-- Board 3: Customer Support Issues
INSERT INTO [States] ([BoardId], [Name], [Order]) VALUES 
(@Board3Id, 'To Do', 1),
(@Board3Id, 'In Progress', 2),
(@Board3Id, 'Escalated', 3),
(@Board3Id, 'Resolved', 4),
(@Board3Id, 'Done', 5);

-- ====================
-- SEED BOARD MEMBERS
-- ====================

-- Add members to boards
INSERT INTO [BoardMembers] ([BoardId], [UserId], [Role]) VALUES 
-- TaskPilot Development Board
(@Board1Id, @AdminUserId, 'Admin'),
(@Board1Id, @User1Id, 'Member'),
(@Board1Id, @User2Id, 'Member'),

-- Marketing Campaign Board
(@Board2Id, @User1Id, 'Admin'),
(@Board2Id, @User3Id, 'Member'),

-- Customer Support Board
(@Board3Id, @User2Id, 'Admin'),
(@Board3Id, @AdminUserId, 'Member'),
(@Board3Id, @User3Id, 'Member');

-- ====================
-- SEED DEMO TASKS
-- ====================

-- Get state IDs for task creation (Board-specific states)
-- Board 1 States
DECLARE @Board1_ToDoStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board1Id AND [Name] = 'To Do');
DECLARE @Board1_InProgressStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board1Id AND [Name] = 'In Progress');
DECLARE @Board1_ReviewStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board1Id AND [Name] = 'Review');
DECLARE @Board1_DoneStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board1Id AND [Name] = 'Done');
DECLARE @Board1_BlockedStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board1Id AND [Name] = 'Blocked');

-- Board 2 States
DECLARE @Board2_ToDoStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board2Id AND [Name] = 'To Do');
DECLARE @Board2_InProgressStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board2Id AND [Name] = 'In Progress');
DECLARE @Board2_DoneStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board2Id AND [Name] = 'Done');

-- Board 3 States
DECLARE @Board3_ToDoStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board3Id AND [Name] = 'To Do');
DECLARE @Board3_InProgressStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board3Id AND [Name] = 'In Progress');
DECLARE @Board3_ResolvedStateId INT = (SELECT [Id] FROM [States] WHERE [BoardId] = @Board3Id AND [Name] = 'Resolved');

-- Insert demo tasks
DECLARE @Task1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task5Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task6Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task7Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task8Id UNIQUEIDENTIFIER = NEWID();

-- TaskPilot Development Board Tasks
INSERT INTO [Tasks] ([Id], [BoardId], [Title], [Description], [StateId], [AssigneeId], [DueDate]) VALUES 
(@Task1Id, @Board1Id, 'Implement User Authentication', 'Set up Azure AD integration for user authentication and authorization', @Board1_InProgressStateId, @AdminUserId, DATEADD(day, 7, GETUTCDATE())),
(@Task2Id, @Board1Id, 'Create Task Management API', 'Develop REST API endpoints for CRUD operations on tasks', @Board1_ToDoStateId, @User1Id, DATEADD(day, 14, GETUTCDATE())),
(@Task3Id, @Board1Id, 'Design Database Schema', 'Finalize database schema and create migration scripts', @Board1_DoneStateId, @User2Id, NULL),
(@Task4Id, @Board1Id, 'Fix Comment Threading Bug', 'Comments are not displaying in correct threaded order', @Board1_ReviewStateId, @User1Id, DATEADD(day, 3, GETUTCDATE()));

-- Marketing Campaign Board Tasks
INSERT INTO [Tasks] ([Id], [BoardId], [Title], [Description], [StateId], [AssigneeId], [DueDate]) VALUES 
(@Task5Id, @Board2Id, 'Social Media Content Calendar', 'Plan and schedule social media posts for Q3', @Board2_ToDoStateId, @User3Id, DATEADD(day, 10, GETUTCDATE())),
(@Task6Id, @Board2Id, 'Email Campaign Setup', 'Configure email marketing automation for new feature announcements', @Board2_InProgressStateId, @User1Id, DATEADD(day, 5, GETUTCDATE()));

-- Customer Support Board Tasks
INSERT INTO [Tasks] ([Id], [BoardId], [Title], [Description], [StateId], [AssigneeId], [DueDate]) VALUES 
(@Task7Id, @Board3Id, 'Customer Onboarding Guide', 'Create comprehensive onboarding documentation for new users', @Board3_ToDoStateId, @User2Id, DATEADD(day, 21, GETUTCDATE())),
(@Task8Id, @Board3Id, 'Support Ticket Integration', 'Integrate with helpdesk system for automatic ticket creation', @Board3_ToDoStateId, NULL, DATEADD(day, 30, GETUTCDATE()));

-- ====================
-- SEED DEMO COMMENTS
-- ====================

-- Insert demo comments
INSERT INTO [Comments] ([TaskId], [AuthorId], [Content]) VALUES 
(@Task1Id, @AdminUserId, 'Started working on the Azure AD integration. Following the official Microsoft docs.'),
(@Task1Id, @User1Id, 'Great! Let me know if you need help with the token validation part.'),
(@Task1Id, @AdminUserId, 'Thanks! I might need some assistance with role-based authorization later.'),

(@Task2Id, @User1Id, 'Planning to use ASP.NET Core Web API with Entity Framework. Any objections?'),
(@Task2Id, @AdminUserId, 'Sounds good. Make sure to implement proper error handling and validation.'),

(@Task3Id, @User2Id, 'Database schema is complete. All foreign key constraints are properly configured.'),
(@Task3Id, @AdminUserId, 'Excellent work! The cascade delete issue has been resolved.'),

(@Task4Id, @User1Id, 'Found the issue - comments were being sorted by ID instead of CreatedAt timestamp.'),
(@Task4Id, @User2Id, 'Good catch! Can you also add pagination for comments while you''re at it?'),

(@Task5Id, @User3Id, 'Working on the content calendar. Focusing on feature highlights and user testimonials.'),

(@Task6Id, @User1Id, 'Email templates are ready. Need approval before setting up the automation.'),

(@Task7Id, @User2Id, 'Blocked by legal review of the user agreement terms. Waiting for their feedback.'),
(@Task7Id, @AdminUserId, 'I''ll follow up with the legal team to expedite the review process.');
