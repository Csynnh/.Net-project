#!/bin/bash
set -e

# Create the table if it does not exist
psql -h postgres -U user -d todo_db <<EOF
CREATE TABLE IF NOT EXISTS library_app.books (
    Book_Id SERIAL PRIMARY KEY,
    Book_Title VARCHAR(255) NOT NULL,
    Cover_Img_Url VARCHAR(255) NOT NULL,
    Author VARCHAR(255) NOT NULL,
    Publisher VARCHAR(255) NOT NULL
);
"
EOF