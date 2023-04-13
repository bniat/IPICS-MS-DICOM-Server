﻿/*************************************************************
    Stored procedures for updating an instance status.
**************************************************************/
--
-- STORED PROCEDURE
--     EndUpdateInstance
--
-- DESCRIPTION
--     Bulk update all instances in a study, creates new entry in changefeed.
--
-- PARAMETERS
--     @batchSize
--         * The desired number of instances per batch. Actual number may be smaller.
--     @partitionKey
--         * The partition key.
--     @studyInstanceUid
--         * The study instance UID.
--     @patientId
--         * The Id of the patient.
--     @patientName
--         * The name of the patient.
--     @patientBirthDate
--         * The patient's birth date.
--
-- RETURN VALUE
--     None
--
CREATE OR ALTER PROCEDURE dbo.EndUpdateInstance
    @batchSize                          INT,
    @partitionKey                       INT,
    @studyInstanceUid                   VARCHAR(64),
    @patientId                          NVARCHAR(64) = NULL,
    @patientName                        NVARCHAR(325) = NULL,
    @patientBirthDate                   DATE = NULL
AS
BEGIN
    SET NOCOUNT ON

    SET XACT_ABORT ON
    BEGIN TRANSACTION

    DECLARE @rowsUpdated INT = 0
    DECLARE @imageResourceType AS TINYINT = 0
    DECLARE @currentDate DATETIME2(7) = SYSUTCDATETIME()
    DECLARE @updatedInstances AS TABLE
           (PartitionKey INT,
            StudyInstanceUid VARCHAR(64),
            SeriesInstanceUid VARCHAR(64),
            SopInstanceUid VARCHAR(64),
            StudyKey BIGINT,
            SeriesKey BIGINT,
            InstanceKey BIGINT,
            Watermark BIGINT)

    DECLARE @totalCount INT = (SELECT COUNT(*) FROM dbo.Instance WHERE PartitionKey = @partitionKey AND StudyInstanceUid = @studyInstanceUid AND Status = 1 AND NewWatermark IS NOT NULL) 

    WHILE (@rowsUpdated < @totalCount)
    BEGIN
        
        DELETE FROM @updatedInstances

        UPDATE TOP (@batchSize) dbo.Instance
        SET LastStatusUpdatedDate = @currentDate, OriginalWatermark = ISNULL(OriginalWatermark, Watermark), Watermark = NewWatermark, NewWatermark = NULL
        OUTPUT deleted.PartitionKey, @studyInstanceUid, deleted.SeriesInstanceUid, deleted.SopInstanceUid, deleted.StudyKey, deleted.SeriesKey, deleted.InstanceKey, deleted.NewWatermark  INTO @updatedInstances
        WHERE PartitionKey = @partitionKey
            AND StudyInstanceUid = @studyInstanceUid
            AND Status = 1
            AND NewWatermark IS NOT NULL

        SET @rowsUpdated = @rowsUpdated + @@ROWCOUNT;

        -- Insert into change feed table for update action type
        INSERT INTO dbo.ChangeFeed
        (TimeStamp, Action, PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, OriginalWatermark, CurrentWatermark)
        SELECT @currentDate, 2, PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, Watermark, Watermark
        FROM @updatedInstances
    END

    -- Only updating patient information in a study
    UPDATE dbo.Study
    SET PatientId = ISNULL(@patientId, PatientId), 
        PatientName = ISNULL(@patientName, PatientName), 
        PatientBirthDate = ISNULL(@patientBirthDate, PatientBirthDate)
    WHERE PartitionKey = @partitionKey
        AND StudyInstanceUid = @studyInstanceUid 

    -- The study does not exist. May be deleted
    IF @@ROWCOUNT = 0
        THROW 50404, 'Study does not exist', 1

    COMMIT TRANSACTION
END