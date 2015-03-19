Dragon.Security.Hmac.Demo
=========================

A project to demonstrate the usage of the Hmac module.


Setup
-----

* Build the Module project
* Copy the resulting binaries to the bin directory this project
* Setup a SQL Server database in App_Data\db.mdf, and create the tables "Users" and "Apps" as shown in the README.md of the solution.
* Register the demo project by inserting following row:
    INSERT INTO [dbo].[Apps] ([AppId], [ServiceId], [Secret], [Enabled], [CreatedAt]) VALUES 
        (N'00000001-0001-0001-0003-000000000001', N'00000001-0001-0001-0001-000000000001', N'secret', 1, N'2015-02-05 00:00:00')