CREATE TABLE [dbo].[Applications] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [NameApp]     NVARCHAR (50) NOT NULL,
    [Creation_dt] DATETIME      NOT NULL,
    [Res_type]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([NameApp] ASC)
);

CREATE TABLE [dbo].[Datas] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Content]     NVARCHAR (50) NOT NULL,
    [Creation_dt] DATETIME      NOT NULL,
    [Parent]      INT           NOT NULL,
    [Res_type]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Datas_ToModules] FOREIGN KEY ([Parent]) REFERENCES [dbo].[Modules] ([Id])
);

CREATE TABLE [dbo].[Modules] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [NameMod]     NVARCHAR (50) NOT NULL,
    [Creation_dt] DATETIME      NOT NULL,
    [Parent]      INT           NOT NULL,
    [Res_type]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([NameMod] ASC),
    CONSTRAINT [FK_Modules_ToApplications] FOREIGN KEY ([Parent]) REFERENCES [dbo].[Applications] ([Id])
);

CREATE TABLE [dbo].[Subscriptions] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [NameSub]     NVARCHAR (50) NOT NULL,
    [Creation_dt] NVARCHAR (50) NOT NULL,
    [Parent]      INT           NOT NULL,
    [Event]       NVARCHAR (50) NOT NULL,
    [EndPoint]    NVARCHAR (50) NOT NULL,
    [Res_type]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([NameSub] ASC),
    CONSTRAINT [FK_Subscriptions_ToModules] FOREIGN KEY ([Parent]) REFERENCES [dbo].[Modules] ([Id]),
    CONSTRAINT [CK_Subscriptions_Event] CHECK ([Event]='creation' OR [Event]='deletion' OR [Event]='both')
);
