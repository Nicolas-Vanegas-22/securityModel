
CREATE TABLE Person (
    PersonId INTEGER PRIMARY KEY,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Document VARCHAR(50),
    PhoneNumber VARCHAR(50),
    Email VARCHAR(100)
);

CREATE TABLE "User" (
    UserId INTEGER PRIMARY KEY,
    Username VARCHAR(50),
    Email VARCHAR(100),
    RegistratorDate TIMESTAMP,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP,
    PersonId INTEGER
);

CREATE TABLE Rol (
    RolId INTEGER PRIMARY KEY,
    RolName VARCHAR(100),
    Description VARCHAR(255)
);

CREATE TABLE UserRol (
    UserRolId INTEGER PRIMARY KEY,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP,
    UserId INTEGER,
    RolId INTEGER
);

CREATE TABLE Permission (
    PermissionId INTEGER PRIMARY KEY,
    Name VARCHAR(100),
    Description VARCHAR(255)
);

CREATE TABLE RolPermission (
    RolPermissionId INTEGER PRIMARY KEY,
    RolId INTEGER,
    PermissionId INTEGER
);

CREATE TABLE Form (
    FormId INTEGER PRIMARY KEY,
    Code VARCHAR(50),
    Name VARCHAR(100),
    Active BOOLEAN,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP
);

CREATE TABLE Module (
    ModuleId INTEGER PRIMARY KEY,
    Name VARCHAR(100),
    Active BOOLEAN,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP
);

CREATE TABLE FormModule (
    FormModuleId INTEGER PRIMARY KEY,
    FormId INTEGER,
    ModuleId INTEGER,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP
);

CREATE TABLE RolFormPermission (
    RolFormPermissionId INTEGER PRIMARY KEY,
    CreateAt TIMESTAMP,
    DeleteAt TIMESTAMP,
    RolId INTEGER,
    FormId INTEGER,
    PermissionId INTEGER
);

CREATE TABLE Activity (
    ActivityId INTEGER PRIMARY KEY,
    Name VARCHAR(100),
    Description VARCHAR(255),
    Category VARCHAR(50),
    Price DECIMAL(10,2),
    DurationHours INTEGER,
    Available BOOLEAN
);

CREATE TABLE Payment (
    PaymentId INTEGER PRIMARY KEY,
    PaymentMethod VARCHAR(50),
    Amount DECIMAL(10,2),
    Activity VARCHAR(100),
    PaymentDate TIMESTAMP,
    UserActivityId INTEGER,
    UserId INTEGER
);

CREATE TABLE UserActivity (
    UserActivityId INTEGER PRIMARY KEY,
    ReservationDate TIMESTAMP,
    Status VARCHAR(50),
    UserId INTEGER,
    ActivityId INTEGER,
    PaymentId INTEGER
);

CREATE TABLE Destination (
    DestinationId INTEGER PRIMARY KEY,
    Name VARCHAR(100),
    Description VARCHAR(255),
    Region VARCHAR(100),
    Latitude DOUBLE PRECISION,
    Longitude DOUBLE PRECISION
);

CREATE TABLE DestinationActivity (
    DestinationActivityId INTEGER PRIMARY KEY,
    DestinationId INTEGER,
    ActivityId INTEGER
);

CREATE TABLE ChangeLog (
    Id INTEGER PRIMARY KEY,
    TableName VARCHAR(100),
    RecordId INTEGER,
    Action VARCHAR(50),
    ChangeTimeStamp TIMESTAMP,
    PreviousValue TEXT,
    NewValue TEXT,
    DeleteAt TIMESTAMP,
    CreateAt TIMESTAMP
);

-- Foreign keys
ALTER TABLE "User" ADD CONSTRAINT FK_User_Person FOREIGN KEY (PersonId) REFERENCES Person(PersonId);

ALTER TABLE UserRol ADD CONSTRAINT FK_UserRol_User FOREIGN KEY (UserId) REFERENCES "User"(UserId);
ALTER TABLE UserRol ADD CONSTRAINT FK_UserRol_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);

ALTER TABLE RolPermission ADD CONSTRAINT FK_RolPermission_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);
ALTER TABLE RolPermission ADD CONSTRAINT FK_RolPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId);

ALTER TABLE FormModule ADD CONSTRAINT FK_FormModule_Form FOREIGN KEY (FormId) REFERENCES Form(FormId);
ALTER TABLE FormModule ADD CONSTRAINT FK_FormModule_Module FOREIGN KEY (ModuleId) REFERENCES Module(ModuleId);

ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Rol FOREIGN KEY (RolId) REFERENCES Rol(RolId);
ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Form FOREIGN KEY (FormId) REFERENCES Form(FormId);
ALTER TABLE RolFormPermission ADD CONSTRAINT FK_RolFormPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId);

ALTER TABLE Payment ADD CONSTRAINT FK_Payment_User FOREIGN KEY (UserId) REFERENCES "User"(UserId);

ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_User FOREIGN KEY (UserId) REFERENCES "User"(UserId);
ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_Activity FOREIGN KEY (ActivityId) REFERENCES Activity(ActivityId);
ALTER TABLE UserActivity ADD CONSTRAINT FK_UserActivity_Payment FOREIGN KEY (PaymentId) REFERENCES Payment(PaymentId);

ALTER TABLE DestinationActivity ADD CONSTRAINT FK_DestinationActivity_Destination FOREIGN KEY (DestinationId) REFERENCES Destination(DestinationId);
ALTER TABLE DestinationActivity ADD CONSTRAINT FK_DestinationActivity_Activity FOREIGN KEY (ActivityId) REFERENCES Activity(ActivityId);
