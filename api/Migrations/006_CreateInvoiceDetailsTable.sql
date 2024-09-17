CREATE TABLE IF NOT EXISTS noir.InvoiceDetails (
    invoice_id INT,
    product_id INT,
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