INSERT INTO SVCCFG (ISVC_NO, CSVC_NAME, CACCESS_RIGHT) VALUES (7, 'GOGO 籃球', '011');
INSERT INTO SVCCFG (ISVC_NO, CSVC_NAME, CACCESS_RIGHT) VALUES (8, '馬會 籃球', '011');

INSERT INTO ADMINCFG VALUES (54, '籃球', '賽事及比數<br>(清除數據庫及傳呼機)', 'CLR_TBL', 'MATCH_PAGER');
INSERT INTO ADMINCFG VALUES (55, '籃球', '賽事及比數<br>(只清除數據庫；不清除傳呼機)', 'CLR_TBL', 'MATCH');
INSERT INTO ADMINCFG VALUES (56, '籃球', '賽事及比數<br>(清除數據庫及傳呼機)', 'CLR_ALL', 'MATCH_WEB_PAGER');
INSERT INTO ADMINCFG VALUES (57, '籃球', '賽事及比數<br>(只清除數據庫；不清除傳呼機)', 'CLR_ALL', 'MATCH_WEB');
INSERT INTO ADMINCFG VALUES (58, '籃球', '賽事', 'RESEND', 'MATCH');
INSERT INTO ADMINCFG VALUES (59, '籃球', '比數', 'RESEND', 'GOAL');
INSERT INTO ADMINCFG VALUES (60, '籃球', '賽果', 'RESEND', 'HKRES');
INSERT INTO ADMINCFG VALUES (61, '足球1', '預報', 'RESEND_FOREODDS', '');

INSERT INTO ADMINSVCMAP VALUES (7, 54, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 55, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 56, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 57, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 58, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 59, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (7, 60, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 54, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 55, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 56, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 57, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 58, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 59, CURRENT_TIMESTAMP);
INSERT INTO ADMINSVCMAP VALUES (8, 60, CURRENT_TIMESTAMP);