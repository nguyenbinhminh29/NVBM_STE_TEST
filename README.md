# 🛒 NVBM: Hệ Thống Bán Lẻ Thực Phẩm (Grocery Retail Backend System)

**NVBM** là một giải pháp backend quản lý bán lẻ đa nền tảng, được xây dựng dựa trên kiến trúc Microservices hiện đại. Dự án được thiết kế đặc biệt để đáp ứng các yêu cầu khắt khe về hiệu năng, tính mở rộng linh hoạt và khả năng bảo trì dài hạn trong môi trường doanh nghiệp quy mô doanh nghiệp chuyên nghiệp.

---

## 💻 Nền Tảng Công Nghệ (Technology Platform)

Hệ thống tận dụng sức mạnh của hệ sinh thái .NET mới và hiện đại nhất:
- **.NET 9 SDK (C# 13):** Tận dụng tối đa các tính năng ngôn ngữ mới và hiệu năng cực cao của runtime .NET mới nhất.
- **.NET Aspire:** Cung cấp giải pháp Cloud-native Orchestration mạnh mẽ. Aspire đơn giản hóa việc quản lý vòng đời của các Microservices, tự động cấu hình Service Discovery (định tuyến nội bộ) và cung cấp Dashboard giám sát (Observability) toàn diện.
- **YARP (Yet Another Reverse Proxy):** Đóng vai trò làm API Gateway, tiếp nhận đánh chặn và định tuyến (routing) request từ Client tới các microservices phía sau dựa trên cấu hình bảo mật.
- **Wolverine:** Được sử dụng làm kiến trúc Message Bus nội bộ thay thế cho MediatR truyền thống. Wolverine mang lại mô hình xử lý Command/Query tinh gọn, giảm thiểu boilerplate code, độ trễ cực thấp (low-latency) và tích hợp hoàn hảo với hệ thống FluentValidation.
- **Entity Framework Core 9 & SQL Server:** Xử lý Object-Relational Mapping (ORM) với hiệu suất cao, hỗ trợ Query Filters, đảm bảo tính nhất quán dữ liệu (Strong Consistency) và kiểm soát đồng thời.
- **Redis Output Caching:** Quản lý caching (bộ nhớ tạm) với hiệu suất tốc độ cao, tối ưu hoá thời gian phản hồi cho các truy vấn dữ liệu ít thay đổi.
- **Scalar / OpenAPI:** Cung cấp tài liệu hoá API đẹp mắt, hiệu suất cao để các module hoặc team khác có thể tương tác testing API dễ dàng (Vượt trội và nhanh hơn Swagger UI truyền thống).

---

## 📈 Quy Mô Phục Vụ (Scale of Serving)

Hệ thống được định hướng kiến trúc để sẵn sàng phục vụ quy mô từ chuỗi siêu thị vừa đến rất lớn:
- **Khối lượng giao dịch cao:** Việc triển khai kiến trúc **CQRS** giúp tách biệt rõ ràng luồng đọc (Read) và ghi (Write), đảm bảo các tác vụ Query nặng không làm giảm thông lượng (throughput) của các tác vụ ghi/chỉnh sửa (Command).
- **Tính sẵn sàng cao (High Availability):** Thiết kế Microservices cho phép từng mảng nghiệp vụ (VD: Catalog, Inventory, Shift, Promotion) có thể được cô lập rủi ro và **scale ngang (horizontal scaling) độc lập** tùy theo nhu cầu tải của hệ thống.
- **Xử lý đồng thời (Concurrency Management):** Hệ thống có thể xử lý mượt mà các tình huống cạnh tranh dữ liệu (Ví dụ: nhiều thu ngân cùng trừ tổn kho một mặt hàng đồng thời hay thanh toán) thông qua cơ chế kiểm soát lạc quan (Optimistic Concurrency với RowVersion) ở tầng CSDL.
- **Tính bền bỉ mạng lưới (Resilience):** Hệ thống tích hợp sẵn các tiêu chuẩn phục hồi kết nối tự động (Standard Resilience Handlers của .NET Aspire) giúp hệ thống chủ động Retry, Circuit Breaker để đối phó liền mạch với tình trạng nghẽn mạng hoặc độ trễ dịch vụ.

---

## 🛠️ Cách Tiếp Cận Phát Triển (Development Approach)

Hệ thống tuân theo các nguyên lý phát triển Software Engineering tốt nhất:
1. **Kiến trúc vẹn toàn (Clean Architecture):** 
   Bộ khung được tổ chức chặt chẽ theo nguyên lý Dependency Inversion, cô lập hoàn toàn lớp Domain (chứa Business Logic/Entities) khỏi các phần phụ thuộc công nghệ như Framework, Database, hay Message broker (Infrastructure).
2. **Domain-Driven Design (DDD):**
   Mỗi Service được phân rã rạch ròi theo ngữ cảnh nghiệp vụ cốt lõi (Bounded Context), chẳng hạn như: Module quản lý ca làm việc (Shift) hoạt động độc lập với Module quản lý hàng hoá (Catalog). Điều này giúp code gọn nhẹ, không dính líu chằng chịt, thuận tiện cho nhiều teams cùng phát triển.
3. **Mỏng hóa Controller (Thin Controllers):**
   Thay vì nhét logic dày đặc vào API Controller, các Controller thu hẹp tối đa chỉ làm nhiệm vụ tiếp nhận HTTP Request rồi ủy quyền ngay cho Mediator/MessageBus (Wolverine) đẩy xuống lớp Handlers chuyên biệt để xử lý.
4. **Cloud-Native by Design:**
   Xây dựng ngay từ ban đầu với tư duy Cloud-Native, hệ thống tương thích 100% với container hóa và được nhúng sẵn luồng theo dõi Telemetry (Metrics, Tracing, Logging) qua định dạng chuẩn OpenTelemetry. Rất dễ dàng nếu muốn nâng cấp đẩy ứng dụng lên nền tảng Kubernetes hoặc Azure Container Apps.

---

## 🌟 Các Yếu Tố Kỹ Thuật Ưu Việt (Outstanding Technical Benefits)

Bên cạnh nền tảng công nghệ mạnh mẽ, hệ thống backend còn sở hữu những mô hình thiết kế nghiệp vụ (Business Model) và luồng xử lý kỹ thuật tối ưu mang tính doanh nghiệp (Enterprise-level):

1. **Hiệu suất (Performance) vượt trội:** Việc áp dụng C# 13, .NET 9 kết hợp với Wolverine giúp loại bỏ hoàn toàn độ trễ phản xạ (Reflection) thường thấy ở các hệ thống cũ, cho phép xử lý các payload JSON lớn với chu kỳ thu gom rác (Garbage Collection) thấp nhất lượng phân bổ bộ nhớ bằng "0".
2. **Khả năng quan sát (Observability):** Tích hợp sâu vào OpenTelemetry, giúp truy vết (Distributed Tracing) toàn bộ vòng đời của một Request xuyên qua Gateway -> API -> DB. Nếu có lỗi xảy ra ở hệ thống tính tiền, người quản trị có thể ngay lập tức nhìn ra nút thắt nằm ở dịch vụ nào trên Aspire Dashboard.
3. **Môi trường cục bộ nguyên khối (Dev-Env):** Chỉ với duy nhất một lệnh `dotnet run`, hệ thống sẽ tự động pull toàn bộ Gateway, các microservices, và Redis Cache lên hoạt động hoàn hảo, tiết kiệm hàng chục giờ setup môi trường cho developer.
4. **Mô hình Đa đơn vị tính & Mã vạch (Multi-UOM & Dynamic Barcodes):** Không giống các hệ thống bán lẻ cơ bản chỉ map 1 sản phẩm - 1 mã vạch. NVBM hỗ trợ quan hệ 1-N phức tạp: Một sản phẩm `Thùng Bia` có thể chia làm `Lốc` và `Lon`, mỗi UOM sở hữu các hệ số chuyển đổi, giá trị bán và một chuỗi nhiều **Mã vạch (Barcodes) khác nhau** hoạt động song song mà không bị Duplicate dữ liệu.
5. **Chương trình Khuyến mãi (Promotions):** Hệ thống không tính toán giảm giá tĩnh (static) tại Client, thay vào đó sở hữu một Promotion Microservices cho phép tính toán các rule bán hàng linh hoạt (VD: Buy 1 Get 1, Buy X% off Y) và áp dụng thời gian thực ngay khi quét giá POS, thiết kế theo pattern dạng Strategy dễ dàng gắn thêm chương trình khuyến mãi mới.
6. **Kiểm soát Tồn kho chính xác (Concurrency Inventory):** Thiết kế Inventory độc lập hoàn toàn, hỗ trợ Reserve/Update số lượng song song thông qua cơ chế chốt phiên bản dữ liệu (RowVersion Check & State Tracking), triệt tiêu hoàn toàn lỗi bán âm kho (Overselling) khi có hàng ngàn user hoặc nhiều thu ngân thao tác vào cùng 1 mặt hàng trong cùng 1 mili-giây.
7. **Bảo toàn Số dư Ca làm việc (Shift Checkpoint):** Thiết kế cho hệ thống POS với cơ chế Đóng/Mở ca rõ ràng, quản lý chặt các giao dịch In/Out, theo dõi lượng Tiền Mặt đầu ngày (Starting Cash) và tự đối soát, tính toán độ chênh lệch tiền thu thực tế (Variance) ở cuối phiên.

---

## 🚀 Hướng Dẫn Vận Hành

### Yêu cầu môi trường
- Máy có cài đặt **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**.
- SQL Server (Dùng bản LocalDB, SQLEXPRESS chuẩn hoặc thông qua Docker).
- Docker Desktop (Để chạy Redis Cache).

### Cấu hình Cơ Sở Dữ Liệu (SQL Server)
Để thay đổi theo máy chủ Database của bạn:
1. Mở file `src/NVBM.AppHost/appsettings.Development.json`.
2. Sửa lại chuỗi kết nối tại biến `"grocerydb"`. *(.NET Aspire sẽ tự động lấy chuỗi kết nối ở file AppHost này và tiêm xuống cho toàn bộ các Microservices bên dưới)*.
3. **Lưu ý quan trọng cho EF Core:** Hãy copy chuỗi kết nối vừa sửa dán đè trực tiếp vào các file `appsettings.Development.json` nằm trong từng API (Catalog.API, Inventory.API, Shift.API...) để hỗ trợ việc chạy Entity Framework Migrations cục bộ.

### Khởi động dự án
Hệ thống sử dụng dự án **.NET Aspire (AppHost)** quản lý toàn bộ quá trình build và chạy của các Microservices:

1. Mở PowerShell hoặc Terminal tại **thư mục gốc**.
2. Thực hiện Migration Database:
   ```bash
   dotnet ef migrations add AddNewDataNVBM --project src\NVBM.Infrastructure
   dotnet ef database update --project src\NVBM.Infrastructure --startup-project src\NVBM.Catalog.API
   ```
3. Thực thi lệnh khởi động Aspire:
   ```bash
   dotnet run --project src/NVBM.AppHost/NVBM.AppHost.csproj
   ```
4. Sau khi chạy, Hệ thống sẽ sinh ngẫu nhiên một đường dẫn bảo mật trỏ tới **Aspire Dashboard** (ví dụ: `https://localhost:17076`). Truy cập vào giao diện web này để xem danh sách toàn bộ các Web API đang chạy, biểu đồ CPU/RAM, các Log tập trung và flow Traces giữa các dịch vụ.
5. Mọi tương tác nghiệp vụ từ bên ngoài sẽ được cấu hình đi qua **YARP API Gateway** (địa chỉ lấy từ thông báo của Gateway trên Console, thường là `https://localhost:7122`).


