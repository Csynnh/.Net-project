DROP TABLE IF EXISTS noir.Accounts;
DROP TABLE IF EXISTS noir.Contact_History;
DROP TABLE IF EXISTS noir.UserAddresses;
DROP TABLE IF EXISTS noir.CustomerReviews;
DROP TABLE IF EXISTS noir.Carts;
DROP TABLE IF EXISTS noir.Products;
DROP TABLE IF EXISTS noir.ProductColors;
DROP TABLE IF EXISTS noir.Invoices;
DROP TABLE IF EXISTS noir.InvoiceDetails;


CREATE TABLE IF NOT EXISTS library_app.Category (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL
);
