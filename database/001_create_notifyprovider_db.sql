-- USE master;
-- GO

-- IF DB_ID('NotifyProviderDb') IS NULL
-- BEGIN
--     CREATE DATABASE NotifyProviderDb;
-- END
-- GO

USE BattaTest;
GO

IF OBJECT_ID('dbo.NotificationAllowedSites', 'U') IS NOT NULL
    DROP TABLE dbo.NotificationAllowedSites;
GO

IF OBJECT_ID('dbo.NotificationRecipients', 'U') IS NOT NULL
    DROP TABLE dbo.NotificationRecipients;
GO

IF OBJECT_ID('dbo.NotificationProviderConfigurations', 'U') IS NOT NULL
    DROP TABLE dbo.NotificationProviderConfigurations;
GO

CREATE TABLE dbo.NotificationProviderConfigurations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL UNIQUE,
    ProviderType INT NOT NULL,
    SmtpHost NVARCHAR(255) NOT NULL,
    SmtpPort INT NOT NULL,
    UseSsl BIT NOT NULL,
    Username NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FromAddress NVARCHAR(255) NOT NULL,
    FromName NVARCHAR(255) NOT NULL
);
GO

CREATE TABLE dbo.NotificationRecipients (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL,
    EmailAddress NVARCHAR(255) NOT NULL,
    CONSTRAINT FK_NotificationRecipients_NotificationProviderConfigurations
        FOREIGN KEY (NotificationKey)
        REFERENCES dbo.NotificationProviderConfigurations(NotificationKey)
);
GO

CREATE TABLE dbo.NotificationAllowedSites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL,
    Host NVARCHAR(255) NOT NULL,
    AllowSubdomains BIT NOT NULL CONSTRAINT DF_NotificationAllowedSites_AllowSubdomains DEFAULT 0,
    CONSTRAINT FK_NotificationAllowedSites_NotificationProviderConfigurations
        FOREIGN KEY (NotificationKey)
        REFERENCES dbo.NotificationProviderConfigurations(NotificationKey)
);
GO

CREATE UNIQUE INDEX UX_NotificationAllowedSites_KeyHost
    ON dbo.NotificationAllowedSites(NotificationKey, Host);
GO
