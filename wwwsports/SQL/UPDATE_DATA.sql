Remark: ASIA_NBA

Remark: Create New Table LOG_BSK_XXX, index, RANKPERSONAL_INFO, PLAYERS_INFO

Remark: Update CLEAG_ID and CTEAM_ID to 3 digits by VBScript Rearrange BSK Team/Leag ID

Remark: Add league for personal rank, modify CORG to league type
Remark: Delete league that not used. i.e. BLOCK_RANK, STEAL_RANK

Remark: Double check the CLEAG_ID
INSERT INTO LEAGUE_INFO (CLEAG_ID, CLEAGUE, CALIAS, CORG, CLEAGUETYPE) VALUES ('007', '�o���]', '�o���]', 'SCORE_RANK', '2');
INSERT INTO LEAGUE_INFO (CLEAG_ID, CLEAGUE, CALIAS, CORG, CLEAGUETYPE) VALUES ('008', '�x�O�]', '�x�O�]', 'REB_RANK', '2');
INSERT INTO LEAGUE_INFO (CLEAG_ID, CLEAGUE, CALIAS, CORG, CLEAGUETYPE) VALUES ('009', '�U��]', '�U��]', 'AST_RANK', '2');
commit;
Remark: Double check the CLEAG_ID
UPDATE LEAGUE_INFO SET CORG='ATL_RANK', CLEAGUETYPE='1' where CLEAGUE='�j��v��';
UPDATE LEAGUE_INFO SET CORG='CEN_RANK', CLEAGUETYPE='1' where CLEAGUE='������';
UPDATE LEAGUE_INFO SET CORG='MW_RANK', CLEAGUETYPE='1' where CLEAGUE='�����';
UPDATE LEAGUE_INFO SET CORG='PAC_RANK', CLEAGUETYPE='1' where CLEAGUE='�ӥ��v��';
UPDATE LEAGUE_INFO SET CORG='', CLEAGUETYPE='0' where CLEAGUE='NBA';
UPDATE LEAGUE_INFO SET CORG='', CLEAGUETYPE='0' where CLEAGUE='NBA���P��';
commit;

Remark: Get Personal Rank and Player SQL, double check CLEAG_ID
Remark: Modify Player SQL
���y�Z��	0
�o���Z��	1
�p�e�W		2
�j�e�W		3
���W			4

Remark: Create trigger and default value in JOB_QUEUE
Remark: Drop table, PLAYER_INFO, RANK_PERSONAL_INFO, SELECTSTAUS (May Be in future)