-- 3. Bảng Nhận xét của khách hàng (CustomerReviews)
-- •	ID_NhanXet (Primary Key): Khóa chính, định danh duy nhất cho mỗi nhận xét.
-- •	ID_TaiKhoan (Foreign Key): Khóa ngoại, liên kết tới tài khoản của người dùng (Bảng Tài khoản).
-- •	ID_HangHoa (Foreign Key): Khóa ngoại, liên kết tới sản phẩm được nhận xét (Bảng Hàng hóa).
-- •	NoiDung: Nội dung của nhận xét.
-- •	DanhGia: Đánh giá của người dùng (thang điểm, ví dụ từ 1 đến 5 sao).
-- •	NgayNhanXet: Ngày người dùng viết nhận xét.

-- create random key value
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";


CREATE TABLE IF NOT EXISTS noir.CustomerReviews(
    id UUID PRIMARY KEY DEFAULT UUID_GENERATE_V4(),
    account_id UUID REFERENCES noir.Accounts(id),
    product_id UUID REFERENCES noir.Products(id),
    content TEXT,
    vote INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)