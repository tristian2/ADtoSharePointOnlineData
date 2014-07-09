USE [DigitalWorkspace]
GO

/****** Object:  Table [dbo].[UserProfileImportFails]    Script Date: 15/04/2014 17:21:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


USE [DigitalWorkspace]
GO

/****** Object:  Table [dbo].[UserProfileImportFails]    Script Date: 15/04/2014 17:22:02 ******/
DROP TABLE [dbo].[UserProfileImportFails]
GO




CREATE TABLE [dbo].[UserProfileImportFails](
	[AccountName] [varchar](255) NOT NULL,
	[Timestamp] [datetime] NULL,
	[State] [int] NULL,
	[ErrorCode] [int] NULL,
	[ErrorMessage] [nvarchar](1000) NULL,
	[NumberOfRetries] [int] NULL,
	[PropertyDataSerialized] [nvarchar](1000) NULL,
	[Email] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


