CREATE TABLE services (
    id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    business_id uuid NOT NULL REFERENCES businesses(id) on delete cascade,
    name varchar(200) NOT NULL,
    description text,
    price numeric(10,2) NOT NULL CHECK(price > 0),
    duration_minutes int NOT NUll CHECK(duration_minutes BETWEEN 5 and 480)
);