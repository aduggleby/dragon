Dependencies
============

* Visual Studio 2010
* SQL Server 2010
* ASP.NET MVC 4

Install
=======

* create database (see Dragon in db.config)
* add schema to database:

CREATE TABLE [dbo].[PermissionNode](
	[LID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[ParentID] [uniqueidentifier] NULL,
	[ChildID] [uniqueidentifier] NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[PermissionRight](
	[LID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[NodeID] [uniqueidentifier] NULL,
	[SubjectID] [uniqueidentifier] NULL,
	[Spec] [text] NULL,
	[Inherit] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Registration](
	[RegistrationID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[Service] [nvarchar](50) NULL,
	[Key] [nvarchar](50) NULL,
	[Secret] [nvarchar](550) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Registration](
	[RegistrationID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[Service] [nvarchar](50) NULL,
	[Key] [nvarchar](50) NULL,
	[Secret] [nvarchar](550) NULL
) ON [PRIMARY]

* for WebNotification, add to Global.asx:
  WebNotificationDispatcher.Init();  


Usage
=====

Following tools help in testing the app:

* SMTP: local server, e.g. https://papercut.codeplex.com/
