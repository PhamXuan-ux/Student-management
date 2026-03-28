# Student Information Management System (SIMS) 🎓

SIMS là một hệ thống quản lý thông tin sinh viên toàn diện, được xây dựng để hiện đại hóa quy trình quản trị trong giáo dục. Dự án tập trung vào việc áp dụng các nguyên tắc lập trình hướng đối tượng (OOP) tiên tiến, kiến trúc sạch (Clean Architecture) và các mẫu thiết kế (Design Patterns) để đảm bảo tính bền vững và khả năng mở rộng.

---
 ** Flie demo và giới thiệu dự án **
**https://drive.google.com/file/d/1ATR6Ptus630ZI3Xf27Zgu3gjJjYfRoo-/view?usp=drive_link

## 🚀 Tính năng chính

Hệ thống được thiết kế với **3 phân quyền người dùng (Actors):**

- **Admin (Quản trị viên):** Quản lý toàn bộ hệ thống, quản lý khóa học, phân công giảng viên và theo dõi nhật ký hoạt động.
- **Lecturer (Giảng viên):** Quản lý lớp học được phân công và thực hiện nhập/cập nhật điểm số cho sinh viên.
- **Student (Sinh viên):** Đăng ký/hủy khóa học, xem bảng điểm cá nhân và cập nhật thông tin hồ sơ.

---

## 🏗️ Kiến trúc & Công nghệ

- **Ngôn ngữ:** C# (.NET Core)  
- **Kiến trúc:** Model-View-Controller (MVC)  
- **Cơ sở dữ liệu:** Microsoft SQL Server (sử dụng Entity Framework Core)  
- **Xác thực:** ASP.NET Core Identity Framework  

---

## 🛠️ Nguyên tắc thiết kế & Design Patterns

### 1. SOLID Principles
- **SRP (Single Responsibility):** Mỗi lớp chỉ thực hiện một nhiệm vụ duy nhất.  
- **OCP (Open/Closed):** Hệ thống có thể mở rộng mà không cần sửa mã nguồn hiện có.  
- **DIP (Dependency Inversion):** Sử dụng Interfaces để giảm sự phụ thuộc giữa các module.  

### 2. Design Patterns áp dụng
- **Singleton Pattern:** Đảm bảo duy nhất một instance cho các dịch vụ chung như `LoggingService`.  
- **Factory Method Pattern:** `UserFactory` tạo các loại người dùng khác nhau, `CalculatorFactory` xử lý logic tính toán điểm.  
- **Repository Pattern:** Tách biệt logic truy cập dữ liệu khỏi lớp nghiệp vụ.  

---

## 🧪 Kiểm thử (Testing)

- **Unit Testing:** Kiểm tra thuật toán tính điểm và logic nghiệp vụ.  
- **Integration Testing:** Đảm bảo phối hợp giữa Web UI, Controller và Database.  
- **Kết quả:** 100% kịch bản kiểm thử thành công (Passed).  

---

---

## 👤 Thông tin tác giả

- **Sinh viên:** PHẠM XUÂN CUNG (BD00544)  , LÊ XUÂN THÀNH, NGUYEN XUAN THINH, LÊ QUỐC KHÁNH
- **Lớp:** SE07201  
- **Đơn vị:** Pearson BTEC Level 5 Higher National Diploma in Computing  
- **Giảng viên hướng dẫn:** ĐỖ TRUNG ANH

## 📂 Cấu trúc dự án
