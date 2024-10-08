CREATE TABLE IF NOT EXISTS noir.InvoiceDetails (
    invoice_id UUID NOT NULL,
    product_id UUID NOT NULL,
    amount INT NOT NULL,
    price NUMERIC(10, 2) NOT NULL,
    PRIMARY KEY (invoice_id, product_id),
    CONSTRAINT fk_invoice
        FOREIGN KEY (invoice_id) REFERENCES noir.Invoices(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_product
        FOREIGN KEY (product_id) REFERENCES noir.Products(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);