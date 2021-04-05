--liquibase formatted sql

--changeset creator:add-items
CREATE TABLE IF NOT EXISTS item_template (
	id SERIAL PRIMARY KEY,
	name TEXT NOT NULL,
	script TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS item (
	id SERIAL PRIMARY KEY,
	owner_id INT REFERENCES account (id),
	template_id INT REFERENCES item_template (id) NOT NULL,
	public_uuid UUID NOT NULL DEFAULT gen_random_uuid()
);
--rollback DROP TABLE item; DROP TABLE item_template;


--changeset creator:add-item-image
ALTER TABLE item_template ADD COLUMN IF NOT EXISTS image_src TEXT;
--rollback ALTER TABLE item_template DROP COLUMN image_src;