CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS noir.Products  (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    prod_name VARCHAR(255) NOT NULL,
    prod_desc TEXT,
    price NUMERIC(10, 2) NOT NULL,
    wid NUMERIC(10, 3) NOT NULL,
    hei NUMERIC(10, 3) NOT null,
    type text NOT null
);
