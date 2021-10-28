--liquibase formatted sql

--changeset creator:add-login-ip
ALTER TABLE login ADD COLUMN ip cidr;

--changeset creator:add-default-topics
INSERT INTO topic (name) VALUES ('general'), ('music'), ('gaming')
ON CONFLICT DO NOTHING;
