CREATE GENERATOR LOG_SPTNEWS_SEQ_GEN;
SET GENERATOR LOG_SPTNEWS_SEQ_GEN TO 0;
COMMIT;

create table LOG_SPORTNEWS
(
  ISEQ_NO INTEGER NOT NULL,
  TIMEFLAG TIMESTAMP NOT NULL,
  SECTION VARCHAR(7) NOT NULL,
  IMSG_ID INTEGER NOT NULL,
  IAPP_ID INTEGER NOT NULL,
  Act VARCHAR(1),
  NEWSDATE VARCHAR(8),
  NEWSTIME VARCHAR(4),
	CONTENT VARCHAR(400),
	BATCHJOB VARCHAR(60),
  CONSTRAINT LOG_SPTNEWS_PK PRIMARY KEY (ISEQ_NO, TIMEFLAG)
);
grant all on LOG_SPORTNEWS to public;
commit;

SET TERM !! ;
create trigger AUTO_LOG_SPTNEWS_SEQ for LOG_SPORTNEWS
before insert as
begin
new.ISEQ_NO = GEN_ID(LOG_SPTNEWS_SEQ_GEN, 1);
end!!
SET TERM ;!!
commit;

Remark: if required
alter table LOG_SPORTNEWS add BATCHJOB VARCHAR(60);