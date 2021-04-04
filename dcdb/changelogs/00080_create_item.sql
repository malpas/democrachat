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
