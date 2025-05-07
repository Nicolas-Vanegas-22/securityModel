CREATE TABLE Person (
    PersonId INT PRIMARY KEY,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Document NVARCHAR(50),
    PhoneNumber NVARCHAR(50),
    Email NVARCHAR(100)
);

CREATE TABLE [User] (
    UserId INT PRIMARY KEY,
    Username NVARCHAR(50),
    Email NVARCHAR(100),
    RegistratorDate DATETIME,
    DeleteAt DATETIME,
    CreateAt DATETIME,
    PersonId INT
);

CREATE TABLE Rol (
    RolId INT PRIMARY KEY,
    RolName NVARCHAR(100),
    Description NVARCHAR(255)
);

CREATE TABLE UserRol (
    UserRolId INT PRIMARY KEY,
    DeleteAt DATETIME,
    CreateAt DATETIME,
    UserId INT,
    RolId INT
);

CREATE TABLE Permission (
    PermissionId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Description NVARCHAR(255)
);

CREATE TABLE RolPermission (
    RolPermissionId INT PRIMARY KEY,
    RolId INT,
    PermissionId INT
);

CREATE TABLE Form (
    FormId INT PRIMARY KEY,
    Code NVARCHAR(50),
    Name NVARCHAR(100),
    Active BIT,
    DeleteAt DATETIME,
    CreateAt DATETIME
);

CREATE TABLE Module (
    ModuleId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Active BIT,
    DeleteAt DATETIME,
    CreateAt DATETIME
);

CREATE TABLE FormModule (
    FormModuleId INT PRIMARY KEY,
    FormId INT,
    ModuleId INT,
    DeleteAt DATETIME,
    CreateAt DATETIME
);

CREATE TABLE RolFormPermission (
    RolFormPermissionId INT PRIMARY KEY,
    CreateAt DATETIME,
    DeleteAt DATETIME,
    RolId INT,
    FormId INT,
    PermissionId INT
);

CREATE TABLE Activity (
    ActivityId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Description NVARCHAR(255),
    Category NVARCHAR(50),
    Price DECIMAL(10,2),
    DurationHours INT,
    Available BIT
);

CREATE TABLE Payment (
    PaymentId INT PRIMARY KEY,
    PaymentMethod NVARCHAR(50),
    Amount DECIMAL(10,2),
    Activity NVARCHAR(100),
    PaymentDate DATETIME,
    UserActivityId INT,
    UserId INT
);

CREATE TABLE UserActivity (
    UserActivityId INT PRIMARY KEY,
    ReservationDate DATETIME,
    Status NVARCHAR(50),
    UserId INT,
    ActivityId INT,
    PaymentId INT
);

CREATE TABLE Destination (
    DestinationId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Description NVARCHAR(255),
    Region NVARCHAR(100),
    Latitude FLOAT,
    Longitude FLOAT
);

CREATE TABLE DestinationActivity (
    DestinationActivityId INT PRIMARY KEY,
    DestinationId INT,
    ActivityId INT
);

CREATE TABLE ChangeLog (
    Id INT PRIMARY KEY,
    TableName NVARCHAR(100),
    RecordId INT,
    Action NVARCHAR(50),
    ChangeTimeStamp DATETIME,
    PreviousValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    DeleteAt DATETIME,
    CreateAt DATETIME
);


-- usuario
ALTER TABLE [User] ADD CONSTRAINT FK_User_Person FOREIGN KEY (PersonId) REFERENCES Person(PersonId);

-- roles
ALTER TABLE UserRol ADD CONSTRAINT FK_UserRol_User FOREIGN KEY (UserId) REFERENCES [User](UserId);
ALTER TABLE UserRol ADD CONSTRAINT FK_UserRol_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);

ALTER TABLE RolPermission ADD CONSTRAINT FK_RolPermission_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);
ALTER TABLE RolPermission ADD CONSTRAINT FK_RolPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId);

-- formularios y módulos
ALTER TABLE FormModule ADD CONSTRAINT FK_FormModule_Form FOREIGN KEY (FormId) REFERENCES Form(FormId);
ALTER TABLE FormModule ADD CONSTRAINT FK_FormModule_Module FOREIGN KEY (ModuleId) REFERENCES Module(ModuleId);

ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);
ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Form FOREIGN KEY (FormId) REFERENCES Form(FormId);
ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId);

-- actividades y pagos
ALTER TABLE Payment ADD CONSTRAINT FK_Payment_User FOREIGN KEY (UserId) REFERENCES [User](UserId);

ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_User FOREIGN KEY (UserId) REFERENCES [User](UserId);
ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_Activity FOREIGN KEY (ActivityId) REFERENCES Activity(ActivityId);
ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_Payment FOREIGN KEY (PaymentId) REFERENCES Payment(PaymentId);

-- destino
ALTER TABLE DestinationActivity ADD CONSTRAINT FK_DestinationActivity_Destination FOREIGN KEY (DestinationId) REFERENCES Destination(DestinationId);
ALTER TABLE DestinationActivity ADD CONSTRAINT FK_DestinationActivity_Activity FOREIGN KEY (ActivityId) REFERENCES Activity(ActivityId);