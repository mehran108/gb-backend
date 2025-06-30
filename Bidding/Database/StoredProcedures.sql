
----- STORED PROCEDURE FOR GetAll


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE ApplicationUserGetAll
--AS
--	BEGIN
--		Select ApplicationUserId, Username, Email, Firstname, Lastname, CreatedDate, ModifiedDate, CreatedById, ModifiedById, Active from ApplicationUser
--		where Active = 1
--	END

----- STORED PROCEDURE FOR DeleteByID


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE ApplicationUserActivate
--	@PApplicationUserId  bigint, @PCurrentUserId int,@PActive bit
--AS
--BEGIN
--	UPDATE ApplicationUser 
--	SET Active = @PActive ,
--		ModifiedDate = GETDATE(),
--		ModifiedById = @PCurrentUserId
--	where ApplicationUserId= @PApplicationUserId
--END

----- STORED PROCEDURE FOR Update


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE ApplicationUserUpdate
--@PCurrentUserId  bigint, @PApplicationUserId  bigint, @PUsername  nvarchar(MAX), @PEmail  nvarchar(MAX), @PFirstname  nvarchar(MAX), @PLastname  nvarchar(MAX),  @PModifiedDate  DateTime, @PModifiedById  bigint, @PActive  bit
--AS
--BEGIN
--	UPDATE ApplicationUser
--	SET  
--	  Username=@PUsername, Email=@PEmail, Firstname=@PFirstname, Lastname=@PLastname,  ModifiedDate=GETDATE(), ModifiedById=@PCurrentUserId, Active=@PActive
--	where ApplicationUserId = @PApplicationUserId;
--END

----- STORED PROCEDURE FOR Add


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE ApplicationUserAdd
--@PApplicationUserId  bigint OUT, @PUsername  nvarchar(MAX), @PEmail  nvarchar(MAX), @PFirstname  nvarchar(MAX), @PLastname  nvarchar(MAX), @PPasswordHash  nvarchar(MAX), @PCreatedDate  DateTime, @PCreatedById  bigint, @PActive  bit
--AS
--BEGIN
--INSERT ApplicationUser(Username, Email, Firstname, Lastname,PasswordHash , CreatedDate, CreatedById, Active) VALUES 
--					(@PUsername, @PEmail, @PFirstname, @PLastname,@PPasswordHash, GETDATE(),@PCreatedById , @PActive);
--SET @PApplicationUserId = scope_identity();
--END

----- STORED PROCEDURE FOR GetById


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE ApplicationUserGetById
--@PApplicationUserId int,
--@PEmail varchar(max)
--AS
--BEGIN
--Select ApplicationUserId, Username, Email, Firstname, Lastname, CreatedDate, ModifiedDate, CreatedById, ModifiedById, Active
--from ApplicationUser 
--where 
--    (ApplicationUserId = @PApplicationUserId OR @PApplicationUserId = 0)
--    AND
--    (Email = @PEmail OR @PEmail = '');
     
--END




--- STORED PROCEDURE FOR GetAll


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE CustomerGetAll
--AS
--	BEGIN
--		Select CustomerId, Username, Email, Firstname, Lastname,Mobile,Address, CreatedDate, ModifiedDate, CreatedById, ModifiedById, Active from Customer
--		where Active = 1
--	END

----- STORED PROCEDURE FOR DeleteByID


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE CustomerActivate
--	@PCustomerId  bigint, @PCurrentUserId int,@PActive bit
--AS
--BEGIN
--	UPDATE Customer 
--	SET Active = @PActive ,
--		ModifiedDate = GETDATE(),
--		ModifiedById = @PCurrentUserId
--	where CustomerId= @PCustomerId
--END

----- STORED PROCEDURE FOR Update


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE CustomerUpdate
--@PCurrentUserId  bigint, @PCustomerId  bigint, @PUsername  nvarchar(MAX), @PEmail  nvarchar(MAX), @PFirstname  nvarchar(MAX), @PLastname  nvarchar(MAX),@PMobile nvarchar(max),@PAddress nvarchar(max),  @PModifiedDate  DateTime, @PModifiedById  bigint, @PActive  bit
--AS
--BEGIN
--	UPDATE Customer
--	SET  
--	  Username=@PUsername, Email=@PEmail, Firstname=@PFirstname, Lastname=@PLastname, Mobile = @PMobile, Address = @PAddress, ModifiedDate=GETDATE(), ModifiedById=@PCurrentUserId, Active=@PActive
--	where CustomerId = @PCustomerId;
--END

----- STORED PROCEDURE FOR Add


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE CustomerAdd
--@PCustomerId  bigint OUT, @PUsername  nvarchar(MAX), @PEmail  nvarchar(MAX), @PFirstname  nvarchar(MAX), @PLastname  nvarchar(MAX), @PPasswordHash  nvarchar(MAX),@PMobile nvarchar(max),@PAddress nvarchar(max), @PCreatedDate  DateTime, @PCreatedById  bigint, @PActive  bit
--AS
--BEGIN
--INSERT Customer(Username, Email, Firstname, Lastname,Mobile, Address,PasswordHash,  CreatedDate, CreatedById, Active) VALUES 
--					(@PUsername, @PEmail, @PFirstname, @PLastname,@PMobile, @PAddress,@PPasswordHash, GETDATE(),@PCreatedById , @PActive);
--SET @PCustomerId = scope_identity();
--END

----- STORED PROCEDURE FOR GetById


--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE OR ALTER    PROCEDURE CustomerGetById
--@PCustomerId int,
--@PEmail varchar(max)
--AS
--BEGIN
--Select CustomerId, Username, Email, Firstname, Lastname,Mobile, Address, CreatedDate, ModifiedDate, CreatedById, ModifiedById, Active
--from Customer 
--where 
--    (CustomerId = @PCustomerId OR @PCustomerId = 0)
--    AND
--    (Email = @PEmail OR @PEmail = '');
     
--END




