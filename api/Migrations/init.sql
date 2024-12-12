CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE SCHEMA IF NOT EXISTS DEV;

-- Create the Accounts table
CREATE TABLE IF NOT EXISTS DEV.ACCOUNTS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    phone_number VARCHAR(12),
    role VARCHAR(50) NOT NULL
);

-- Create Size table
CREATE TABLE IF NOT EXISTS DEV.SIZES (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    size VARCHAR(255) NOT NULL,
    UNIQUE (size)
);

-- Create the Types table
CREATE TABLE IF NOT EXISTS DEV.TYPES (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type VARCHAR(255) NOT NULL,
    UNIQUE (type)
);


-- Create the Colors table
CREATE TABLE IF NOT EXISTS DEV.COLORS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    color VARCHAR(255) NOT NULL,
    UNIQUE (color)
);

-- Create the Products table
CREATE TABLE IF NOT EXISTS DEV.PRODUCTS  (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT,
    description TEXT,
    price NUMERIC(10, 2) NOT NULL,
    type_id UUID REFERENCES DEV.TYPES(id),
    inventory INT NOT NULL,
    details JSON,
    UNIQUE (name)
);

-- Create the ProductVariant table
CREATE TABLE IF NOT EXISTS DEV.PRODUCTVARIANTS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID REFERENCES DEV.PRODUCTS(id) ON DELETE CASCADE,
    size_id UUID REFERENCES DEV.SIZES(id),
    color_id UUID REFERENCES DEV.COLORS(id),
    images JSON,
    inventory INT NOT NULL
);

-- Create the CustomerReviews table
CREATE TABLE IF NOT EXISTS DEV.CUSTOMERREVIEWS(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    product_id UUID REFERENCES DEV.PRODUCTS(id),
    content JSON,
    vote INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create the Carts table
CREATE TABLE IF NOT EXISTS DEV.CARTS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    product_variant_id UUID REFERENCES DEV.PRODUCTVARIANTS(id),
    quantity INT NOT NULL
);

-- Create the UserSTOREDINFOMATION table
CREATE TABLE IF NOT EXISTS DEV.USERSTOREDINFOMATION (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    info JSON
);

-- Create the PaymentMethods table
CREATE TABLE IF NOT EXISTS DEV.PAYMENTMETHODS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    payment_method JSON
);

-- Create the ShippingMethods table
CREATE TABLE IF NOT EXISTS DEV.SHIPPINGMETHODS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    shipping_name VARCHAR(255) NOT NULL,
    shipping_cost NUMERIC(10, 2) NOT NULL
);

-- Create the Orders table
CREATE TABLE IF NOT EXISTS DEV.ORDERS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    payment_method_id UUID REFERENCES DEV.PAYMENTMETHODS(id),
    shipping_method_id UUID REFERENCES DEV.SHIPPINGMETHODS(id),
    stored_information_id UUID REFERENCES DEV.USERSTOREDINFOMATION(id),
    status VARCHAR(255) NOT NULL,
    total NUMERIC(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create the OrderDetails table
CREATE TABLE IF NOT EXISTS DEV.ORDERDETAILS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID REFERENCES DEV.ORDERS(id),
    product_variant_id UUID REFERENCES DEV.PRODUCTVARIANTS(id) ON DELETE CASCADE,
    quantity INT NOT NULL,
    price NUMERIC(10, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS DEV.OTP (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) REFERENCES DEV.ACCOUNTS(email) ON UPDATE CASCADE,
    otp VARCHAR(6) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS DEV.NOTIFICATIONS (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID REFERENCES DEV.ACCOUNTS(id),
    content JSON,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    type VARCHAR(255) NOT NULL,
    is_read BOOLEAN DEFAULT FALSE
);
CREATE TABLE IF NOT EXISTS DEV.EMPLOYEE(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    position VARCHAR(50) NOT NULL,
    man_hours int,
    hired_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    email VARCHAR(255),
    phone VARCHAR(255),
    avatar text
);
