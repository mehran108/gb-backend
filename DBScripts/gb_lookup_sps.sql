--************ STORE PROCEDURES ****************


--************ LOOK VALUE ****************

--Add lookup table
DROP PROCEDURE IF EXISTS AddLookupTable;
DELIMITER $$

CREATE PROCEDURE AddLookupTable(
    IN p_code VARCHAR(100),
    IN p_description TEXT
)
BEGIN
    INSERT INTO lookupTable_gb (code, description)
    VALUES (p_code, p_description);
END $$
DELIMITER ;

--Update lookup table

DROP PROCEDURE IF EXISTS UpdateLookupTable;
DELIMITER $$

CREATE PROCEDURE UpdateLookupTable(
    IN p_id INT,
    IN p_code VARCHAR(100),
    IN p_description TEXT
)
BEGIN
    UPDATE lookupTable_gb
    SET code = p_code,
        description = p_description
    WHERE id = p_id;
END $$
DELIMITER ;

--GEt lookup table
DROP PROCEDURE IF EXISTS GetLookupTable;
DELIMITER $$

CREATE PROCEDURE GetLookupTable()
BEGIN
    SELECT id, code, description FROM lookupTable_gb;
END $$

DELIMITER ;

--Get lookup Table by id
DROP PROCEDURE IF EXISTS GetLookupTableById;
DELIMITER $$

CREATE PROCEDURE GetLookupTableById(
    IN p_id INT
)
BEGIN
    SELECT id, code, description
    FROM lookupTable_gb
    WHERE id = p_id;
END $$

DELIMITER ;

--************ LOOK VALUE ****************
DROP PROCEDURE IF EXISTS AddLookupValue;
DELIMITER $$

CREATE PROCEDURE AddLookupValue(
    IN p_lookupTableId int,
    IN p_lookupValueCode TEXT,
    IN p_description TEXT,
    IN p_userid INT,
    IN p_extra MEDIUMTEXT
)
BEGIN
    INSERT INTO lookupValue_gb (
        lookupTableId, lookupValueCode, description,createdById,extra
    )
    VALUES (
        p_lookupTableId, p_lookupValueCode, p_description,p_userid,p_extra
    );
END $$
DELIMITER ;

DROP PROCEDURE IF EXISTS UpdateLookupValue;
DELIMITER $$

CREATE PROCEDURE UpdateLookupValue(
    IN p_id INT,
    IN p_lookupTableId VARCHAR(100),
    IN p_lookupValueCode VARCHAR(100),
    IN p_description TEXT,
    IN p_userid INT,
    IN p_extra MEDIUMTEXT
)
BEGIN
    UPDATE lookupValue_gb
    SET
        lookupTableId = p_lookupTableId,
        lookupValueCode = p_lookupValueCode,
        description = p_description,
        updatedById = p_userid,
        updatedDate = current_timestamp()
    WHERE id = p_id;
END $$
DELIMITER ;

DROP PROCEDURE IF EXISTS GetLookupValueByCode;
DELIMITER $$

CREATE PROCEDURE GetLookupValueByCode(
    IN p_lookupValueCode VARCHAR(100)
)
BEGIN
    SELECT *
    FROM lookupValue_gb
    WHERE lookupValueCode = p_lookupValueCode;
END $$
DELIMITER ;

DROP PROCEDURE IF EXISTS GetLookupValueByCode;
DELIMITER $$

CREATE PROCEDURE GetLookupValueByCode(
    IN p_lookupValueCode TEXT
)
BEGIN
    SELECT *
    FROM lookupValue_gb
    WHERE lookupValueCode = p_lookupValueCode;
END $$
DELIMITER ;

