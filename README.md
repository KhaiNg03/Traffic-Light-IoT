
# Traffic-Light-IoT

## Mô tả dự án
Traffic-Light-IoT là một hệ thống điều khiển đèn giao thông thông minh dựa trên IoT. Hệ thống cho phép quản lý tín hiệu giao thông từ xa thông qua giao diện phần mềm và giao tiếp với các thiết bị phần cứng như Arduino, Raspberry Pi.

## Tính năng chính
- Điều khiển đèn giao thông từ xa.
- Lưu trữ và quản lý dữ liệu tín hiệu giao thông trong cơ sở dữ liệu SQLite.
- Giao tiếp giữa phần mềm và phần cứng qua giao thức Serial.
- Hiển thị trạng thái đèn theo thời gian thực.
- Tích hợp múi giờ Việt Nam (GMT+7) để lưu trữ dữ liệu chính xác.

## Công nghệ sử dụng
- **Ngôn ngữ lập trình**: C# (Windows Forms)
- **Cơ sở dữ liệu**: SQLite
- **Phần cứng hỗ trợ**: Arduino, Raspberry Pi
- **Giao tiếp**: Serial Communication (UART)

## Cài đặt và sử dụng
### Yêu cầu hệ thống
- Windows 10/11
- .NET Framework 4.7 trở lên
- SQLite Database
- Arduino với kết nối Serial

### Hướng dẫn cài đặt
1. **Clone repository**:
   ```bash
   git clone https://github.com/your-repo/Traffic-Light-IoT.git
   cd Traffic-Light-IoT
   ```
2. **Cài đặt thư viện cần thiết**:
   - Đối với .NET: Kiểm tra thư viện SerialPort đã được tích hợp.
   - Đối với Arduino: Sử dụng Arduino IDE để nạp firmware.
3. **Chạy ứng dụng**:
   - Mở file `TrafficLightIoT.sln` trong Visual Studio.
   - Build và chạy ứng dụng.
   - Kết nối với thiết bị phần cứng qua Serial.

## Cách sử dụng
1. Chọn cổng Serial kết nối với Arduino.
2. Chọn chế độ điều khiển (tự động hoặc thủ công).
3. Gửi tín hiệu điều khiển đèn.
4. Quan sát trạng thái đèn trên giao diện phần mềm.

## Đóng góp
Mọi đóng góp và cải tiến cho dự án đều được hoan nghênh. Hãy tạo Pull Request hoặc liên hệ với chúng tôi qua Issues trên GitHub.

## Nhóm phát triển
- **Nguyễn Quang Khải** - Project Leader
- Các thành viên khác trong nhóm.
