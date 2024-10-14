CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS noir.ProductColors (
    id SERIAL PRIMARY KEY,
    product_id UUID REFERENCES Products(id) ON DELETE CASCADE,
    color_name VARCHAR(255) NOT NULL,
    color_code VARCHAR(7) NOT NULL,
    inventory INT NOT NULL,
    total INT NOT null,
    image1_url TEXT,
    image2_url TEXT,
    image3_url TEXT,
    image4_url TEXT,
    image5_url TEXT
);