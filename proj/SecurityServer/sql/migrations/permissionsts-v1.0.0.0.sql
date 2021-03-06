-- Script Date: 12/14/2015 12:32 PM  - ErikEJ.SqlCeScripting version 3.5.2.56
CREATE TABLE [HmacUser] (
  [Id] bigint NOT NULL PRIMARY KEY IDENTITY
, [UserId] uniqueidentifier NOT NULL
, [AppId] uniqueidentifier NOT NULL
, [ServiceId] uniqueidentifier NOT NULL
, [Enabled] bit NOT NULL
, [CreatedAt] datetime NOT NULL
);
GO
CREATE TABLE [IdentityUserLogin] (
  [LoginProvider] nvarchar(200) NOT NULL
, [ProviderKey] nvarchar(200) NOT NULL
, [UserId] nvarchar(50) NOT NULL
);
GO
CREATE TABLE [IdentityUserClaim] (
  [Id] nvarchar(50) NOT NULL
, [UserId] nvarchar(50) NULL
, [ClaimType] nvarchar(1000) NULL
, [ClaimValue] nvarchar(1000) NULL
);
GO
CREATE TABLE [Exceptions] (
  [Id] bigint IDENTITY (2,1) NOT NULL
, [GUID] uniqueidentifier NOT NULL
, [ApplicationName] nvarchar(50) NOT NULL
, [MachineName] nvarchar(50) NOT NULL
, [CreationDate] datetime NOT NULL
, [Type] nvarchar(100) NOT NULL
, [IsProtected] bit DEFAULT 0 NOT NULL
, [Host] nvarchar(100) NULL
, [Url] nvarchar(500) NULL
, [HTTPMethod] nvarchar(10) NULL
, [IPAddress] nvarchar(40) NULL
, [Source] nvarchar(100) NULL
, [Message] nvarchar(1000) NULL
, [Detail] ntext NULL
, [StatusCode] int NULL
, [SQL] ntext NULL
, [DeletionDate] datetime NULL
, [FullJson] ntext NULL
, [ErrorHash] int NULL
, [DuplicateCount] int DEFAULT 1 NOT NULL
);
GO
CREATE TABLE [HmacConsumer] (
  [Id] int NOT NULL PRIMARY KEY IDENTITY
, [AppId] uniqueidentifier NOT NULL
, [ServiceId] uniqueidentifier NOT NULL
, [Secret] nvarchar(200) NULL
, [Enabled] bit NOT NULL
, [CreatedAt] datetime NOT NULL
, [Name] nvarchar(100) NULL
);
GO
CREATE TABLE [IdentityUser] (
  [Id] nvarchar(50) NOT NULL
);
GO
INSERT INTO [HmacConsumer] ([AppId],[ServiceId],[Secret],[Enabled],[CreatedAt],[Name]) VALUES ('00000001-0001-0003-0003-200000000001','00000001-0001-0003-0003-000000000011',N'secret_permissionSTS',1,{ts '2015-12-14 11:30:49.403'},N'PermissionSTSClientTest');
GO
ALTER TABLE [IdentityUserLogin] ADD CONSTRAINT [PK_IdentityUserLogin] PRIMARY KEY ([LoginProvider],[ProviderKey],[UserId]);
GO
ALTER TABLE [IdentityUserClaim] ADD CONSTRAINT [PK_IdentityUserClaim] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Exceptions] ADD CONSTRAINT [PK_Exceptions] PRIMARY KEY ([Id]);
GO
ALTER TABLE [IdentityUser] ADD CONSTRAINT [PK_IdentityUser] PRIMARY KEY ([Id]);
GO
CREATE INDEX [IX_Exceptions_ApplicationName_DeletionDate_CreationDate_Filtered] ON [Exceptions] ([ApplicationName] ASC,[DeletionDate] ASC,[CreationDate] DESC);
GO
CREATE INDEX [IX_Exceptions_ErrorHash_ApplicationName_CreationDate_DeletionDate] ON [Exceptions] ([ErrorHash] ASC,[ApplicationName] ASC,[CreationDate] DESC,[DeletionDate] ASC);
GO
ALTER TABLE [Exceptions] ADD CONSTRAINT [IX_Exceptions_GUID_ApplicationName_DeletionDate_CreationDate] UNIQUE ([GUID],[ApplicationName],[DeletionDate],[CreationDate]);
GO

