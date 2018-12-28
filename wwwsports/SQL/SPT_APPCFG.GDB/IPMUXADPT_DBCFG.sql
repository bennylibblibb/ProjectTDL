create table IPMUXADPT_DBCFG
(
  IDB_ID INTEGER NOT NULL,
  CMACHINE VARCHAR(50) NOT NULL,
  CDBCONN	VARCHAR(200),
  PRIMARY KEY (IDB_ID)
);
grant all on IPMUXADPT_DBCFG to public;
commit;

INSERT INTO IPMUXADPT_DBCFG (IDB_ID, CMACHINE, CDBCONN) VALUES (1, 'Sports Local - 127.0.0.1', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/Sports/db/ASIA_SOCCER_DB.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100');
INSERT INTO IPMUXADPT_DBCFG (IDB_ID, CMACHINE, CDBCONN) VALUES (2, 'HKJC Local - 127.0.0.1', 'Provider=IbOleDb;Location=127.0.0.1;Data Source=D:/JCSports_2G/db/JCSPORTS_2G.GDB;User ID=sysdba;Password=masterkey;Min Pool Size=5;Max Pool Size=100');
commit;