Dependencies
============

* Visual Studio 2010
* SQL Server 2010
* ASP.NET MVC 4

Install
=======

* create a db.config with a ConnectionString called Dragon
* create database (see Dragon in db.config)
* add schema to database:

CREATE TABLE [dbo].[DragonPermissionNode](
	[LID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[ParentID] [uniqueidentifier] NULL,
	[ChildID] [uniqueidentifier] NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[DragonPermissionRight](
	[LID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[NodeID] [uniqueidentifier] NULL,
	[SubjectID] [uniqueidentifier] NULL,
	[Spec] [text] NULL,
	[Inherit] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[DragonRegistration](
	[RegistrationID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[Service] [nvarchar](50) NULL,
	[Key] [nvarchar](50) NULL,
	[Secret] [nvarchar](550) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[DragonSession](
	[SessionID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[Location] [nvarchar](50) NULL,
	[Hash] [integer] NULL,
	[Expires] [datetime] NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[DragonProfile](
	[LID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[Key] [nvarchar](50) NULL,
	[Value] [nvarchar](400) NULL,
) ON [PRIMARY]

CREATE TABLE [dbo].[Notification](
	[NotificationID] [uniqueidentifier] ROWGUIDCOL  NULL,
	[UserID] [uniqueidentifier] NULL,
	[TypeKey] [nvarchar](50) NULL,
	[Subject] [nvarchar](100) NULL,
	[LanguageCode] [nvarchar](10) NULL,
	[Parameter] [nvarchar](500) NULL,
	[Dispatched] [bit]
) ON [PRIMARY]

ALTER TABLE [dbo].[Notification] ADD  CONSTRAINT [DF_Notification_Dispatched]  DEFAULT ((0)) FOR [Dispatched]

* for WebNotification, add to Global.asx:
  WebNotificationDispatcher.Init();
  WebNotificationDispatcher.NotificationHub.Dispatcher = new WebNotificationDispatcher(...);


Usage
=====

Following tools help in testing the app:

* SMTP: local server, e.g. https://papercut.codeplex.com/
