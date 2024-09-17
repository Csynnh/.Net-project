-- Create the schema if it does not exist
CREATE SCHEMA IF NOT EXISTS library_app;

-- Create the table within the schema
CREATE TABLE IF NOT EXISTS noir.books (
    BookId SERIAL PRIMARY KEY,
    BookTitle VARCHAR(255) NOT NULL,
    CoverImgUrl VARCHAR(255) NOT NULL,
    Author VARCHAR(255) NOT NULL,
    Publisher VARCHAR(255) NOT NULL
);