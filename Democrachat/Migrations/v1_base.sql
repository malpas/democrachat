--liquibase formatted sql

--changeset creator:create-account
CREATE TABLE IF NOT EXISTS account (
	id SERIAL PRIMARY KEY,
	username TEXT UNIQUE,
	hash TEXT
);
--rollback DROP TABLE account;


--changeset creator:add-account-timestamp
ALTER TABLE account
ADD IF NOT EXISTS created_at TIMESTAMP DEFAULT NOW();
--rollback ALTER TABLE DROP COLUMN account;


--changeset creator:add-topic
CREATE TABLE IF NOT EXISTS topic (
    name TEXT PRIMARY KEY
)
--rollback DROP TABLE topic;


--changeset creator:add-currency-columns
ALTER TABLE account ADD IF NOT EXISTS gold INT;
ALTER TABLE account ADD IF NOT EXISTS silver INT;
--rollback ALTER TABLE account DROP COLUMN gold
--rollback ALTER TABLE account DROP COLUMN silver


--changeset creator:add-account-muted-until
ALTER TABLE account
ADD IF NOT EXISTS muted_until TIMESTAMP;
--rollback ALTER TABLE account DROP COLUMN muted_until;


--changeset creator:add-topic-bid
CREATE TABLE IF NOT EXISTS topic_bid (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES account (id),
    name TEXT NOT NULL,
    silver INT NOT NULL
);
--rollback DROP TABLE topic_bid;


--changeset creator:set-account-currency-defaults
ALTER TABLE account
ALTER silver SET DEFAULT 0;

UPDATE account SET silver = 0 WHERE silver IS NULL;
ALTER TABLE account
ALTER silver SET NOT NULL;

ALTER TABLE account
ALTER gold SET DEFAULT 0;

UPDATE account SET gold = 0 WHERE gold IS NULL;
ALTER TABLE account
ALTER gold SET NOT NULL;


--changeset creator:add-chat-message
CREATE TABLE IF NOT EXISTS chat_message (
	id SERIAL PRIMARY KEY,
	user_id INT REFERENCES account (id) NOT NULL,
	topic TEXT NOT NULL, 
	text TEXT NOT NULL,
	time TIMESTAMP DEFAULT now()
);
--rollback DROP TABLE chat_message;
