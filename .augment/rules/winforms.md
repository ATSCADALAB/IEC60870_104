---
type: "manual"
---

# 🧠 Quy Tắc Code Chuẩn Cho Dự Án WinForms

## 1. Đọc thư viện trước khi code
- Trước khi viết bất kỳ tính năng nào, **đọc kỹ thư viện, API, cấu trúc class hiện có**.
- Kiểm tra các hàm đã có để **tránh viết trùng hoặc lặp logic**.

## 2. Đặt tên rõ ràng, nhất quán
- Tên biến, hàm, class phải **rõ nghĩa, không viết tắt** mơ hồ.
- Quy ước:
  - Control UI: `txtTen`, `btnLuu`, `lblThongBao`
  - Hàm xử lý: `TaiDanhSachNguoiDung()`, `LuuDuLieu()`, `TinhTongTien()`
  - Class: `NguoiDungService`, `PhongBanRepository`, `ThongBaoHelper`

## 3. Phân tách UI và logic
- Không viết logic xử lý nặng trực tiếp trong form → đưa vào lớp `Service` hoặc `Repository`.
- Mọi kết nối DB, xử lý file, API phải được tách riêng, **không code trong sự kiện Click**.

## 4. Có comment tiếng Việt đầy đủ
- Mỗi hàm **bắt buộc có comment tiếng Việt** mô tả ngắn gọn mục đích, tham số và trả về (nếu có).
- Nếu đoạn code dài > 10 dòng hoặc phức tạp → comment thêm giải thích từng bước chính.

## 5. Quy tắc giao diện (UI)
- Giao diện phải **gọn gàng, canh chỉnh đều**, dùng `TableLayoutPanel` hoặc `FlowLayoutPanel` nếu cần.
- Không dùng màu mè hoặc font lạ trong giao diện.
- **Không hardcode text** → dùng Resource file hoặc hằng số.

## 6. Kết nối & truy xuất dữ liệu
- **Luôn sử dụng `using` để đóng kết nối tự động.**
- Tuyệt đối **không dùng query string nối chuỗi** → phải dùng parameterized SQL để tránh SQL Injection.

## 7. Bắt lỗi & ghi log
- Mọi thao tác đọc/ghi file, DB, API đều phải bọc `try-catch`.
- Nếu xảy ra lỗi: **ghi log + hiển thị thông báo dễ hiểu cho người dùng.**
- Không hiển thị `Exception.Message` trực tiếp ra giao diện.

## 8. Tái sử dụng mã nguồn
- Nếu một đoạn code dùng lại > 1 lần → tách ra thành hàm riêng.
- Hạn chế sao chép code giữa các form.

## 9. Chuẩn bị cho mở rộng
- Mỗi form, class viết sao cho dễ mở rộng hoặc thay đổi sau này.
- Không dùng `static` tràn lan. Dùng `interface` nếu cần mở rộng.

## 10. Giao tiếp giữa các form
- Dùng **delegate, event hoặc truyền qua constructor**, không sử dụng biến toàn cục.

## 11. Testing thủ công
- Trước khi gửi code, **tự test ít nhất 2 lần**: 1 lần input hợp lệ, 1 lần input lỗi.
- Kiểm tra thông báo lỗi, UI hiển thị đúng, dữ liệu được lưu đúng.

---

📝 **Lưu ý cuối cùng:**  
Viết code sạch, dễ đọc, không cần phải thông minh – chỉ cần **người khác dễ hiểu là thành công**.  
