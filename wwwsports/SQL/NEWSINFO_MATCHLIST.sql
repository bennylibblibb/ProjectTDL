create table NEWSINFO_MATCHLIST
(
  IAPP_ID	INTEGER NOT NULL,
  IMATCH_CNT	INTEGER NOT NULL,
  CLEAGUE	VARCHAR(24),
  CHOST		VARCHAR(8),
  CGUEST	VARCHAR(8),
  CONSTRAINT PKNEWSINFO_LIST PRIMARY KEY (IAPP_ID, IMATCH_CNT)
);
grant all on NEWSINFO_MATCHLIST to public;
commit;