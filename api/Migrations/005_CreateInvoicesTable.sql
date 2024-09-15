
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS noir.Invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    account_id UUID NOT NULL REFERENCES noir.Account(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total INT NOT NULL,
    status INT NOT NULL,
);