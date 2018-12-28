CREATE TABLE LOGINGROUP 
(
  IGP_ID	INTEGER NOT NULL,
  CDESC	VARCHAR(20) NOT NULL,
 UNIQUE (CDESC),
 PRIMARY KEY (IGP_ID)
);

insert into LOGINGROUP values (0, '所有資訊');
insert into LOGINGROUP values (1, '足球賽程');
insert into LOGINGROUP values (2, '足球資訊');
insert into LOGINGROUP values (3, '其他地區');
insert into LOGINGROUP values (4, '賽馬資訊');
insert into LOGINGROUP values (5, '其他資訊');
insert into LOGINGROUP values (6, '現場賠率');
insert into LOGINGROUP values (7, 'JC足智彩');
insert into LOGINGROUP values (8, '籃球資訊');