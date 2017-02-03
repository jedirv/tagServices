/*
.read tagBackendDummyData.sql
CREATE TABLE Tags (ID INTEGER PRIMARY KEY, Name char(300));
CREATE TABLE Persons (ID INTEGER PRIMARY KEY, Name char(200));
CREATE TABLE Resources (ID INTEGER PRIMARY KEY, Type char(20), Name char(1024), LastUse DATETIME);
CREATE TABLE Email (ID INTEGER PRIMARY KEY, EntryID char(200), ConversatioID char(100));
CREATE TABLE ResourceTags (ID INTEGER PRIMARY KEY, ResourceID INTEGER, TagID INTEGER);
CREATE TABLE PersonTags (ID INTEGER PRIMARY KEY, PersonID INTEGER, TagID INTEGER);
CREATE TABLE EmailTags (ID INTEGER PRIMARY KEY, emailID INTEGER, TagID INTEGER);
CREATE TABLE DocumentsReceived (ID INTEGER PRIMARY KEY, PersonID INTEGER, ResourceID INTEGER);
CREATE TABLE DocumentsSent (ID INTEGER PRIMARY KEY, PersonID INTEGER, ResourceID INTEGER);
CREATE TABLE EmailReceived (ID INTEGER PRIMARY KEY, PersonID INTEGER, EmailID INTEGER);
CREATE TABLE EmailSent (ID INTEGER PRIMARY KEY, PersonID INTEGER, EmailID INTEGER);
*/
INSERT INTO Tags VALUES(1,'tag1');
INSERT INTO Tags VALUES(2,'tag2');
INSERT INTO Tags VALUES(3,'tag3');
INSERT INTO Tags VALUES(4,'tag4');
INSERT INTO Tags VALUES(5,'tag5');
INSERT INTO Tags VALUES(6,'tag6');
INSERT INTO Persons VALUES(1,'Alex');
INSERT INTO Persons VALUES(2,'Brenda');
INSERT INTO Persons VALUES(3,'Carl');
INSERT INTO Persons VALUES(4,'Donna');
INSERT INTO Persons VALUES(5,'Ed');
INSERT INTO Persons VALUES(6,'Francis');
INSERT INTO Resources VALUES(1,'URL','www.allrecipes.com', '20120612 10:34:09 AM');
INSERT INTO Resources VALUES(2,'URL','www.weavesilk.com',  '20120613 10:34:09 AM');
INSERT INTO Resources VALUES(3,'URL','www.amazon.com',     '20120614 10:34:09 AM');
INSERT INTO Resources VALUES(4,'FILE','C:\file1.docx',     '20120615 10:34:09 AM');
INSERT INTO Resources VALUES(5,'FILE','C:\file2.docx',     '20120616 10:34:09 AM');
INSERT INTO Resources VALUES(6,'FILE','C:\file3.docx',     '20120617 10:34:09 AM');
INSERT INTO Resources VALUES(7,'FILE','C:\file4.docx',     '20120618 10:34:09 AM');
INSERT INTO ResourceTags VALUES(1, 1, 3);
INSERT INTO ResourceTags VALUES(2, 3, 3);
INSERT INTO ResourceTags VALUES(3, 4, 3);
INSERT INTO ResourceTags VALUES(4, 5, 3);
INSERT INTO ResourceTags VALUES(5, 6, 3);
INSERT INTO ResourceTags VALUES(6, 2, 2);
INSERT INTO ResourceTags VALUES(7, 7, 2);
INSERT INTO Emails VALUES(1, '00001', '0001');
INSERT INTO Emails VALUES(2, '00002', '0001');
INSERT INTO Emails VALUES(3, '00003', '0002');
INSERT INTO Emails VALUES(4, '00004', '0002');
INSERT INTO Emails VALUES(5, '00005', '0002');
INSERT INTO Emails VALUES(6, '00006', '0003');
INSERT INTO Emails VALUES(7, '00007', '0004');
INSERT INTO Emails VALUES(8, '00008', '0004');
INSERT INTO Emails VALUES(9, '00009', '0004');
INSERT INTO EmailTags VALUES(1, 1, 3);
INSERT INTO EmailTags VALUES(2, 1, 5);
INSERT INTO EmailTags VALUES(3, 1, 6);
INSERT INTO PersonTags VALUES(1, 1, 3);
/*
get the Emails.ID that matches the entryID given

SELECT Emails.ID from Emails WHERE Emails.EntryID=x

then get the
SELECT Tags.Name FROM Tags INNER Join EmailTags on EmailTags.tagID=Tags.ID WHERE EmailID=y
*/

/*
select all the most recent resources that are associated with this tag 

SELECT Resources.Name FROM Resources INNER JOIN ResourceTags ON Resources.ID=ResourceTags.ResourceID WHERE Resources.Type='FILE' AND ResourceTags.TagID='3' ORDER BY Resources.LastUse DESC LIMIT 2;

the user would save the file and then switch to Outlook, find the relevant email (presumably in a search folder for that tag), hit Reply, click on the tag, and there (somewhere), find the MRU files for this tag and click on the most recent file to cause it to be attached to the out-going message.

*/