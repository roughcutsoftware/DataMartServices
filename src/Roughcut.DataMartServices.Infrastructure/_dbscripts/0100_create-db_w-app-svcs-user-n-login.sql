
-- Houston's generic sql server db create script - 2021.06.18
USE master

/*
Introduced in SQL Server 2005, the xp_cmdshell option is a server configuration option 
that enables system administrators to control whether the xp_cmdshell extended stored 
procedure can be executed on a system. 
By default, the xp_cmdshell option is disabled on new installations and can be enabled by 
using the Surface Area Configuration tool or by running the sp_configure system stored procedure 
as shown in the following code example:
*/
-- *** NEED TO TURN FEATURE ON ***
-- To allow advanced options to be changed.
EXEC sp_configure	'show advanced options'
					,1
GO
-- To update the currently configured value for advanced options.
RECONFIGURE
GO
-- To enable the feature.
EXEC sp_configure	'xp_cmdshell'
					,1
GO
-- To update the currently configured value for this feature.
RECONFIGURE
GO

/*
create the directory location for the new db.
*/
EXEC xp_cmdshell 'mkdir D:\sql2012dbs\_community\DataMartServices\'

-- *** NEED TO TURN FEATURE ON ***
-- To allow advanced options to be changed.
EXEC sp_configure	'show advanced options'
					,1
GO
-- To update the currently configured value for advanced options.
RECONFIGURE
GO
-- To enable the feature.
EXEC sp_configure	'xp_cmdshell'
					,0
GO
-- To update the currently configured value for this feature.
RECONFIGURE
GO

CREATE DATABASE DataMartServices
ON
(
NAME = DataMartServices_Data,
FILENAME = 'D:\sql2012dbs\_community\DataMartServices\DataMartServices_Data.MDF',
SIZE = 5 MB,
MAXSIZE = UNLIMITED,
FILEGROWTH = 10 %
)
LOG ON
(
NAME = DataMartServices_Log,
FILENAME = 'D:\sql2012dbs\_community\DataMartServices\DataMartServices_Log.LDF',
SIZE = 1 MB,
MAXSIZE = UNLIMITED,
FILEGROWTH = 10 %
)

GO
-- **************************************************************
-- ALTER DATABASE Compatibility Level (Transact-SQL)
-- https://msdn.microsoft.com/en-us/library/bb510680.aspx 
-- **************************************************************
/*
Product				|	Database Engine Version	|	Compatibility Level Designation	|	Supported Compatibility Level Values
-----------------------------------------------------------------------------------------------------------------
SQL Server vNext	|	14						|	140								|	140, 130, 120, 110, 100 
SQL Server 2016		|	13						|	130								|	130, 120, 110, 100 
SQL Database		|	12						|	120								|	130, 120, 110, 100 
SQL Server 2014		|	12						|	120								|	120, 110, 100 
SQL Server 2012		|	11						|	110								|	110, 100, 90 
SQL Server 2008 R2	|	10.5					|	105								|	100, 90, 80 
*/
-- **************************************************************
ALTER DATABASE DataMartServices SET COMPATIBILITY_LEVEL = 110

/* 
	= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
	IMPORTANT NOTE: !!! change the password !!!	
	= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

*/
DECLARE @tempSql NVARCHAR(2000)
SELECT
	@tempSql = 'CREATE LOGIN [DataMartServicesDbSvcAcct] WITH PASSWORD=''' + 'ChAnG3Th!$P@ssW0Rd' + ''', '
SELECT
	@tempSql = @tempSql + 'DEFAULT_DATABASE=[DataMartServices], '
SELECT
	@tempSql = @tempSql + 'DEFAULT_LANGUAGE=[us_english], '
SELECT
	@tempSql = @tempSql + 'CHECK_EXPIRATION=OFF, '
SELECT
	@tempSql = @tempSql + 'CHECK_POLICY=OFF'
EXEC sp_executesql @tempSql
GO
/* adds the new login as a good login within the target db - Houston - 2021.06.18 */
USE [DataMartServices]
GO
CREATE USER [DataMartServicesDbSvcAcct] FOR LOGIN [DataMartServicesDbSvcAcct] WITH DEFAULT_SCHEMA = [dbo]
GO
/* ensure login is enabled - houston 12.29.07 */
ALTER LOGIN [DataMartServicesDbSvcAcct] ENABLE

/*
	= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
	grant initial base set of app specific permissions - Houston - 2021.06.18
	= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
*/
USE [DataMartServices]
GO
Grant CONNECT ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
USE [DataMartServices]
GO
-- optional - most DBA(s) will NOT allow this role-assignment in Production - Houston - 2021.06.18
--EXEC sp_addrolemember	'db_owner'
--						,'DataMartServicesDbSvcAcct'
--GO
Grant ALTER ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant EXECUTE ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant SELECT ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant UPDATE ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant INSERT ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant DELETE ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
-- ******************************************************************
Grant references ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
Grant SHOWPLAN   ON Database::[DataMartServices] TO [DataMartServicesDbSvcAcct]  
GO
GRANT VIEW DEFINITION TO [DataMartServicesDbSvcAcct];
/* switch back to master db */
USE master