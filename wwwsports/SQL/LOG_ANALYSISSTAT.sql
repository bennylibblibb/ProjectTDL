CREATE GENERATOR LOG_ANLYSTAT_GEN;
SET GENERATOR LOG_ANLYSTAT_GEN TO 0;
COMMIT;

create table LOG_ANALYSISSTAT
(
  ISEQ_NO INTEGER NOT NULL,
  TIMEFLAG TIMESTAMP NOT NULL,
  SECTION VARCHAR(15) NOT NULL,
  Act VARCHAR(1),
  League VARCHAR(18) NOT NULL,
  Host VARCHAR(8) NOT NULL,
  Guest VARCHAR(8) NOT NULL,
  MatchDate VARCHAR(8),
	MatchTime VARCHAR(4),
	MatchField VARCHAR(1),
	HostHandicap VARCHAR(1),
  HostWin INTEGER,
  HostDraw INTEGER,
  HostLoss INTEGER,
  GuestWin INTEGER,
  GuestDraw INTEGER,
  GuestLoss INTEGER,
  BATCHJOB VARCHAR(60),
  CONSTRAINT LOG_ANLYSTAT_PK PRIMARY KEY (ISEQ_NO, TIMEFLAG)
);
grant all on LOG_ANALYSISSTAT to public;
commit;

SET TERM !! ;
create trigger AUTO_LOG_ANLYSTAT_SEQ for LOG_ANALYSISSTAT
before insert as
begin
new.ISEQ_NO = GEN_ID(LOG_ANLYSTAT_GEN, 1);
end!!
SET TERM ;!!
commit;

Remark: if required
alter table LOG_ANALYSISSTAT add MatchDate VARCHAR(8);
alter table LOG_ANALYSISSTAT add MatchTime VARCHAR(4);
alter table LOG_ANALYSISSTAT add MatchField VARCHAR(1);
alter table LOG_ANALYSISSTAT add HostHandicap VARCHAR(1);
commit;
alter table LOG_ANALYSISSTAT alter MatchDate POSITION 7;
alter table LOG_ANALYSISSTAT alter MatchTime POSITION 8;
alter table LOG_ANALYSISSTAT alter MatchField POSITION 9;
alter table LOG_ANALYSISSTAT alter HostHandicap POSITION 10;
commit;