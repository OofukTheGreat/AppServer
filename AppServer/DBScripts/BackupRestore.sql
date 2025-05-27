-- REPLACE YOUR DATABASE LOGIN AND PASSWORD IN THE SCRIPT BELOW 

Use master
Go

-- Create a login for the admin user
CREATE LOGIN [AdminLogin] WITH PASSWORD = '1234';
Go

--so user can restore the DB!
ALTER ROLE db_owner ADD MEMBER [AdminUser];
Go

Create Database AppServer_DB
go