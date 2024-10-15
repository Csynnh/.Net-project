CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA IF NOT EXISTS noir;

CREATE TABLE IF NOT EXISTS noir.Contact_History (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID NOT NULL REFERENCES Accounts(id),
    contact_details JSON,
    contact_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);