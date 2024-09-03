#!/bin/bash

# Create a .pgpass file to store the PostgreSQL credentials
echo "$DB_HOST:5432:$DB_NAME:$DB_USER:$DB_PASSWORD" > ~/.pgpass
chmod 600 ~/.pgpass
echo "db_name: $DB_NAME:$DB_USER:$DB_PASSWORD -  $DB_HOST:5432:"
# Wait for PostgreSQL to be ready
until pg_isready -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME"; do
  echo "Waiting for PostgreSQL..."
  sleep 2
done

# Initialize the database
echo "Initializing the database..."
psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -c "
CREATE SCHEMA IF NOT EXISTS library_app;

CREATE TABLE IF NOT EXISTS library_app.books (
    Book_Id SERIAL PRIMARY KEY,
    Book_Title VARCHAR(255) NOT NULL,
    Cover_Img_Url VARCHAR(255) NOT NULL,
    Author VARCHAR(255) NOT NULL,
    Publisher VARCHAR(255) NOT NULL
);
"

export pgconn="postgresql://${DB_USER}:${DB_PASSWORD}@${DB_HOST}/${DB_NAME}"
echo "postgresql://${DB_USER}:${DB_PASSWORD}@${DB_HOST}/${DB_NAME}"
exec "$@"