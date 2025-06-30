CREATE TABLE lookupTable_gb (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    code TEXT NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE lookupValue_gb (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    lookupTableId TEXT NOT NULL,
	lookupValueCode TEXT NOT NULL,
    description TEXT NOT NULL,
    createdById int not null,
    createdDate datetime null,
    updatedById int not null,
    updatedDate int null,
    active bit not null,
    extra MEDIUMTEXT null
);







