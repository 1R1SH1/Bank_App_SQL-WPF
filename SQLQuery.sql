CREATE TABLE [dbo].[Client] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NULL,
    [SurName]    NVARCHAR (50) NULL,
    [Patronymic] NVARCHAR (50) NULL
);

CREATE TABLE [dbo].[Deposit] (
    [ClientId]       INT           NOT NULL,
    [DepositNumber]  numeric  (18,0) NULL,
    [AmountFunds]    INT           NULL,
    [DepositType]    NVARCHAR (50) NULL
);

SET IDENTITY_INSERT [dbo].[Client] ON
INSERT INTO [dbo].[Client] ([id], [Name], [SurName], [Patronymic]) VALUES (1, N'Карл', N'Карлов', N'Карлович')
SET IDENTITY_INSERT [dbo].[Client] OFF

SELECT * FROM Client

INSERT INTO [dbo].[Deposit] ([ClientId], [DepositNumber], [AmountFunds], [DepositType]) VALUES (1, 1234567891234567, 1000000, N'Капитализированный')

SELECT * FROM Deposit

SELECT 
Client.Id as 'Id',
Deposit.ClientId  as 'Id'
FROM  Client, Deposit
WHERE Client.Id = Deposit.ClientId and Deposit.ClientId = Client.Id
