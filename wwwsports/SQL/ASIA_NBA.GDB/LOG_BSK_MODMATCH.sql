CREATE GENERATOR LOG_BSKMODMTH_SEQ_GEN;
SET GENERATOR LOG_BSKMODMTH_SEQ_GEN TO 0;
COMMIT;

CREATE TABLE LOG_BSK_MODMATCH
(
  ISEQ_NO	INTEGER NOT NULL,
  TIMEFLAG	TIMESTAMP NOT NULL,
  Section	VARCHAR(8) NOT NULL,
  CMD	VARCHAR(20),
  PARAM1	VARCHAR(20),
  PARAM2	VARCHAR(20),
  PARAM3	VARCHAR(20),
  PARAM4	VARCHAR(4),
  PARAM5	VARCHAR(4),
  PARAM6	VARCHAR(8),
  PARAM7	VARCHAR(8),
  PARAM8	VARCHAR(4),
  PARAM9	VARCHAR(4),
  BATCHJOB	VARCHAR(60),
CONSTRAINT LOG_BSK_MODMTH_PK PRIMARY KEY (ISEQ_NO, TIMEFLAG)
);

SET TERM !! ;
create trigger AUTO_LOG_BSKMODMTH_SEQ for LOG_BSK_MODMATCH
before insert as
begin
new.ISEQ_NO = GEN_ID(LOG_BSKMODMTH_SEQ_GEN, 1);
end!!
SET TERM ;!!
commit;