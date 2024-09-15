CREATE TABLE IF NOT EXISTS noir.Products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    desc TEXT,
    price INT NOT NULL, 
    inventory INT NOT NULL,
    image_url TEXT,
);
