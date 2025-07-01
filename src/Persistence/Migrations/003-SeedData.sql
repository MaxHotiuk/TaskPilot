-- ========================================
-- Initial seed data for TaskPilot database
-- ========================================

-- ====================
-- SEED STATES
-- ====================

-- Insert default task states
INSERT INTO [States] ([Name]) VALUES 
('To Do'),
('In Progress'),
('Review'),
('Done'),
('Blocked'),
('Cancelled');

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

-- Get state IDs for task creation
DECLARE @ToDoStateId INT = (SELECT [Id] FROM [States] WHERE [Name] = 'To Do');
DECLARE @InProgressStateId INT = (SELECT [Id] FROM [States] WHERE [Name] = 'In Progress');
DECLARE @ReviewStateId INT = (SELECT [Id] FROM [States] WHERE [Name] = 'Review');
DECLARE @DoneStateId INT = (SELECT [Id] FROM [States] WHERE [Name] = 'Done');
DECLARE @BlockedStateId INT = (SELECT [Id] FROM [States] WHERE [Name] = 'Blocked');

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
(@Task1Id, @Board1Id, 'Implement User Authentication', 'Set up Azure AD integration for user authentication and authorization', @InProgressStateId, @AdminUserId, DATEADD(day, 7, GETUTCDATE())),
(@Task2Id, @Board1Id, 'Create Task Management API', 'Develop REST API endpoints for CRUD operations on tasks', @ToDoStateId, @User1Id, DATEADD(day, 14, GETUTCDATE())),
(@Task3Id, @Board1Id, 'Design Database Schema', 'Finalize database schema and create migration scripts', @DoneStateId, @User2Id, NULL),
(@Task4Id, @Board1Id, 'Fix Comment Threading Bug', 'Comments are not displaying in correct threaded order', @ReviewStateId, @User1Id, DATEADD(day, 3, GETUTCDATE()));

-- Marketing Campaign Board Tasks
INSERT INTO [Tasks] ([Id], [BoardId], [Title], [Description], [StateId], [AssigneeId], [DueDate]) VALUES 
(@Task5Id, @Board2Id, 'Social Media Content Calendar', 'Plan and schedule social media posts for Q3', @ToDoStateId, @User3Id, DATEADD(day, 10, GETUTCDATE())),
(@Task6Id, @Board2Id, 'Email Campaign Setup', 'Configure email marketing automation for new feature announcements', @InProgressStateId, @User1Id, DATEADD(day, 5, GETUTCDATE()));

-- Customer Support Board Tasks
INSERT INTO [Tasks] ([Id], [BoardId], [Title], [Description], [StateId], [AssigneeId], [DueDate]) VALUES 
(@Task7Id, @Board3Id, 'Customer Onboarding Guide', 'Create comprehensive onboarding documentation for new users', @BlockedStateId, @User2Id, DATEADD(day, 21, GETUTCDATE())),
(@Task8Id, @Board3Id, 'Support Ticket Integration', 'Integrate with helpdesk system for automatic ticket creation', @ToDoStateId, NULL, DATEADD(day, 30, GETUTCDATE()));

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
