CREATE TABLE categories
(
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL,
    slug varchar(100) UNIQUE NOT NULL
);