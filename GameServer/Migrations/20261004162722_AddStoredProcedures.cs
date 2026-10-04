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
            migrationBuilder.Sql(@"CREATE PROCEDURE `UpdatePlayerCreationPoints`(
	IN `creationId` INT
)
LANGUAGE SQL
NOT DETERMINISTIC
MODIFIES SQL DATA
SQL SECURITY DEFINER
COMMENT 'Updates player creation\'s points data'
BEGIN
	DECLARE points, pointsToday, pointsYesterday, pointsThisWeek, pointsLastWeek INT DEFAULT 0;
	
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
	END FOR;
	
	FOR creation IN (SELECT PlayerCreationId FROM PlayerCreations) DO
		CALL UpdatePlayerCreationPoints(creation.PlayerCreationId);
	END FOR;
END");
			
			migrationBuilder.Sql(@"CREATE TRIGGER `AfterUsersUpdate` AFTER UPDATE ON `Users` FOR EACH ROW BEGIN
	IF (NEW.Username != OLD.Username) THEN
		CALL UpdateScoresUsername(NEW.UserId, NEW.Username);
		CALL UpdatePlayerCreationsUsername(NEW.UserId, NEW.Username);
	END IF;
END");

			migrationBuilder.Sql(@"CREATE TRIGGER `AfterCreationPointsInsert` AFTER INSERT ON `PlayerCreationPoints` FOR EACH ROW BEGIN
	CALL UpdatePlayerCreationPoints(NEW.PlayerCreationId);
END");

			migrationBuilder.Sql(@"CREATE EVENT `DailyRefresh`
	ON SCHEDULE
		EVERY 1 DAY STARTS '2026-09-24'
	ON COMPLETION PRESERVE
	ENABLE
	COMMENT 'Refresh all calculated params each day'
	DO CALL RefreshAll()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {//TODO: drop whatever was created
        }
    }
}
