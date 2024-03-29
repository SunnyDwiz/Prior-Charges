USE [master]
GO
/****** Object:  Database [PharMerica]    Script Date: 3/15/2019 8:30:00 PM ******/
CREATE DATABASE [PharMerica]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PharMerica', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS2014\MSSQL\DATA\PharMerica.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PharMerica_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS2014\MSSQL\DATA\PharMerica_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [PharMerica] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PharMerica].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PharMerica] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PharMerica] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PharMerica] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PharMerica] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PharMerica] SET ARITHABORT OFF 
GO
ALTER DATABASE [PharMerica] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PharMerica] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PharMerica] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PharMerica] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PharMerica] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PharMerica] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PharMerica] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PharMerica] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PharMerica] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PharMerica] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PharMerica] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PharMerica] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PharMerica] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PharMerica] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PharMerica] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PharMerica] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PharMerica] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PharMerica] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PharMerica] SET  MULTI_USER 
GO
ALTER DATABASE [PharMerica] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PharMerica] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PharMerica] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PharMerica] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [PharMerica] SET DELAYED_DURABILITY = DISABLED 
GO
USE [PharMerica]
GO
/****** Object:  Table [dbo].[City]    Script Date: 3/15/2019 8:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[CityId] [int] IDENTITY(1,1) NOT NULL,
	[CityName] [varchar](25) NULL,
	[FKStateID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[State]    Script Date: 3/15/2019 8:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[State](
	[StateId] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [varchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRole]    Script Date: 3/15/2019 8:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRole](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Temprature]    Script Date: 3/15/2019 8:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Temprature](
	[TempId] [int] IDENTITY(1,1) NOT NULL,
	[Country] [varchar](50) NULL,
	[States] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[Location] [varchar](50) NULL,
	[Temp] [decimal](18, 0) NULL,
	[IsDelete] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[TempId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserData]    Script Date: 3/15/2019 8:30:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserData](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](25) NOT NULL,
	[LastName] [varchar](25) NULL,
	[UserName] [varchar](25) NULL,
	[Password] [varchar](15) NOT NULL,
	[Email] [varchar](25) NOT NULL,
	[Mobile] [varchar](12) NOT NULL,
	[Gender] [varchar](10) NOT NULL,
	[Address] [varchar](40) NOT NULL,
	[CityID] [int] NOT NULL,
	[StateID] [int] NOT NULL,
	[PinCode] [varchar](6) NOT NULL,
	[CreatedDate] [date] NULL DEFAULT (getdate()),
	[ModifiedDate] [date] NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedBy] [int] NULL,
	[RoleID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[City] ON 

INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (1, N'Patna', 1)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (2, N'SriNagar', 2)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (3, N'Leh', 2)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (4, N'Simla', 3)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (5, N'Manali', 3)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (6, N'Ranchi', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (7, N'Jamsedpur', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (8, N'Dhanbad', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (9, N'Lakhnow', 5)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (10, N'Kanpur', 5)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (11, N'Jabalpur', 6)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (12, N'Sagar', 6)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (13, N'Nagpur', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (14, N'Mumbai', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (15, N'Pune', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (16, N'Secunderabad', 8)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (17, N'Hyderabad', 8)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (18, N'Simoga', 9)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (19, N'Bangalore', 9)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (20, N'Bhagalpur', 1)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (21, N'Anantnag', 2)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (22, N'Baramula', 2)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (23, N'Dharamsala', 3)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (24, N'Mandi', 3)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (25, N'Bokaro', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (26, N'West.Singhbhum', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (27, N'Devghar', 4)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (28, N'Merath', 5)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (29, N'Noida', 5)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (30, N'Bhopal', 6)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (31, N'Katni', 6)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (32, N'Nashik', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (33, N'Solapur', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (34, N'Amravati', 7)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (35, N'Warangal', 8)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (36, N'Suryapet', 8)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (37, N'Manglore', 9)
INSERT [dbo].[City] ([CityId], [CityName], [FKStateID]) VALUES (38, N'Mysore', 9)
SET IDENTITY_INSERT [dbo].[City] OFF
SET IDENTITY_INSERT [dbo].[State] ON 

INSERT [dbo].[State] ([StateId], [StateName]) VALUES (1, N'Bihar')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (2, N'J&K')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (3, N'Himachal')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (4, N'Jharkhand')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (5, N'UttarPardes')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (6, N'MadhayPrades')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (7, N'Maharastra')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (8, N'Telanagna')
INSERT [dbo].[State] ([StateId], [StateName]) VALUES (9, N'Karnatka')
SET IDENTITY_INSERT [dbo].[State] OFF
SET IDENTITY_INSERT [dbo].[tblRole] ON 

INSERT [dbo].[tblRole] ([RoleId], [RoleName]) VALUES (1, N'Admin')
INSERT [dbo].[tblRole] ([RoleId], [RoleName]) VALUES (2, N'User')
SET IDENTITY_INSERT [dbo].[tblRole] OFF
SET IDENTITY_INSERT [dbo].[Temprature] ON 

INSERT [dbo].[Temprature] ([TempId], [Country], [States], [City], [Location], [Temp], [IsDelete]) VALUES (1, N'India', N'Telangana', N'HYD', N'HI-Tech City', CAST(42 AS Decimal(18, 0)), 1)
INSERT [dbo].[Temprature] ([TempId], [Country], [States], [City], [Location], [Temp], [IsDelete]) VALUES (2, N'India', N'Telangana', N'SEC', N'Nampally', CAST(45 AS Decimal(18, 0)), 1)
INSERT [dbo].[Temprature] ([TempId], [Country], [States], [City], [Location], [Temp], [IsDelete]) VALUES (3, N'India', N'Jharkhand', N'RNC', N'LalPur', CAST(45 AS Decimal(18, 0)), 0)
INSERT [dbo].[Temprature] ([TempId], [Country], [States], [City], [Location], [Temp], [IsDelete]) VALUES (4, N'India', N'Jharkhand', N'RNC', N'Namkum', CAST(47 AS Decimal(18, 0)), 1)
SET IDENTITY_INSERT [dbo].[Temprature] OFF
SET IDENTITY_INSERT [dbo].[UserData] ON 

INSERT [dbo].[UserData] ([UserId], [FirstName], [LastName], [UserName], [Password], [Email], [Mobile], [Gender], [Address], [CityID], [StateID], [PinCode], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [RoleID]) VALUES (1, N'Sunny Kumar', N'Singh', N'Sunny', N'sunny@123', N'Sunny.singh@yash.com', N'8965245874', N'Male', N'Ratu Road', 6, 4, N'834001', CAST(N'2019-03-01' AS Date), NULL, 1, NULL, 1)
SET IDENTITY_INSERT [dbo].[UserData] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__UserData__C9F2845698D9B516]    Script Date: 3/15/2019 8:30:00 PM ******/
ALTER TABLE [dbo].[UserData] ADD UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD FOREIGN KEY([FKStateID])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[UserData]  WITH CHECK ADD FOREIGN KEY([CityID])
REFERENCES [dbo].[City] ([CityId])
GO
ALTER TABLE [dbo].[UserData]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[tblRole] ([RoleId])
GO
ALTER TABLE [dbo].[UserData]  WITH CHECK ADD FOREIGN KEY([StateID])
REFERENCES [dbo].[State] ([StateId])
GO
USE [master]
GO
ALTER DATABASE [PharMerica] SET  READ_WRITE 
GO
