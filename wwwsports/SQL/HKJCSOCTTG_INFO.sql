CREATE TABLE HKJCSOCTTG_INFO 
(
  IMATCH_CNT	INTEGER NOT NULL,
  ITOTAL_GOAL	INTEGER NOT NULL,
	CODDS				VARCHAR(6),
CONSTRAINT PKHKJCSOCTTG PRIMARY KEY (IMATCH_CNT, ITOTAL_GOAL)
);