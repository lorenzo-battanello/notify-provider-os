USE BattaTest;
GO

INSERT INTO dbo.NotificationProviderConfigurations
(
    NotificationKey,
    ProviderType,
    SmtpHost,
    SmtpPort,
    UseSsl,
    Username,
    Password,
    FromAddress,
    FromName
)
VALUES
(
    'orders-confirmation',
    1,
    'smtp.gmail.com',
    587,
    1,
    'your-google-username',
    'your-google-password-or-app-password',
    'noreply@example.com',
    'Notify Provider'
);
GO

INSERT INTO dbo.NotificationRecipients (NotificationKey, EmailAddress)
VALUES
('orders-confirmation', 'ops@example.com'),
('orders-confirmation', 'dev@example.com');
GO

INSERT INTO dbo.NotificationAllowedSites (NotificationKey, Host, AllowSubdomains)
VALUES
('orders-confirmation', 'example.com', 1),
('orders-confirmation', 'myshop.com', 0);
GO
