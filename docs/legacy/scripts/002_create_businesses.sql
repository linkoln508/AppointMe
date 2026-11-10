CREATE TABLE businesses
(
    id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    category_id int NOT NULL REFERENCES categories(id),
    name varchar(200) NOT NULL,
    description text,
    city varchar(100) NOT NULL,
    address varchar(300) NOT NULL,
    created_at timestamp NOT NULL DEFAULT now()
);