
CREATE TABLE ApplicationUser (

ApplicationUserId bigint IDENTITY (1,1) NOT NULL,  
Username varchar(100) NULL,  
Email varchar(100) NULL,  
Firstname varchar(100) NULL,  
Lastname varchar(100) NULL,  
PasswordHash varchar(MAX) NULL,  

CreatedDate datetime2 NULL,
ModifiedDate datetime2 NULL,
CreatedById bigint  NOT NULL,  
ModifiedById bigint  NULL,
Active bit default 0

);


CREATE   TABLE Customer (

CustomerId bigint IDENTITY (1,1) NOT NULL,  
Username varchar(100) NULL,  
Email varchar(100) NULL,  
Firstname varchar(100) NULL,  
Lastname varchar(100) NULL,  
PasswordHash varchar(MAX) NULL,  
Mobile varchar(100) NULL, 
Address varchar(100) NULL, 
CreatedDate datetime2 NULL,
ModifiedDate datetime2 NULL,
CreatedById bigint  NOT NULL,  
ModifiedById bigint  NULL,
Active bit default 0

);