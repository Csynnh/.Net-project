CREATE TABLE IF NOT EXISTS noir.InvoiceDetails (
    invoice_id INT NOT NULL,
    product_id INT NOT NULL,
    amount INT NOT NULL,
    price INT NOT NULL,
    PRIMARY KEY (invoice_id, product_id),
    CONSTRAINT fk_invoice
        FOREIGN KEY (invoice_id) REFERENCES Invoices(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_product
        FOREIGN KEY (product_id) REFERENCES Products(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
);