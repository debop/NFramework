CREATE DEFINER=`root`@`localhost` PROCEDURE `CityByCountry`(IN p_CountryCode CHAR(3))
    READS SQL DATA
    COMMENT 'Retrieve Cities by CountryCode'
BEGIN
    SELECT * 
      FROM world.city
     WHERE CountryCode = p_CountryCode;
END

