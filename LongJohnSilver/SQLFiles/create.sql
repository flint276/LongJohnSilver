CREATE TABLE contenders(id INT PRIMARY KEY, name VARCHAR(200), score INT, killer VARCHAR(50), epitaph VARCHAR(200), channel VARCHAR(50));
CREATE TABLE knockout(id INT PRIMARY KEY, name VARCHAR(200), status INT, owner VARCHAR(50), channel VARCHAR(50));
CREATE TABLE kplayers(id INT PRIMARY KEY, playerid VARCHAR(30), turnsleft INT, lastplayed INT, channel VARCHAR(50));
CREATE TABLE channelperms(id INT PRIMARY KEY, guild VARCHAR(200), channel VARCHAR(200), rolename VARCHAR(50));
CREATE TABLE version(ver INT);