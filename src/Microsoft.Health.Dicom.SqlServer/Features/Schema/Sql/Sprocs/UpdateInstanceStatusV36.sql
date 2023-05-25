﻿/*************************************************************
    Stored procedures for updating an instance status.
**************************************************************/
--
-- STORED PROCEDURE
--     UpdateInstanceStatusV6
--
-- DESCRIPTION
--     Updates a DICOM instance status, which allows for consistency during indexing.
--
-- PARAMETERS
--     @partitionKey
--         * The partition key.
--     @studyInstanceUid
--         * The study instance UID.
--     @seriesInstanceUid
--         * The series instance UID.
--     @sopInstanceUid
--         * The SOP instance UID.
--     @watermark
--         * The watermark.
--     @status
--         * The new status to update to.
--     @maxTagKey
--         * Optional max ExtendedQueryTag key
--     @hasFrameMetadata
--         * Optional flag to indicate frame metadata existance
--     @instanceKey
--         * The instance key.
--     @path
--         * path to dcm blob file
--     @eTag
--         * eTag of upload blob operation
--
-- RETURN VALUE
--     None
--
CREATE OR ALTER PROCEDURE dbo.UpdateInstanceStatusV36
    @partitionKey               INT,
    @studyInstanceUid           VARCHAR(64),
    @seriesInstanceUid          VARCHAR(64),
    @sopInstanceUid             VARCHAR(64),
    @watermark                  BIGINT,
    @status                     TINYINT,
    @maxTagKey                  INT = NULL,
    @hasFrameMetadata           BIT = 0,
    @path                       VARCHAR(4000) = NULL,
    @eTag                       VARCHAR(200) = NULL,
    @instanceKey                BIGINT = NULL,
    @size                       BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON

    SET XACT_ABORT ON
    BEGIN TRANSACTION

    -- This check ensures the client is not potentially missing 1 or more query tags that may need to be indexed.
    -- Note that if @maxTagKey is NULL, < will always return UNKNOWN.
    IF @maxTagKey < (SELECT ISNULL(MAX(TagKey), 0) FROM dbo.ExtendedQueryTag WITH (HOLDLOCK))
        THROW 50409, 'Max extended query tag key does not match', 10

    DECLARE @currentDate DATETIME2(7) = SYSUTCDATETIME()

    UPDATE dbo.Instance
    SET Status = @status, LastStatusUpdatedDate = @currentDate, HasFrameMetadata = @hasFrameMetadata
    WHERE PartitionKey = @partitionKey
        AND StudyInstanceUid = @studyInstanceUid
        AND SeriesInstanceUid = @seriesInstanceUid
        AND SopInstanceUid = @sopInstanceUid
        AND Watermark = @watermark

    -- The instance does not exist. Perhaps it was deleted?
    IF @@ROWCOUNT = 0
        THROW 50404, 'Instance does not exist', 1

    -- Insert to FileProperty
    IF (@instanceKey IS NOT NULL AND @path IS NOT NULL)
        INSERT INTO dbo.FileProperty (InstanceKey, Watermark, FilePath, ETag, Size)
        VALUES                       (@instanceKey, @watermark, @path, @eTag, @size)

    -- Insert to change feed.
    -- Currently this procedure is used only updating the status to created
    -- If that changes an if condition is needed.
    INSERT INTO dbo.ChangeFeed
        (Timestamp, Action, PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, OriginalWatermark)
    VALUES
        (@currentDate, 0, @partitionKey, @studyInstanceUid, @seriesInstanceUid, @sopInstanceUid, @watermark)

    -- Update existing instance currentWatermark to latest
    UPDATE dbo.ChangeFeed
    SET CurrentWatermark      = @watermark
    WHERE PartitionKey = @partitionKey
        AND StudyInstanceUid    = @studyInstanceUid
        AND SeriesInstanceUid = @seriesInstanceUid
        AND SopInstanceUid    = @sopInstanceUid

    COMMIT TRANSACTION
END
