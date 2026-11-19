using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    { //TODO: create procedures for all other calculated params (the values to calculate from are inside the comments right next to them)
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        //Procedures
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationPoints`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s points data'
BEGIN
	DECLARE points, pointsToday, pointsYesterday, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT SUM(Amount) INTO points FROM PlayerCreationPoints WHERE PlayerCreationId=creationId;
	SELECT SUM(Amount) INTO pointsToday FROM PlayerCreationPoints WHERE PlayerCreationId=creationId AND CreatedAt > DATE(CURRENT_TIMESTAMP(6));
	SELECT SUM(Amount) INTO pointsYesterday FROM PlayerCreationPoints WHERE PlayerCreationId=creationId AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL -1 DAY) AND CreatedAt < DATE(CURRENT_TIMESTAMP(6));
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerCreationPoints WHERE PlayerCreationId=creationId AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerCreationPoints WHERE PlayerCreationId=creationId AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsToday IS NULL) THEN
		SET pointsToday = 0;
	END IF;
	
	IF(pointsYesterday IS NULL) THEN
		SET pointsYesterday = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
	
	UPDATE PlayerCreations SET Points=points, PointsToday=pointsToday, PointsYesterday=pointsYesterday, PointsThisWeek=pointsThisWeek, PointsLastWeek=pointsLastWeek WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationDownloads`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s downloads data'
BEGIN
	DECLARE downloads, downloadsThisWeek, downloadsLastWeek INT DEFAULT 0;
	
	SELECT COUNT(*) INTO downloads FROM PlayerCreationDownloads WHERE PlayerCreationId=creationId;
	SELECT COUNT(*) INTO downloadsThisWeek FROM PlayerCreationDownloads WHERE PlayerCreationId=creationId AND DownloadedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO downloadsLastWeek FROM PlayerCreationDownloads WHERE PlayerCreationId=creationId AND DownloadedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND DownloadedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(downloads IS NULL) THEN
		SET downloads = 0;
	END IF;
	
	IF(downloadsThisWeek IS NULL) THEN
		SET downloadsThisWeek = 0;
	END IF;
	
	IF(downloadsLastWeek IS NULL) THEN
		SET downloadsLastWeek = 0;
	END IF;
	
	UPDATE PlayerCreations SET Downloads=downloads, DownloadsThisWeek=downloadsThisWeek, DownloadsLastWeek=downloadsLastWeek WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationViews`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s views data'
BEGIN
	DECLARE views, viewsThisWeek, viewsLastWeek INT DEFAULT 0;
	
	SELECT COUNT(*) INTO views FROM PlayerCreationViews WHERE PlayerCreationId=creationId;
	SELECT COUNT(*) INTO viewsThisWeek FROM PlayerCreationViews WHERE PlayerCreationId=creationId AND ViewedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO viewsLastWeek FROM PlayerCreationViews WHERE PlayerCreationId=creationId AND ViewedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND ViewedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(views IS NULL) THEN
		SET views = 0;
	END IF;
	
	IF(viewsThisWeek IS NULL) THEN
		SET viewsThisWeek = 0;
	END IF;
	
	IF(viewsLastWeek IS NULL) THEN
		SET viewsLastWeek = 0;
	END IF;
	
	UPDATE PlayerCreations SET Views=views, ViewsThisWeek=viewsThisWeek, ViewsLastWeek=viewsLastWeek WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationHearts`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s hearts data'
BEGIN
	DECLARE hearts, heartsThisWeek, heartsThisMonth INT DEFAULT 0;
	
	SELECT COUNT(*) INTO hearts FROM HeartedPlayerCreations WHERE HeartedPlayerCreationId=creationId;
	SELECT COUNT(*) INTO heartsThisWeek FROM HeartedPlayerCreations WHERE HeartedPlayerCreationId=creationId AND HeartedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO heartsThisMonth FROM HeartedPlayerCreations WHERE HeartedPlayerCreationId=creationId AND HeartedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-(DAYOFMONTH(CURRENT_TIMESTAMP(6))-1)) DAY);
	
	IF(hearts IS NULL) THEN
		SET hearts = 0;
	END IF;
	
	IF(heartsThisWeek IS NULL) THEN
		SET heartsThisWeek = 0;
	END IF;
	
	IF(heartsThisMonth IS NULL) THEN
		SET heartsThisMonth = 0;
	END IF;
	
	UPDATE PlayerCreations SET Hearts=hearts, HeartsThisWeek=heartsThisWeek, HeartsThisMonth=heartsThisMonth WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationRacesStarted`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s races started data'
BEGIN
	DECLARE racesStarted, racesStartedThisWeek, racesStartedThisMonth INT DEFAULT 0;
	
	SELECT COUNT(*) INTO racesStarted FROM PlayerCreationRacesStarted WHERE PlayerCreationId=creationId;
	SELECT COUNT(*) INTO racesStartedThisWeek FROM PlayerCreationRacesStarted WHERE PlayerCreationId=creationId AND StartedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO racesStartedThisMonth FROM PlayerCreationRacesStarted WHERE PlayerCreationId=creationId AND StartedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-(DAYOFMONTH(CURRENT_TIMESTAMP(6))-1)) DAY);
	
	IF(racesStarted IS NULL) THEN
		SET racesStarted = 0;
	END IF;
	
	IF(racesStartedThisWeek IS NULL) THEN
		SET racesStartedThisWeek = 0;
	END IF;
	
	IF(racesStartedThisMonth IS NULL) THEN
		SET racesStartedThisMonth = 0;
	END IF;
	
	UPDATE PlayerCreations SET RacesStarted=racesStarted, RacesStartedThisWeek=racesStartedThisWeek, RacesStartedThisMonth=racesStartedThisMonth WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationCoolness`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s coolness data'
BEGIN
	DECLARE ratingUpData, ratingDownData, racesStartedData, racesFinishedData, heartsData INT DEFAULT 0;
    DECLARE coolness FLOAT DEFAULT 0;
   
    SELECT RatingUp, RatingDown, RacesStarted, RacesFinished, Hearts INTO ratingUpData, ratingDownData, racesStartedData, racesFinishedData, heartsData FROM PlayerCreations WHERE PlayerCreationId=creationId;
   
    IF(ratingUpData IS NULL) THEN
		SET ratingUpData = 0;
	END IF;
   
    IF(ratingDownData IS NULL) THEN
		SET ratingDownData = 0;
	END IF;
   
    IF(racesStartedData IS NULL) THEN
		SET racesStartedData = 0;
	END IF;
   
    IF(racesFinishedData IS NULL) THEN
		SET racesFinishedData = 0;
	END IF;
   
    SET coolness = (ratingUpData - ratingDownData) + ((racesStartedData + racesFinishedData) / 2) + heartsData;
   
	UPDATE PlayerCreations SET Coolness=coolness WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationUniqueRacers`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s unique racers data'
BEGIN
	DECLARE uniqueRacers, versionData INT DEFAULT 0;
	
	SELECT Version INTO versionData FROM PlayerCreations WHERE PlayerCreationId=creationId;

	IF(versionData IS NULL) THEN
		SET versionData = 0;
	END IF;

	SELECT COUNT(*) INTO uniqueRacers FROM PlayerCreationUniqueRacers WHERE PlayerCreationId=creationId AND Version=versionData;
   	
	IF(uniqueRacers IS NULL) THEN
		SET uniqueRacers = 0;
	END IF;
	
	UPDATE PlayerCreations SET UniqueRacers=uniqueRacers WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationRating`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s rating data'
BEGIN
	DECLARE MNR TINYINT(1) DEFAULT 0;
	DECLARE votes, ratingDown, ratingUp, ratingUpThisWeek, ratingUpThisMonth INT DEFAULT 0;
    DECLARE starRating FLOAT DEFAULT 0;
   
    SELECT IsMNR INTO MNR FROM PlayerCreations WHERE PlayerCreationId=creationId;
   
    IF(MNR IS NULL) THEN
		SET MNR = 0;
	END IF;
	
    IF(MNR=1) THEN
		SELECT COUNT(*) INTO votes FROM PlayerCreationRatings WHERE PlayerCreationId=creationId AND Rating != 0;
    ELSE
   		SELECT COUNT(*) INTO votes FROM PlayerCreationRatings WHERE PlayerCreationId=creationId;
	END IF;
   
    SELECT COUNT(*) INTO ratingDown FROM PlayerCreationRatings WHERE PlayerCreationId=creationId AND Type=0;
    SELECT COUNT(*) INTO ratingUp FROM PlayerCreationRatings WHERE PlayerCreationId=creationId AND Type=1;
	SELECT COUNT(*) INTO ratingUpThisWeek FROM PlayerCreationRatings WHERE PlayerCreationId=creationId AND Type=1 AND RatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO ratingUpThisMonth FROM PlayerCreationRatings WHERE PlayerCreationId=creationId AND Type=1 AND RatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-(DAYOFMONTH(CURRENT_TIMESTAMP(6))-1)) DAY);
	SELECT CAST(AVG(Rating) AS FLOAT) INTO starRating FROM PlayerCreationRatings WHERE PlayerCreationId=creationId;
   
	IF(votes IS NULL) THEN
		SET votes = 0;
	END IF;
	
    IF(ratingDown IS NULL) THEN
		SET ratingDown = 0;
	END IF;
   
    IF(ratingUp IS NULL) THEN
		SET ratingUp = 0;
	END IF;
   
    IF(ratingUpThisWeek IS NULL) THEN
		SET ratingUpThisWeek = 0;
	END IF;
   
    IF(ratingUpThisMonth IS NULL) THEN
		SET ratingUpThisMonth = 0;
	END IF;
   
    IF(starRating IS NULL) THEN
		SET starRating = 0;
	END IF;
   
	UPDATE PlayerCreations SET Votes=votes, RatingDown=ratingDown, RatingUp=ratingUp, RatingUpThisWeek=ratingUpThisWeek, RatingUpThisMonth=ratingUpThisMonth, Rating=starRating WHERE PlayerCreationId=creationId;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserOnlineRaces`(
	IN `user` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s online races data'
BEGIN
	DECLARE onlineRaces, onlineRacesThisWeek, onlineRacesLastWeek INT DEFAULT 0;
	
	SELECT COUNT(*) INTO onlineRaces FROM OnlineRacesStarted WHERE PlayerId=user;
	SELECT COUNT(*) INTO onlineRacesThisWeek FROM OnlineRacesStarted WHERE PlayerId=user AND StartedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO onlineRacesLastWeek FROM OnlineRacesStarted WHERE PlayerId=user AND StartedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND StartedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(onlineRaces IS NULL) THEN
		SET onlineRaces = 0;
	END IF;
	
	IF(onlineRacesThisWeek IS NULL) THEN
		SET onlineRacesThisWeek = 0;
	END IF;
	
	IF(onlineRacesLastWeek IS NULL) THEN
		SET onlineRacesLastWeek = 0;
	END IF;
	
	UPDATE Users SET OnlineRaces=onlineRaces, OnlineRacesThisWeek=onlineRacesThisWeek, OnlineRacesLastWeek=onlineRacesLastWeek WHERE UserId=user;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserOnlineFinished`(
	IN `user` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s online finished data'
BEGIN
	DECLARE onlineFinished, onlineFinishedThisWeek, onlineFinishedLastWeek INT DEFAULT 0;
	
	SELECT COUNT(*) INTO onlineFinished FROM OnlineRacesFinished WHERE PlayerId=user;
	SELECT COUNT(*) INTO onlineFinishedThisWeek FROM OnlineRacesFinished WHERE PlayerId=user AND FinishedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO onlineFinishedLastWeek FROM OnlineRacesFinished WHERE PlayerId=user AND FinishedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND FinishedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(onlineFinished IS NULL) THEN
		SET onlineFinished = 0;
	END IF;
	
	IF(onlineFinishedThisWeek IS NULL) THEN
		SET onlineFinishedThisWeek = 0;
	END IF;
	
	IF(onlineFinishedLastWeek IS NULL) THEN
		SET onlineFinishedLastWeek = 0;
	END IF;
	
	UPDATE Users SET OnlineFinished=onlineFinished, OnlineFinishedThisWeek=onlineFinishedThisWeek, OnlineFinishedLastWeek=onlineFinishedLastWeek WHERE UserId=user;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserOnlineWins`(
	IN `user` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s online wins data'
BEGIN
	DECLARE onlineWins, onlineWinsThisWeek, onlineWinsLastWeek INT DEFAULT 0;
	
	SELECT COUNT(*) INTO onlineWins FROM OnlineRacesFinished WHERE PlayerId=user AND IsWinner=1;
	SELECT COUNT(*) INTO onlineWinsThisWeek FROM OnlineRacesFinished WHERE PlayerId=user AND IsWinner=1 AND FinishedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT COUNT(*) INTO onlineWinsLastWeek FROM OnlineRacesFinished WHERE PlayerId=user AND IsWinner=1 AND FinishedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND FinishedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(onlineWins IS NULL) THEN
		SET onlineWins = 0;
	END IF;
	
	IF(onlineWinsThisWeek IS NULL) THEN
		SET onlineWinsThisWeek = 0;
	END IF;
	
	IF(onlineWinsLastWeek IS NULL) THEN
		SET onlineWinsLastWeek = 0;
	END IF;
	
	UPDATE Users SET OnlineWins=onlineWins, OnlineWinsThisWeek=onlineWinsThisWeek, OnlineWinsLastWeek=onlineWinsLastWeek WHERE UserId=user;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserPoints`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s points data'
BEGIN
	DECLARE points, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT AVG(Amount) INTO points FROM PlayerPoints WHERE PlayerId=user AND Platform=forPlatform;
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerPoints WHERE PlayerId=user AND Platform=forPlatform AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerPoints WHERE PlayerId=user AND Platform=forPlatform AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
   
	IF(points<0) THEN
   		SET points=0;
	END IF;
   
	IF(points>3000) THEN
   		SET points=3000;
	END IF;
	
	IF(forPlatform=4) THEN
		UPDATE Users SET PointsPSV=points, PointsThisWeekPSV=pointsThisWeek, PointsLastWeekPSV=pointsLastWeek WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET PointsPSP=points, PointsThisWeekPSP=pointsThisWeek, PointsLastWeekPSP=pointsLastWeek WHERE UserId=user;
	ELSE
   		UPDATE Users SET PointsPS3=points, PointsThisWeekPS3=pointsThisWeek, PointsLastWeekPS3=pointsLastWeek WHERE UserId=user;
	END IF;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserTotalTracks`(
	IN `user` INT,
	IN `forPlatform` INT,
	IN `MNR` TINYINT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s total tracks data'
BEGIN
	DECLARE totalTracks INT DEFAULT 0;
	
	SELECT COUNT(*) INTO totalTracks FROM PlayerCreations WHERE PlayerId=user AND Type=2 AND Platform=forPlatform AND IsMNR=MNR AND ModerationStatus!=1204 AND ModerationStatus!=1205;
	
	IF(totalTracks IS NULL) THEN
		SET totalTracks = 0;
	END IF;
	
	IF(MNR=0) THEN
		UPDATE Users SET TotalTracks=totalTracks WHERE UserId=user;
	ELSEIF(forPlatform=4) THEN
   		UPDATE Users SET TotalMNRTracksPSV=totalTracks WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET TotalMNRTracksPSP=totalTracks WHERE UserId=user;
	ELSE
   		UPDATE Users SET TotalMNRTracksPS3=totalTracks WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserTotalCharacters`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s total characters data'
BEGIN
	DECLARE totalCharacters INT DEFAULT 0;
	
	SELECT COUNT(*) INTO totalCharacters FROM PlayerCreations WHERE PlayerId=user AND Type=6 AND Platform=forPlatform AND ModerationStatus!=1204 AND ModerationStatus!=1205;
	
	IF(totalCharacters IS NULL) THEN
		SET totalCharacters = 0;
	END IF;
	
	IF(forPlatform=4) THEN
   		UPDATE Users SET TotalCharactersPSV=totalCharacters WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET TotalCharactersPSP=totalCharacters WHERE UserId=user;
	ELSE
   		UPDATE Users SET TotalCharactersPS3=totalCharacters WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserTotalKarts`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s total karts data'
BEGIN
	DECLARE totalKarts INT DEFAULT 0;
	
	SELECT COUNT(*) INTO totalKarts FROM PlayerCreations WHERE PlayerId=user AND Type=7 AND Platform=forPlatform AND ModerationStatus!=1204 AND ModerationStatus!=1205;
	
	IF(totalKarts IS NULL) THEN
		SET totalKarts = 0;
	END IF;
	
	IF(forPlatform=4) THEN
   		UPDATE Users SET TotalKartsPSV=totalKarts WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET TotalKartsPSP=totalKarts WHERE UserId=user;
	ELSE
   		UPDATE Users SET TotalKartsPS3=totalKarts WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserCharacterCreatorPoints`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s character creator points data'
BEGIN
	DECLARE points, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT SUM(Amount) INTO points FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=6;
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=6 AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=6 AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
	
	IF(forPlatform=4) THEN
		UPDATE Users SET CharacterCreatorPointsPSV=points, CharacterCreatorPointsThisWeekPSV=pointsThisWeek, CharacterCreatorPointsLastWeekPSV=pointsLastWeek WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET CharacterCreatorPointsPSP=points, CharacterCreatorPointsThisWeekPSP=pointsThisWeek, CharacterCreatorPointsLastWeekPSP=pointsLastWeek WHERE UserId=user;
	ELSE
   		UPDATE Users SET CharacterCreatorPointsPS3=points, CharacterCreatorPointsThisWeekPS3=pointsThisWeek, CharacterCreatorPointsLastWeekPS3=pointsLastWeek WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserKartCreatorPoints`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s kart creator points data'
BEGIN
	DECLARE points, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT SUM(Amount) INTO points FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=7;
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=7 AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=7 AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
	
	IF(forPlatform=4) THEN
		UPDATE Users SET KartCreatorPointsPSV=points, KartCreatorPointsThisWeekPSV=pointsThisWeek, KartCreatorPointsLastWeekPSV=pointsLastWeek WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET KartCreatorPointsPSP=points, KartCreatorPointsThisWeekPSP=pointsThisWeek, KartCreatorPointsLastWeekPSP=pointsLastWeek WHERE UserId=user;
	ELSE
   		UPDATE Users SET KartCreatorPointsPS3=points, KartCreatorPointsThisWeekPS3=pointsThisWeek, KartCreatorPointsLastWeekPS3=pointsLastWeek WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserTrackCreatorPoints`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s track creator points data'
BEGIN
	DECLARE points, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT SUM(Amount) INTO points FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=2;
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=2 AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerCreationPoints WHERE PlayerId=user AND Platform=forPlatform AND Type=2 AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
	
	IF(forPlatform=4) THEN
		UPDATE Users SET TrackCreatorPointsPSV=points, TrackCreatorPointsThisWeekPSV=pointsThisWeek, TrackCreatorPointsLastWeekPSV=pointsLastWeek WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET TrackCreatorPointsPSP=points, TrackCreatorPointsThisWeekPSP=pointsThisWeek, TrackCreatorPointsLastWeekPSP=pointsLastWeek WHERE UserId=user;
	ELSE
   		UPDATE Users SET TrackCreatorPointsPS3=points, TrackCreatorPointsThisWeekPS3=pointsThisWeek, TrackCreatorPointsLastWeekPS3=pointsLastWeek WHERE UserId=user;
	END IF;
END");
            
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateUserExperiencePoints`(
	IN `user` INT,
	IN `forPlatform` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates user\'s experience points data'
BEGIN
	DECLARE points, pointsThisWeek, pointsLastWeek FLOAT DEFAULT 0;
	
	SELECT SUM(Amount) INTO points FROM PlayerExperiencePoints WHERE PlayerId=user AND Platform=forPlatform;
	SELECT SUM(Amount) INTO pointsThisWeek FROM PlayerExperiencePoints WHERE PlayerId=user AND Platform=forPlatform AND CreatedAt > DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	SELECT SUM(Amount) INTO pointsLastWeek FROM PlayerExperiencePoints WHERE PlayerId=user AND Platform=forPlatform AND CreatedAt > DATE_ADD(DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY), INTERVAL -1 WEEK) AND CreatedAt < DATE_ADD(DATE(CURRENT_TIMESTAMP(6)), INTERVAL(-WEEKDAY(CURRENT_TIMESTAMP(6))) DAY);
	
	IF(points IS NULL) THEN
		SET points = 0;
	END IF;
	
	IF(pointsThisWeek IS NULL) THEN
		SET pointsThisWeek = 0;
	END IF;
	
	IF(pointsLastWeek IS NULL) THEN
		SET pointsLastWeek = 0;
	END IF;
	
	IF(forPlatform=4) THEN
		UPDATE Users SET ExperiencePointsPSV=points, ExperiencePointsThisWeekPSV=pointsThisWeek, ExperiencePointsLastWeekPSV=pointsLastWeek WHERE UserId=user;
	ELSEIF(forPlatform=1) THEN
   		UPDATE Users SET ExperiencePointsPSP=points, ExperiencePointsThisWeekPSP=pointsThisWeek, ExperiencePointsLastWeekPSP=pointsLastWeek WHERE UserId=user;
	ELSE
   		UPDATE Users SET ExperiencePointsPS3=points, ExperiencePointsThisWeekPS3=pointsThisWeek, ExperiencePointsLastWeekPS3=pointsLastWeek WHERE UserId=user;
	END IF;
END");
            
			migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationsUsername`(
	IN `userId` INT,
	IN `username` LONGTEXT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Update username for a specified user'
BEGIN
	UPDATE PlayerCreations SET Username=username WHERE PlayerId=userId;
END");

			migrationBuilder.Sql(@"CREATE PROCEDURE `UpdateScoresUsername`(
	IN `userId` INT,
	IN `username` LONGTEXT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Update username for a specified user'
BEGIN
	UPDATE Scores SET Username=username WHERE PlayerId=userId;
END");
			
			migrationBuilder.Sql(@"CREATE PROCEDURE `RefreshAll`()
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Refresh all calculated params'
BEGIN
	FOR user IN (SELECT UserId, Username FROM Users) DO
		CALL UpdatePlayerCreationsUsername(user.UserId, user.Username);
		CALL UpdateScoresUsername(user.UserId, user.Username);
		CALL UpdateUserOnlineRaces(user.UserId);
		CALL UpdateUserOnlineFinished(user.UserId);
		CALL UpdateUserOnlineWins(user.UserId);
		CALL UpdateUserPoints(user.UserId, 1);
		CALL UpdateUserPoints(user.UserId, 2);
		CALL UpdateUserPoints(user.UserId, 4);
		CALL UpdateUserTotalTracks(user.UserId, 2, 0);
		CALL UpdateUserTotalTracks(user.UserId, 1, 1);
		CALL UpdateUserTotalTracks(user.UserId, 2, 1);
		CALL UpdateUserTotalTracks(user.UserId, 4, 1);
		CALL UpdateUserTotalCharacters(user.UserId, 1);
		CALL UpdateUserTotalCharacters(user.UserId, 2);
		CALL UpdateUserTotalCharacters(user.UserId, 4);
		CALL UpdateUserTotalKarts(user.UserId, 1);
		CALL UpdateUserTotalKarts(user.UserId, 2);
		CALL UpdateUserTotalKarts(user.UserId, 4);
		CALL UpdateUserCharacterCreatorPoints(user.UserId, 1);
		CALL UpdateUserCharacterCreatorPoints(user.UserId, 2);
		CALL UpdateUserCharacterCreatorPoints(user.UserId, 4);
		CALL UpdateUserKartCreatorPoints(user.UserId, 1);
		CALL UpdateUserKartCreatorPoints(user.UserId, 2);
		CALL UpdateUserKartCreatorPoints(user.UserId, 4);
		CALL UpdateUserTrackCreatorPoints(user.UserId, 1);
		CALL UpdateUserTrackCreatorPoints(user.UserId, 2);
		CALL UpdateUserTrackCreatorPoints(user.UserId, 4);
		CALL UpdateUserExperiencePoints(user.UserId, 1);
		CALL UpdateUserExperiencePoints(user.UserId, 2);
		CALL UpdateUserExperiencePoints(user.UserId, 4);
	END FOR;
	
	FOR creation IN (SELECT PlayerCreationId FROM PlayerCreations) DO
		CALL UpdatePlayerCreationPoints(creation.PlayerCreationId);
		CALL UpdatePlayerCreationDownloads(creation.PlayerCreationId);
		CALL UpdatePlayerCreationViews(creation.PlayerCreationId);
		CALL UpdatePlayerCreationHearts(creation.PlayerCreationId);
		CALL UpdatePlayerCreationRacesStarted(creation.PlayerCreationId);
		CALL UpdatePlayerCreationUniqueRacers(creation.PlayerCreationId);
		CALL UpdatePlayerCreationRating(creation.PlayerCreationId);
		CALL UpdatePlayerCreationCoolness(creation.PlayerCreationId);
	END FOR;
END");
			
			//Triggers
			migrationBuilder.Sql(@"CREATE TRIGGER `BeforeUsersUpdate` BEFORE UPDATE ON `Users` FOR EACH ROW BEGIN
	SET NEW.CreatorPointsPSV = NEW.CharacterCreatorPointsPSV+NEW.KartCreatorPointsPSV+NEW.TrackCreatorPointsPSV;
	SET NEW.CreatorPointsPSP = NEW.CharacterCreatorPointsPSP+NEW.KartCreatorPointsPSP+NEW.TrackCreatorPointsPSP;
   	SET NEW.CreatorPointsPS3 = NEW.CharacterCreatorPointsPS3+NEW.KartCreatorPointsPS3+NEW.TrackCreatorPointsPS3;
   	SET NEW.CreatorPointsThisWeekPSV = NEW.CharacterCreatorPointsThisWeekPSV+NEW.KartCreatorPointsThisWeekPSV+NEW.TrackCreatorPointsThisWeekPSV;
   	SET NEW.CreatorPointsThisWeekPSP = NEW.CharacterCreatorPointsThisWeekPSP+NEW.KartCreatorPointsThisWeekPSP+NEW.TrackCreatorPointsThisWeekPSP;
   	SET NEW.CreatorPointsThisWeekPS3 = NEW.CharacterCreatorPointsThisWeekPS3+NEW.KartCreatorPointsThisWeekPS3+NEW.TrackCreatorPointsThisWeekPS3;
   	SET NEW.CreatorPointsLastWeekPSV = NEW.CharacterCreatorPointsLastWeekPSV+NEW.KartCreatorPointsLastWeekPSV+NEW.TrackCreatorPointsLastWeekPSV;
   	SET NEW.CreatorPointsLastWeekPSP = NEW.CharacterCreatorPointsLastWeekPSP+NEW.KartCreatorPointsLastWeekPSP+NEW.TrackCreatorPointsLastWeekPSP;
   	SET NEW.CreatorPointsLastWeekPS3 = NEW.CharacterCreatorPointsLastWeekPS3+NEW.KartCreatorPointsLastWeekPS3+NEW.TrackCreatorPointsLastWeekPS3;
   	
   	SET NEW.TotalXPPSV = NEW.CreatorPointsPSV+NEW.ExperiencePointsPSV;
   	SET NEW.TotalXPPSP = NEW.CreatorPointsPSP+NEW.ExperiencePointsPSP;
   	SET NEW.TotalXPPS3 = NEW.CreatorPointsPS3+NEW.ExperiencePointsPS3;
   	SET NEW.TotalXPThisWeekPSV = NEW.CreatorPointsThisWeekPSV+NEW.ExperiencePointsThisWeekPSV;
   	SET NEW.TotalXPThisWeekPSP = NEW.CreatorPointsThisWeekPSP+NEW.ExperiencePointsThisWeekPSP;
   	SET NEW.TotalXPThisWeekPS3 = NEW.CreatorPointsThisWeekPS3+NEW.ExperiencePointsThisWeekPS3;
   	SET NEW.TotalXPLastWeekPSV = NEW.CreatorPointsLastWeekPSV+NEW.ExperiencePointsLastWeekPSV;
   	SET NEW.TotalXPLastWeekPSP = NEW.CreatorPointsLastWeekPSP+NEW.ExperiencePointsLastWeekPSP;
   	SET NEW.TotalXPLastWeekPS3 = NEW.CreatorPointsLastWeekPS3+NEW.ExperiencePointsLastWeekPS3;	
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterUsersUpdate` AFTER UPDATE ON `Users` FOR EACH ROW BEGIN
	IF (NEW.Username != OLD.Username) THEN
		CALL UpdateScoresUsername(NEW.UserId, NEW.Username);
		CALL UpdatePlayerCreationsUsername(NEW.UserId, NEW.Username);
	END IF;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationsInsert` AFTER INSERT ON `PlayerCreations` FOR EACH ROW BEGIN
	IF (NEW.Type = 7) THEN
		CALL UpdateUserTotalKarts(NEW.PlayerId, NEW.Platform);
	ELSEIF (NEW.Type = 6) THEN
		CALL UpdateUserTotalCharacters(NEW.PlayerId, NEW.Platform);
	ELSEIF (NEW.Type = 2) THEN
		CALL UpdateUserTotalTracks(NEW.PlayerId, NEW.Platform, NEW.IsMNR);
	END IF;
END");

			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationsUpdate` AFTER UPDATE ON `PlayerCreations` FOR EACH ROW BEGIN
	IF (NEW.RatingUp != OLD.RatingUp OR NEW.RatingDown != OLD.RatingDown OR NEW.RacesStarted != OLD.RacesStarted OR NEW.RacesFinished != OLD.RacesFinished OR NEW.Hearts != OLD.Hearts) THEN
		CALL UpdatePlayerCreationCoolness(NEW.PlayerCreationId);
	ELSEIF (NEW.Type != OLD.Type OR NEW.ModerationStatus != OLD.ModerationStatus) THEN
		IF (OLD.Type = 7) THEN
			CALL UpdateUserTotalKarts(NEW.PlayerId, NEW.Platform);
		ELSEIF (OLD.Type = 6) THEN
			CALL UpdateUserTotalCharacters(NEW.PlayerId, NEW.Platform);
		ELSEIF (OLD.Type = 2) THEN
			CALL UpdateUserTotalTracks(NEW.PlayerId, NEW.Platform, NEW.IsMNR);
		END IF;
	END IF;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationsDelete` AFTER DELETE ON `PlayerCreations` FOR EACH ROW BEGIN
	IF (OLD.Type = 7) THEN
		CALL UpdateUserTotalKarts(OLD.PlayerId, OLD.Platform);
	ELSEIF (OLD.Type = 6) THEN
		CALL UpdateUserTotalCharacters(OLD.PlayerId, OLD.Platform);
	ELSEIF (OLD.Type = 2) THEN
		CALL UpdateUserTotalTracks(OLD.PlayerId, OLD.Platform, OLD.IsMNR);
	END IF;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationPointsInsert` AFTER INSERT ON `PlayerCreationPoints` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationPoints(NEW.PlayerCreationId);

	IF (NEW.Type = 7) THEN
		CALL UpdateUserKartCreatorPoints(NEW.PlayerId, NEW.Platform);
	ELSEIF (NEW.Type = 6) THEN
		CALL UpdateUserCharacterCreatorPoints(NEW.PlayerId, NEW.Platform);
	ELSE
		CALL UpdateUserTrackCreatorPoints(NEW.PlayerId, NEW.Platform);
	END IF;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationPointsDelete` AFTER DELETE ON `PlayerCreationPoints` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationPoints(OLD.PlayerCreationId);

	IF (OLD.Type = 7) THEN
		CALL UpdateUserKartCreatorPoints(OLD.PlayerId, OLD.Platform);
	ELSEIF (OLD.Type = 6) THEN
		CALL UpdateUserCharacterCreatorPoints(OLD.PlayerId, OLD.Platform);
	ELSE
		CALL UpdateUserTrackCreatorPoints(OLD.PlayerId, OLD.Platform);
	END IF;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationDownloadsInsert` AFTER INSERT ON `PlayerCreationDownloads` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationDownloads(NEW.PlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationViewsInsert` AFTER INSERT ON `PlayerCreationViews` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationViews(NEW.PlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationHeartsInsert` AFTER INSERT ON `HeartedPlayerCreations` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationHearts(NEW.HeartedPlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationRacesStartedInsert` AFTER INSERT ON `PlayerCreationRacesStarted` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationRacesStarted(NEW.PlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationUniqueRacersInsert` AFTER INSERT ON `PlayerCreationUniqueRacers` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationUniqueRacers(NEW.PlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationRatingsInsert` AFTER INSERT ON `PlayerCreationRatings` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationRating(NEW.PlayerCreationId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterExperiencePointsInsert` AFTER INSERT ON `PlayerExperiencePoints` FOR EACH ROW BEGIN
	CALL UpdateUserExperiencePoints(NEW.PlayerId, NEW.Platform);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterPointsInsert` AFTER INSERT ON `PlayerPoints` FOR EACH ROW BEGIN
	CALL UpdateUserPoints(NEW.PlayerId, NEW.Platform);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterRacesStartedInsert` AFTER INSERT ON `OnlineRacesStarted` FOR EACH ROW BEGIN
	CALL UpdateUserOnlineRaces(NEW.PlayerId);
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterRacesFinishedInsert` AFTER INSERT ON `OnlineRacesFinished` FOR EACH ROW BEGIN
	CALL UpdateUserOnlineFinished(NEW.PlayerId);

	IF (NEW.IsWinner=1) THEN
		CALL UpdateUserOnlineWins(NEW.PlayerId);
	END IF;
END");
			
			//Events
			migrationBuilder.Sql(@"CREATE EVENT `DailyRefresh`
	ON SCHEDULE
		EVERY 1 DAY STARTS '2026-09-24'
	ON COMPLETION PRESERVE
	ENABLE
	COMMENT 'Refresh all calculated params each day'
	DO CALL RefreshAll()");

			migrationBuilder.Sql("CALL RefreshAll()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {//TODO: drop whatever was created
	        migrationBuilder.Sql("DROP EVENT `DailyRefresh`");
	        migrationBuilder.Sql("DROP TRIGGER `BeforeUsersUpdate`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterUsersUpdate`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationsUpdate`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationsDelete`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationPointsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationPointsDelete`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationDownloadsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationViewsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationHeartsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationRacesStartedInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationUniqueRacersInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterCreationRatingsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterExperiencePointsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterPointsInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterRacesStartedInsert`");
	        migrationBuilder.Sql("DROP TRIGGER `AfterRacesFinishedInsert`");
	        migrationBuilder.Sql("DROP PROCEDURE `RefreshAll`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateScoresUsername`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationsUsername`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationRacesStarted`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationHearts`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationViews`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationDownloads`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationPoints`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationCoolness`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationUniqueRacers`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdatePlayerCreationRating`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserOnlineRaces`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserOnlineFinished`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserOnlineWins`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserPoints`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserTotalTracks`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserTotalCharacters`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserTotalKarts`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserCharacterCreatorPoints`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserKartCreatorPoints`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserTrackCreatorPoints`");
	        migrationBuilder.Sql("DROP PROCEDURE `UpdateUserExperiencePoints`");
        }
    }
}
