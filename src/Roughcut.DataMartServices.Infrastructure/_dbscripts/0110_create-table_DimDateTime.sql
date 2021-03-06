USE [DataMartServices]
GO
/****** Object:  Table [dbo].[DimDateTime]    Script Date: 04/11/2017 10:50:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DimDateTime](
	[DateTimeKey] [bigint] NOT NULL,
	[DateTimeCalendarKey] [datetime] NOT NULL,
	[DayNumberOfWeek] [smallint] NOT NULL,
	[EnglishDayNameOfWeek] [nvarchar](10) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[LastChangeDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.DimDateTime] PRIMARY KEY CLUSTERED 
(
	[DateTimeKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
