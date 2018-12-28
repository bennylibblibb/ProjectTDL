CREATE TABLE HLINFO 
(
  LEAGUE	CHAR(6) NOT NULL,
  HOST	CHAR(8) NOT NULL,
  GUEST	CHAR(8) NOT NULL,
  ACT	CHAR(1),
  MATCH_CNT	INTEGER NOT NULL,
  MATCH_ID	CHAR(3) NOT NULL,
  MATCHDATE	CHAR(8),
  MATCHTIME	CHAR(4),
  MSTATUS	CHAR(1),
  FIELD	CHAR(1),
  HOST_HANDI	CHAR(1),
  FENZHONG	CHAR(4),
  HANDI1	CHAR(3),
  HANDI2	CHAR(3),
  ODDS	CHAR(5),
  OSTATUS	CHAR(1),
  ALERT_H	CHAR(1),
  LEAGLONG	CHAR(24),
  ORDER_ID	CHAR(2),
  H_SCR	CHAR(2),
  G_SCR	CHAR(2),
  FLAG	CHAR(1),
  SONG_ID	CHAR(4),
  HALFLIVE	CHAR(1)
);