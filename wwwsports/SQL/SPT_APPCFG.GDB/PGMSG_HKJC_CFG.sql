CREATE TABLE PGMSG_HKJC_CFG 
(
  IMSG_ID	INTEGER NOT NULL,
  CMSGTYPE	VARCHAR(20) NOT NULL,
  CPREFIX	VARCHAR(5) NOT NULL,
  CCAPCODE	VARCHAR(7) NOT NULL,
  CTONE	VARCHAR(1) NOT NULL,
  IHDRIDSTART	INTEGER NOT NULL,
  IHDRIDEND	INTEGER NOT NULL,
  IHDRIDLAST	INTEGER NOT NULL,
  CSONGID	VARCHAR(4) NOT NULL,
  IPRIORITY	INTEGER NOT NULL,
  PRIMARY KEY (IMSG_ID)
);
commit;

INSERT INTO PGMSG_HKJC_CFG VALUES (1, 'HKlivePlace', 'c27W', '1918360', 'D', 1960, 1960, 1960, '0009', 1);
commit;