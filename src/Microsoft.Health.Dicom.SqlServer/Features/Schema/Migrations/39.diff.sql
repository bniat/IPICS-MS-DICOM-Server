﻿SET XACT_ABORT ON

BEGIN TRANSACTION
GO

CREATE OR ALTER PROCEDURE dbo.GetChangeFeedLatestByTimeV39
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SELECT   TOP (1) c.Sequence,
                     c.Timestamp,
                     c.Action,
                     p.PartitionName,
                     c.StudyInstanceUid,
                     c.SeriesInstanceUid,
                     c.SopInstanceUid,
                     c.OriginalWatermark,
                     c.CurrentWatermark,
                     f.FilePath
    FROM     dbo.ChangeFeed AS c WITH (HOLDLOCK)
             INNER JOIN dbo.Partition AS p
             ON p.PartitionKey = c.PartitionKey
    -- Left join as instance may have been deleted
             LEFT JOIN dbo.Instance AS i
             ON i.StudyInstanceUid = c.StudyInstanceUid
                AND i.SeriesInstanceUid = c.SeriesInstanceUid
                AND i.SopInstanceUid = c.SopInstanceUid
    -- Left join as instance and property may have been deleted or we never inserted property for instance when not 
    -- using external store
             LEFT JOIN dbo.FileProperty AS f
             ON f.InstanceKey = i.InstanceKey
    ORDER BY c.Timestamp DESC, c.Sequence DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.GetChangeFeedLatestV39
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SELECT   TOP (1) c.Sequence,
                     c.Timestamp,
                     c.Action,
                     p.PartitionName,
                     c.StudyInstanceUid,
                     c.SeriesInstanceUid,
                     c.SopInstanceUid,
                     c.OriginalWatermark,
                     c.CurrentWatermark,
                     f.FilePath
    FROM     dbo.ChangeFeed AS c WITH (HOLDLOCK)
             INNER JOIN dbo.Partition AS p
             ON p.PartitionKey = c.PartitionKey
    -- Left join as instance may have been deleted
             LEFT JOIN dbo.Instance AS i
             ON i.StudyInstanceUid = c.StudyInstanceUid
                AND i.SeriesInstanceUid = c.SeriesInstanceUid
                AND i.SopInstanceUid = c.SopInstanceUid
    -- Left join as instance and property may have been deleted or we never inserted property for instance when not 
    -- using external store
             LEFT JOIN dbo.FileProperty AS f
             ON f.InstanceKey = i.InstanceKey
    ORDER BY c.Sequence DESC;
END
GO


CREATE OR ALTER PROCEDURE dbo.GetChangeFeedLatestV39
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SELECT   TOP (1) c.Sequence,
                     c.Timestamp,
                     c.Action,
                     p.PartitionName,
                     c.StudyInstanceUid,
                     c.SeriesInstanceUid,
                     c.SopInstanceUid,
                     c.OriginalWatermark,
                     c.CurrentWatermark,
                     f.FilePath
    FROM     dbo.ChangeFeed AS c WITH (HOLDLOCK)
             INNER JOIN dbo.Partition AS p
             ON p.PartitionKey = c.PartitionKey
    -- Left join as instance may have been deleted
             LEFT JOIN dbo.Instance AS i
             ON i.StudyInstanceUid = c.StudyInstanceUid
                AND i.SeriesInstanceUid = c.SeriesInstanceUid
                AND i.SopInstanceUid = c.SopInstanceUid
    -- Left join as instance and property may have been deleted or we never inserted property for instance when not 
    -- using external store
             LEFT JOIN dbo.FileProperty AS f
             ON f.InstanceKey = i.InstanceKey
    ORDER BY c.Sequence DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.GetChangeFeedV39
@limit INT, @offset BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SELECT   c.Sequence,
             c.Timestamp,
             c.Action,
             p.PartitionName,
             c.StudyInstanceUid,
             c.SeriesInstanceUid,
             c.SopInstanceUid,
             c.OriginalWatermark,
             c.CurrentWatermark,
             f.FilePath
    FROM     dbo.ChangeFeed AS c WITH (HOLDLOCK)
             INNER JOIN dbo.Partition AS p
             ON p.PartitionKey = c.PartitionKey
    -- Left join as instance may have been deleted
             LEFT JOIN dbo.Instance AS i
             ON i.StudyInstanceUid = c.StudyInstanceUid
                AND i.SeriesInstanceUid = c.SeriesInstanceUid
                AND i.SopInstanceUid = c.SopInstanceUid
    -- Left join as instance and property may have been deleted or we never inserted property for instance when not 
    -- using external store
             LEFT JOIN
             dbo.FileProperty AS f
             ON f.InstanceKey = i.InstanceKey
    WHERE    c.Sequence BETWEEN @offset + 1 AND @offset + @limit
    ORDER BY c.Sequence;
END
GO

COMMIT TRANSACTION