CREATE TABLE [dbo].[Satellite]
(
	[ID_Satellite] INT NOT NULL PRIMARY KEY, 
    [Name_Satellite] VARCHAR(20) NOT NULL
);
	
CREATE TABLE [dbo].[SatelliteLocation]
(
	[ID_Satellite] INT NOT NULL, 
    [X_Coord] DECIMAL(10,4) NOT NULL,
	[Y_Coord] DECIMAL(10,4) NOT NULL,
	[From_Location] DATETIME NOT NULL,
	[Until_Location] DATETIME NULL, 
    CONSTRAINT [PK_SatelliteLocation] PRIMARY KEY ([ID_Satellite], [From_Location])
);

CREATE TABLE [dbo].[SatelliteHistory]
(
	[ID_Satellite] INT NOT NULL, 
    [Distance] DECIMAL(10,4) NOT NULL,
	[Message] VARCHAR(1000) NOT NULL,
	[From_History] DATETIME NOT NULL,
	[Until_History] DATETIME NULL, 
    CONSTRAINT [PK_SatelliteHistory] PRIMARY KEY ([ID_Satellite], [From_History])
);