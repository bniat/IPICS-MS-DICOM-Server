/***************************************************************************************/
-- STORED PROCEDURE
--     GetChangeFeedByTime
--
-- FIRST SCHEMA VERSION
--     36
--
-- DESCRIPTION
--     Gets a subset of dicom changes within a given time range
--
-- PARAMETERS
--     @startTime
--         * Inclusive timestamp start
--     @endTime
--         * Exclusive timestamp end
--     @offet
--         * Rows to skip
--     @limit
--         * Max rows to return
/***************************************************************************************/
CREATE OR ALTER PROCEDURE dbo.GetChangeFeedByTimeV39 (
    @startTime DATETIMEOFFSET(7),
    @endTime   DATETIMEOFFSET(7),
    @limit     INT,
    @offset    BIGINT)
AS
BEGIN
    SET NOCOUNT     ON
    SET XACT_ABORT  ON

    -- As the offset increases, so too does the number of rows read by SQL which may lead
    -- to performance degradation. This can be minimize by smaller time windows as the
    -- Timestamp column is indexed.
    SELECT  
        c.Sequence,
        c.Timestamp,
        c.Action,
        p.PartitionName,
        c.StudyInstanceUid,
        c.SeriesInstanceUid,
        c.SopInstanceUid,
        c.OriginalWatermark,
        c.CurrentWatermark,
		f.FilePath
    FROM dbo.ChangeFeed AS c WITH (HOLDLOCK)
    INNER JOIN dbo.Partition AS p
        ON p.PartitionKey = c.PartitionKey
    -- Left join as instance may have been deleted
    LEFT JOIN dbo.Instance AS i
        ON i.StudyInstanceUid = c.StudyInstanceUid
        AND i.SeriesInstanceUid = c.SeriesInstanceUid
        AND i.SopInstanceUid = c.SopInstanceUid
    -- Left join as instance and property may have been deleted or we never inserted property for instance when not 
    -- using external store
	LEFT JOIN dbo.FileProperty as f
		ON f.InstanceKey = i.InstanceKey
    WHERE c.Timestamp >= @startTime AND c.Timestamp < @endTime
    ORDER BY c.Timestamp, c.Sequence
    OFFSET @offset ROWS
    FETCH NEXT @limit ROWS ONLY
END
