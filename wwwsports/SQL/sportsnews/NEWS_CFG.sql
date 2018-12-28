
/* Table: NEWS_CFG, Owner: SYSDBA */

CREATE TABLE "NEWS_CFG" 
(
  "ISEQNO"	INTEGER NOT NULL,
  "CAPPTYPE"	VARCHAR(20),
  "CINFOTYPE"	VARCHAR(10),
  "IHDRIDSTART"	INTEGER,
  "IHDRIDEND"	INTEGER,
  "CTONE"	VARCHAR(1),
  "CCAPCODE"	VARCHAR(10),
  "ICOLUMN"		INTEGER,
  "IDISPLAY_NUM"	INTEGER default 0,
 PRIMARY KEY ("ISEQNO")
);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (22, '足球資訊1[1]', '吳群立推介', 576, 576, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (36, '足球資訊1[1]', '丘建威推介', 804, 804, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (20, '足球資訊1[5]', '分析賽程', 560, 564, 'A', '1136283', 5, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (21, '足球資訊1[2]', '分析新聞', 558, 559, 'A', '1136283', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (2, '足球資訊1[10]', '直播', 565, 574, 'A', '1136283',10, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (15, '籃球[5]', '分析', 577, 582, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (16, '籃球[5]', '新聞', 1763, 1767, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (17, '籃球[5]', '直播', 1344, 1348, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (18, '籃球[5]', '賽程', 1324, 1328, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (29, '足球資訊1[1]', '張志德推介', 1647, 1647, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (30, '足球資訊1[1]', '歐偉倫推介', 1671, 1671, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (31, '足球資訊1[1]', '黃興桂推介', 1639, 1639, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (32, '足球資訊1[1]', '彭偉國推介', 1640, 1640, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (33, '足球資訊1[1]', '山度士推介', 1678, 1678, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (34, '足球資訊1[1]', '蔣世豪推介', 1645, 1645, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (35, '足球資訊1[1]', '陳炳安推介', 1646, 1646, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (38, '足球資訊1[1]', '丁偉傑推介', 1995, 1995, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (39, '足球資訊1[2]', '足彩', 1648, 1649, 'A', '1136283', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (101, '馬會資訊1[1]', '吳群立推介', 3660, 3660, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (102, '馬會資訊1[1]', '丘建威推介', 3661, 3661, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (103, '馬會資訊1[1]', '張志德推介', 3664, 3664, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (104, '馬會資訊1[1]', '歐偉倫推介', 3665, 3665, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (105, '馬會資訊1[1]', '黃興桂推介', 3666, 3666, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (106, '馬會資訊1[1]', '彭偉國推介', 3667, 3667, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (107, '馬會資訊1[1]', '山度士推介', 3668, 3668, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (108, '馬會資訊1[1]', '蔣世豪推介', 3662, 3662, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (109, '馬會資訊1[1]', '陳炳安推介', 3663, 3663, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (110, '馬會資訊1[1]', '丁偉傑推介', 3669, 3669, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (40, '高爾夫球[1]', '高球賽程', 1314, 1314, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (41, '高爾夫球[1]', '高球比數', 1315, 1315, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (42, '高爾夫球[1]', '高球賽果', 1316, 1316, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (43, '高爾夫球[1]', '高球分析', 1317, 1317, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (44, '高爾夫球[1]', '高球排名', 1318, 1318, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (45, '高爾夫球[1]', '高球新聞', 1261, 1261, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (46, '高爾夫球[1]', '高球直播', 1262, 1262, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (47, '高爾夫球[1]', '高球賽事', 1263, 1263, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (48, '網球[1]', '網球賽程', 1319, 1319, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (49, '網球[1]', '網球比數', 1320, 1320, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (50, '網球[1]', '網球賽果', 1321, 1321, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (51, '網球[1]', '網球分析', 1322, 1322, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (52, '網球[1]', '網球排名', 1323, 1323, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (53, '網球[1]', '網球新聞', 1636, 1636, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (54, '網球[1]', '網球直播', 1637, 1637, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (55, '網球[1]', '網球賽事', 1638, 1638, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (111, '馬會資訊1[5]', '分析賽程', 3301, 3305, 'A', '1136283', 5, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (5, '其他運動[2]', '棒球新聞', 1672, 1673, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (6, '其他運動[2]', '體操新聞', 1674, 1675, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (7, '其他運動[2]', '桌球新聞', 1251, 1252, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (8, '其他運動[2]', '田徑新聞', 1253, 1254, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (9, '其他運動[2]', '游泳新聞', 1255, 1256, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (10, '其他運動[2]', '排球新聞', 1259, 1260, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (11, '其他運動[2]', '乒乓新聞', 1641, 1642, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (12, '其他運動[2]', '羽球新聞', 1643, 1644, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (60, '其他運動[2]', '風帆新聞', 1679, 1680, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (61, '其他運動[2]', '跳水新聞', 1257, 1258, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (13, '其他運動[2]', '單車新聞', 556, 557, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (14, '其他運動[2]', '賽車新聞', 1339, 1340, 'C', '1143984', 2, 1);


/* Local DataBase  */
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (22, '足球資訊1[1]', '吳群立推介', 576, 576, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (36, '足球資訊1[1]', '丘建威推介', 804, 804, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (20, '足球資訊1[5]', '分析賽程', 560, 564, 'A', '1136283', 5, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (21, '足球資訊1[2]', '分析新聞', 558, 559, 'A', '1136283', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (2, '足球資訊1[10]', '直播', 565, 574, 'A', '1136283', 10, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (15, '籃球[5]', '分析', 577, 582, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (16, '籃球[5]', '新聞', 1763, 1767, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (17, '籃球[5]', '直播', 1344, 1348, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (18, '籃球[5]', '賽程', 1324, 1328, 'C', '1143984', 5, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (29, '足球資訊1[1]', '張志德推介', 1647, 1647, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (30, '足球資訊1[1]', '歐偉倫推介', 1671, 1671, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (31, '足球資訊1[1]', '黃興桂推介', 1639, 1639, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (32, '足球資訊1[1]', '彭偉國推介', 1640, 1640, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (33, '足球資訊1[1]', '山度士推介', 1678, 1678, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (34, '足球資訊1[1]', '蔣世豪推介', 1645, 1645, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (35, '足球資訊1[1]', '陳炳安推介', 1646, 1646, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (38, '足球資訊1[1]', '丁偉傑推介', 1995, 1995, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (39, '足球資訊1[2]', '足彩', 1648, 1649, 'A', '1136283', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (101, '馬會資訊1[1]', '吳群立推介', 3660, 3660, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (102, '馬會資訊1[1]', '丘建威推介', 3661, 3661, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (103, '馬會資訊1[1]', '張志德推介', 3664, 3664, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (104, '馬會資訊1[1]', '歐偉倫推介', 3665, 3665, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (105, '馬會資訊1[1]', '黃興桂推介', 3666, 3666, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (106, '馬會資訊1[1]', '彭偉國推介', 3667, 3667, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (107, '馬會資訊1[1]', '山度士推介', 3668, 3668, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (108, '馬會資訊1[1]', '蔣世豪推介', 3662, 3662, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (109, '馬會資訊1[1]', '陳炳安推介', 3663, 3663, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (110, '馬會資訊1[1]', '丁偉傑推介', 3669, 3669, 'A', '1136283', 1, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (40, '高爾夫球[1]', '高球賽程', 1314, 1314, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (41, '高爾夫球[1]', '高球比數', 1315, 1315, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (42, '高爾夫球[1]', '高球賽果', 1316, 1316, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (43, '高爾夫球[1]', '高球分析', 1317, 1317, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (44, '高爾夫球[1]', '高球排名', 1318, 1318, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (45, '高爾夫球[1]', '高球新聞', 1261, 1261, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (46, '高爾夫球[1]', '高球直播', 1262, 1262, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (47, '高爾夫球[1]', '高球賽事', 1263, 1263, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (48, '網球[1]', '網球賽程', 1319, 1319, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (49, '網球[1]', '網球比數', 1320, 1320, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (50, '網球[1]', '網球賽果', 1321, 1321, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (51, '網球[1]', '網球分析', 1322, 1322, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (52, '網球[1]', '網球排名', 1323, 1323, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (53, '網球[1]', '網球新聞', 1636, 1636, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (54, '網球[1]', '網球直播', 1637, 1637, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (55, '網球[1]', '網球賽事', 1638, 1638, 'A', '1143984', 1, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (111, '馬會資訊1[5]', '分析賽程', 3301, 3305, 'A', '1136283', 5, 0);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (5, '其他運動[2]', '棒球新聞', 1672, 1673, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (6, '其他運動[2]', '體操新聞', 1674, 1675, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (7, '其他運動[2]', '桌球新聞', 1251, 1252, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (8, '其他運動[2]', '田徑新聞', 1253, 1254, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (9, '其他運動[2]', '游泳新聞', 1255, 1256, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (10, '其他運動[2]', '排球新聞', 1259, 1260, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (11, '其他運動[2]', '乒乓新聞', 1641, 1642, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (12, '其他運動[2]', '羽球新聞', 1643, 1644, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (60, '其他運動[2]', '風帆新聞', 1679, 1680, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (61, '其他運動[2]', '跳水新聞', 1257, 1258, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (13, '其他運動[2]', '單車新聞', 556, 557, 'C', '1143984', 2, 1);
INSERT INTO "NEWS_CFG" ("ISEQNO", "CAPPTYPE", "CINFOTYPE", "IHDRIDSTART", "IHDRIDEND", "CTONE", "CCAPCODE", "ICOLUMN", "IDISPLAY_NUM") VALUES (14, '其他運動[2]', '賽車新聞', 1339, 1340, 'C', '1143984', 2, 1);