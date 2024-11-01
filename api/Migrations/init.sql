CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA IF NOT EXISTS NOIRTEST;

-- Create the Accounts table
CREATE TABLE IF NOT EXISTS NOIRTEST.ACCOUNTS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    phone_number VARCHAR(12),
    role VARCHAR(50) NOT NULL
);

-- Create Size table
CREATE TABLE IF NOT EXISTS NOIRTEST.SIZES (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    size VARCHAR(255) NOT NULL,
    UNIQUE (size)
);

-- Create the Types table
CREATE TABLE IF NOT EXISTS NOIRTEST.TYPES (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    type VARCHAR(255) NOT NULL,
    UNIQUE (type)
);


-- Create the Colors table
CREATE TABLE IF NOT EXISTS NOIRTEST.COLORS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    color VARCHAR(255) NOT NULL,
    UNIQUE (color)
);

-- Create the Products table
CREATE TABLE IF NOT EXISTS NOIRTEST.PRODUCTS  (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description VARCHAR(255) NOT NULL,
    price NUMERIC(10, 2) NOT NULL,
    type_id UUID REFERENCES NOIRTEST.TYPES(id),
    inventory INT NOT NULL,
    details JSON,
    UNIQUE (name)
);

-- Create the ProductVariant table
CREATE TABLE IF NOT EXISTS NOIRTEST.PRODUCTVARIANTS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    product_id UUID REFERENCES NOIRTEST.PRODUCTS(id),
    size_id UUID REFERENCES NOIRTEST.SIZES(id),
    color_id UUID REFERENCES NOIRTEST.COLORS(id),
    images JSON,
    inventory INT NOT NULL
);

-- Create the CustomerReviews table
CREATE TABLE IF NOT EXISTS NOIRTEST.CUSTOMERREVIEWS(
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID REFERENCES NOIRTEST.ACCOUNTS(id),
    product_id UUID REFERENCES NOIRTEST.PRODUCTS(id),
    content JSON,
    vote INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create the Carts table
CREATE TABLE IF NOT EXISTS NOIRTEST.CARTS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID REFERENCES NOIRTEST.ACCOUNTS(id),
    product_variant_id UUID REFERENCES NOIRTEST.PRODUCTVARIANTS(id),
    quantity INT NOT NULL
);

-- Create the UserSTOREDINFOMATION table
CREATE TABLE IF NOT EXISTS NOIRTEST.USERSTOREDINFOMATION (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID REFERENCES NOIRTEST.ACCOUNTS(id),
    info JSON
);

-- Create the PaymentMethods table
CREATE TABLE IF NOT EXISTS NOIRTEST.PAYMENTMETHODS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID REFERENCES NOIRTEST.ACCOUNTS(id),
    payment_method JSON,
    enabled BOOLEAN
);

-- Create the ShippingMethods table
CREATE TABLE IF NOT EXISTS NOIRTEST.SHIPPINGMETHODS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    shipping_name VARCHAR(255) NOT NULL,
    shipping_cost NUMERIC(10, 2) NOT NULL
);

-- Create the Orders table
CREATE TABLE IF NOT EXISTS NOIRTEST.ORDERS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID REFERENCES NOIRTEST.ACCOUNTS(id),
    payment_method_id UUID REFERENCES NOIRTEST.PAYMENTMETHODS(id),
    shipping_method_id UUID REFERENCES NOIRTEST.SHIPPINGMETHODS(id),
    stored_information_id UUID REFERENCES NOIRTEST.USERSTOREDINFOMATION(id),
    status VARCHAR(255) NOT NULL,
    total NUMERIC(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create the OrderDetails table
CREATE TABLE IF NOT EXISTS NOIRTEST.ORDERDETAILS (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id UUID REFERENCES NOIRTEST.ORDERS(id),
    product_variant_id UUID REFERENCES NOIRTEST.PRODUCTVARIANTS(id),
    quantity INT NOT NULL,
    price NUMERIC(10, 2) NOT NULL
);

