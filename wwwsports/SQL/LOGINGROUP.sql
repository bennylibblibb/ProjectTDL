CREATE TABLE LOGINGROUP 
(
  IGP_ID	INTEGER NOT NULL,
  CDESC	VARCHAR(20) NOT NULL,
 UNIQUE (CDESC),
 PRIMARY KEY (IGP_ID)
);

insert into LOGINGROUP values (0, '�Ҧ���T');
insert into LOGINGROUP values (1, '���y�ɵ{');
insert into LOGINGROUP values (2, '���y��T');
insert into LOGINGROUP values (3, '��L�a��');
insert into LOGINGROUP values (4, '�ɰ���T');
insert into LOGINGROUP values (5, '��L��T');
insert into LOGINGROUP values (6, '�{���߲v');
insert into LOGINGROUP values (7, 'JC�����m');
insert into LOGINGROUP values (8, '�x�y��T');