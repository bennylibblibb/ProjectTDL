create table CORRECTSCORE_INFO
(
  IMATCH_CNT	INTEGER NOT NULL,
  ICORRECTSCORE_CNT INTEGER NOT NULL,
  CODDS	VARCHAR(5),
  CACT	VARCHAR(1),
  IMATRIXSIZE INTEGER,
  CONSTRAINT PKCORSCORE PRIMARY KEY (IMATCH_CNT, ICORRECTSCORE_CNT)
);
grant all on CORRECTSCORE_INFO to public;
commit;