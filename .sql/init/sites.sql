CREATE TABLE Sites (
    Id            INTEGER  PRIMARY KEY,
    Url           TEXT,
    CheckInterval INTEGER,
    Status        TEXT,
    LastCheck     DATETIME,
    NextCheck     DATETIME
);
