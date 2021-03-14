CREATE TABLE topic_bid (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES account (id),
    name TEXT NOT NULL,
    silver INT NOT NULL
)