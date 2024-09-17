CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS noir.Products  (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    desc TEXT,
    price INT NOT NULL, 
    inventory INT NOT NULL,
    image_url TEXT,
);
