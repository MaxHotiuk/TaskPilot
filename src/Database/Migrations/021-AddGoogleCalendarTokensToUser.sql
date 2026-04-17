-- ========================================
-- ADD GOOGLE CALENDAR INTEGRATION TO USERS
-- ========================================

-- Add OAuth token columns to the Users table
ALTER TABLE [Users]
ADD
    [GoogleAccessToken]          NVARCHAR(2048) NULL,
    [GoogleRefreshToken]         NVARCHAR(512)  NULL,
    [GoogleTokenExpiry]          DATETIME2      NULL,
    [IsGoogleCalendarConnected]  BIT            NOT NULL DEFAULT 0;

GO;
