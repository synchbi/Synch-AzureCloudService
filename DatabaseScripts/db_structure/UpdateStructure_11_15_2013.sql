--	create new tables

-- Business table: name and postalCode combination should be unique
CREATE TABLE Business(
	id int primary key identity, 
	name varchar(100) NOT NULL,
	integration int DEFAULT 0,
	tier int DEFAULT 0,
	address varchar(200),
	postalCode varchar(10),
	email varchar(200),
	phoneNumber varchar(20)
);

-- Account table: login should be unique
CREATE TABLE Account(
	id int primary key identity, 
	businessId int NOT NULL,
	login varchar(256) NOT NULL,
	password varchar(128) NOT NULL,
	tier int DEFAULT 0,
	firstName varchar(50),
	lastName varchar(50),
	email varchar(200) NOT NULL,
	phoneNumber varchar(20),
	sessionId varchar(512) NOT NULL,
	deviceId varchar(128),
	UNIQUE(login),
	FOREIGN KEY (businessId)
		REFERENCES Business(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- Customer table: businessId and customerId should be unique combination
CREATE TABLE Customer
	accountId int NOT NULL,
	businessId int NOT NULL,
	customerId int NOT NULL,
	address varchar(200),
	email varchar(200),
	phoneNumber varchar(20),
	category int DEFAULT 0,
	FOREIGN KEY (businessId)
		REFERENCES Business(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (customerId)
		REFERENCES Business(id)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
);

-- Supplier table: businessId and supplierId should be unique combination
CREATE TABLE Supplier(
	accountId int NOT NULL,
	businessId int NOT NULL,
	supplierId int NOT NULL,
	address varchar(200),
	email varchar(200),
	phoneNumber varchar(20),
	category int DEFAULT 0,
	FOREIGN KEY (businessId)
		REFERENCES Business(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (supplierId)
		REFERENCES Business(id)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
);


CREATE TABLE Product(
	upc varchar(20) primary key
);

CREATE TABLE Inventory(
	businessId int NOT NULL,
	upc varchar(20) NOT NULL,
	name varchar(100) NOT NULL,
	purchasePrice decimal(18,2) NOT NULL,
	defaultPrice decimal(18,2) NOT NULL,
	detail varchar(200),
	leadTime int, 
	quantityAvailable int NOT NULL,
	quantityOnPurchaseOrder int NOT NULL,
	reorderQuantity int NOT NULL,
	reorderPoint int NOT NULL,
	category int DEFAULT 0,
	location varchar(40), 
	FOREIGN KEY (businessId)
		REFERENCES Business(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (upc)
		REFERENCES Product(upc)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE Record(
	id int identity primary key nonclustered,
	category int NOt NULL,
	accountId int NOT NULL,
	ownerId int NOT NULL,
	clientId int NOT NULL,
	status int NOT NULL,
	title varchar(50) NOT NULL,
	comment varchar(140),
	transactionDate datetimeoffset(7) NOT NULL,
	deliveryDate datetimeoffset(7),
	FOREIGN KEY (accountId)
		REFERENCES Account(id)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION,
	FOREIGN KEY (ownerId)
		REFERENCES Business(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (clientId)
		REFERENCES Business(id)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
);

CREATE TABLE RecordLine(
	recordId int NOT NULL,
	upc varchar(20) NOT NULL,
	quantity int NOT NULL,
	price decimal(18,2) NOT NULL,
	note varchar(200),
	FOREIGN KEY (recordId)
		REFERENCES Record(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (upc)
		REFERENCES Product(upc)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE CLUSTERED INDEX IX_Customer_businessId ON Customer(businessId);
CREATE CLUSTERED INDEX IX_Supplier_businessId ON Supplier(businessId);
CREATE CLUSTERED INDEX IX_Inventory_businessId ON Inventory(businessId);
CREATE CLUSTERED INDEX IX_RecordLine_recordId ON RecordLine(recordId);
CREATE CLUSTERED INDEX IX_Record_ownerId_transactionDate ON Record(ownerId, transactionDate);

CREATE INDEX IX_Business_name_postalCode on Business(name, postalCode);
