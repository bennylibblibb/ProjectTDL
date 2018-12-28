drop table SYSMONCFG;
commit;

create table SYSMONCFG
(
  ISYSMON_NO INTEGER NOT NULL,
  CDISPLAY VARCHAR(50) NOT NULL,
  CDBCONN	VARCHAR(200),
  CSQL VARCHAR(150),
  IENABLED INTEGER,
  ISHOW_DETAILS INTEGER,
  CONSTRAINT SYSMONCFG_PK PRIMARY KEY (ISYSMON_NO)
);
grant all on SYSMONCFG to public;
commit;

CREATE INDEX SYSMONIDX1 ON SYSMONCFG(ISHOW_DETAILS);
CREATE INDEX SYSMONIDX2 ON SYSMONCFG(CDISPLAY);
CREATE INDEX SYSMONIDX3 ON SYSMONCFG(CDBCONN);
CREATE INDEX SYSMONIDX4 ON SYSMONCFG(CSQL);
CREATE INDEX SYSMONIDX5 ON SYSMONCFG(IENABLED);

INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (1, 'GOGO1���y(Sender) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT FILE_NAME FROM PENDINGFILE', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (2, 'GOGO2���y(Handler 1) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports2/db/SPORTS_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT PENDINGFILE FROM PENDINGJOB order by IREC_ID', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (4, '���|���y(Sender) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT FILE_NAME FROM PENDINGFILE', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (3, 'GOGO2���y(Handler 2) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports2/db/SPORTS_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT PENDINGFILE FROM PENDINGJOB2 order by IREC_ID', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (5, '���|���y(Handler 1) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT PENDINGFILE FROM PENDINGJOB order by IREC_ID', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (6, '���|���y(Handler 2) - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT PENDINGFILE FROM PENDINGJOB2 order by IREC_ID', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (7, 'GOGO�x�y - ���ԳB�z���u�@�n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/NBASports/db/ASIA_NBA.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100', 'SELECT CFILENAME FROM JOB_QUEUE order by IRECNO', 1, 1);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (8, 'GOGO - ���ԸgCOM�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM COM_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (9, 'GOGO(3625�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM HK_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (10, 'GOGO(0375�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM CHK_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (11, 'GOGO(�D���W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM MAC_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (12, 'GOGO(���Y�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM ST_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (13, 'GOGO(8625�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM CHK2G_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (14, '���| - ���ԸgCOM�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM COM_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (15, '���|(3125�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM HK_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (16, '���|(�����W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM CHK_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (17, '���|(�D���W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM MAC_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (18, '���|(���Y�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM ST_QUEUE order by IREC_NO', 1, 0);
INSERT INTO SYSMONCFG (ISYSMON_NO, CDISPLAY, CDBCONN, CSQL, IENABLED, ISHOW_DETAILS) VALUES (19, '���|(8625�W) - ���ԸgIPMUX�o�e���T���n�s:', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100]', 'SELECT CQUERY FROM CHK2G_QUEUE order by IREC_NO', 1, 0);