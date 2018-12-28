CREATE TABLE LOGINRIGHT 
(
  ACCESS_RIGHT	VARCHAR(3) NOT NULL,
  CDESC	VARCHAR(30) NOT NULL,
 UNIQUE (CDESC),
 PRIMARY KEY (ACCESS_RIGHT)
);

insert into LOGINRIGHT values ('999', 'TDL Web Master');
insert into LOGINRIGHT values ('988', 'TDL Admin User');
insert into LOGINRIGHT values ('011', 'Asia Sports Admin User');
insert into LOGINRIGHT values ('001', 'Asia Sports Operator');