CREATE TABLE [dbo].[Users] (
    [ID]        BIGINT           NOT NULL PRIMARY KEY IDENTITY,
    [UserId]    UNIQUEIDENTIFIER NULL,
    [AppId]     UNIQUEIDENTIFIER NULL,
    [ServiceId] UNIQUEIDENTIFIER NULL, 
    [Enabled]   BIT              NULL,
    [CreatedAt] DATETIME         NULL,
);

CREATE TABLE [dbo].[Apps]
(
    [Id]        INT              NOT NULL PRIMARY KEY IDENTITY, 
    [AppId]     UNIQUEIDENTIFIER NULL,
    [ServiceId] UNIQUEIDENTIFIER NULL, 
    [Secret]    NVARCHAR(100)    NULL,
    [Enabled]   BIT              NULL,
    [CreatedAt] DATETIME         NULL,
    [Name]      NVARCHAR(50)     NULL,
);
