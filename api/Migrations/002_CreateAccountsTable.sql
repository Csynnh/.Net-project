CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA IF NOT EXISTS noir;

CREATE TABLE IF NOT EXISTS noir.Accounts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    phone_number VARCHAR(12),
    address TEXT, -- Địa chỉ người dùng
    role BOOLEAN NOT NULL -- 1 - ADMIN, 0 - USER
);