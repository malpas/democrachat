--liquibase formatted sql

--changeset creator:add-login-ip
ALTER TABLE login ADD COLUMN ip cidr;