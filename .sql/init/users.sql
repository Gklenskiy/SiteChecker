CREATE TABLE ApplicationUser (
    Id                 INTEGER PRIMARY KEY,
    UserName           TEXT    NOT NULL,
    PasswordHash       TEXT,
    NormalizedUserName TEXT    NOT NULL,
    Email              TEXT,
    NormalizedEmail    TEXT,
    EmailConfirmed     BOOLEAN
);


CREATE TABLE ApplicationRole (
    Id             INTEGER PRIMARY KEY,
    Name           TEXT    NOT NULL,
    NormalizedName TEXT    NOT NULL
);

CREATE TABLE ApplicationUserRole (
    UserId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    PRIMARY KEY (
        UserId,
        RoleId
    ),
    FOREIGN KEY (
        UserId
    )
    REFERENCES ApplicationUser (Id),
    FOREIGN KEY (
        RoleId
    )
    REFERENCES ApplicationRole (Id) 
);
