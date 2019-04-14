CREATE ROLE [db_externalexecutor]
GO
GRANT CONNECT TO [db_externalexecutor]
GO
GRANT EXECUTE ON SCHEMA::[External] TO [db_externalexecutor]
GO

CREATE USER ExternalAccess WITH PASSWORD = '{ExternalPassword}';
GO
ALTER ROLE db_externalexecutor ADD MEMBER ExternalAccess;
GO