<template>
    <div class="privacy-container">
      <!-- Particle Background -->
      <div class="particle-background">
        <div v-for="i in 20" :key="i" class="particle"></div>
      </div>
  
      <!-- Main Content -->
      <section class="privacy-content">
        <h1 class="privacy-title">Chính Sách Bảo Mật</h1>
        <p class="privacy-subtitle">Cam kết bảo vệ thông tin của bạn tại GameTradeZone</p>
  
        <div class="privacy-sections">
          <!-- Section 1: Giới thiệu -->
          <div class="privacy-section">
            <h2 class="section-title">1. Giới Thiệu</h2>
            <p class="section-text">
              Chào mừng bạn đến với Chính sách Bảo mật của GameTradeZone. Chúng tôi tại GameTradeZone cam kết bảo vệ thông tin cá nhân của bạn và đảm bảo rằng mọi dữ liệu bạn chia sẻ với chúng tôi được xử lý một cách an toàn, minh bạch và tuân thủ các quy định pháp luật hiện hành. Chính sách này giải thích cách chúng tôi thu thập, sử dụng, bảo vệ và chia sẻ thông tin của bạn khi bạn sử dụng website hoặc dịch vụ của chúng tôi.
            </p>
          </div>
  
          <!-- Section 2: Thu thập thông tin -->
          <div class="privacy-section">
            <h2 class="section-title">2. Thu Thập Thông Tin</h2>
            <p class="section-text">
              Chúng tôi có thể thu thập các loại thông tin sau từ bạn:
              <ul class="section-list">
                <li><strong>Thông tin cá nhân:</strong> Tên, email, số điện thoại, địa chỉ (nếu cần), và thông tin thanh toán khi bạn đăng ký hoặc giao dịch trên trang.</li>
                <li><strong>Thông tin sử dụng:</strong> Dữ liệu về hoạt động của bạn trên website, như lịch sử tìm kiếm, giao dịch, và sở thích game.</li>
                <li><strong>Thông tin kỹ thuật:</strong> Địa chỉ IP, loại trình duyệt, cookie, và thông tin thiết bị để cải thiện trải nghiệm người dùng.</li>
              </ul>
              Thông tin này được thu thập qua các biểu mẫu, cookie, và công nghệ theo dõi mà chúng tôi sử dụng.
            </p>
          </div>
  
          <!-- Section 3: Sử dụng thông tin -->
          <div class="privacy-section">
            <h2 class="section-title">3. Sử Dụng Thông Tin</h2>
            <p class="section-text">
              Chúng tôi sử dụng thông tin của bạn cho các mục đích sau:
              <ul class="section-list">
                <li>Cung cấp và cải thiện dịch vụ, bao gồm giao dịch tài khoản và dịch vụ game.</li>
                <li>Gửi thông báo, cập nhật, và ưu đãi liên quan đến GameTradeZone.</li>
                <li>Phân tích và tối ưu hóa trải nghiệm người dùng trên website.</li>
                <li>Đảm bảo an toàn và tuân thủ pháp luật, chẳng hạn như phát hiện gian lận hoặc xử lý tranh chấp.</li>
              </ul>
            </p>
          </div>
  
          <!-- Section 4: Chia sẻ thông tin -->
          <div class="privacy-section">
            <h2 class="section-title">4. Chia Sẻ Thông Tin</h2>
            <p class="section-text">
              Chúng tôi không bán hoặc chia sẻ thông tin cá nhân của bạn với bên thứ ba trừ khi:
              <ul class="section-list">
                <li>Có sự đồng ý rõ ràng từ bạn.</li>
                <li>Được yêu cầu bởi pháp luật hoặc để bảo vệ quyền lợi của GameTradeZone.</li>
                <li>Chia sẻ với các đối tác cung cấp dịch vụ (như thanh toán, lưu trữ) nhưng chỉ trong phạm vi cần thiết và với cam kết bảo mật.</li>
              </ul>
            </p>
          </div>
  
          <!-- Section 5: Bảo mật -->
          <div class="privacy-section">
            <h2 class="section-title">5. Bảo Mật</h2>
            <p class="section-text">
              Chúng tôi sử dụng các biện pháp bảo mật tiên tiến, bao gồm mã hóa SSL, tường lửa, và các công cụ bảo vệ dữ liệu để đảm bảo thông tin của bạn được an toàn. Tuy nhiên, không có hệ thống nào hoàn toàn miễn nhiễm rủi ro, và chúng tôi khuyến khích bạn bảo vệ tài khoản của mình bằng mật khẩu mạnh và xác thực hai yếu tố.
            </p>
          </div>
  
          <!-- Section 6: Quyền của bạn -->
          <div class="privacy-section">
            <h2 class="section-title">6. Quyền Của Bạn</h2>
            <p class="section-text">
              Bạn có quyền:
              <ul class="section-list">
                <li>Yêu cầu truy cập, chỉnh sửa, hoặc xóa thông tin cá nhân của mình.</li>
                <li>Yêu cầu ngừng nhận thông báo hoặc quảng cáo từ chúng tôi.</li>
                <li>Phàn nàn hoặc khiếu nại nếu bạn cho rằng thông tin của mình bị xử lý không đúng quy định.</li>
              </ul>
              Vui lòng liên hệ với chúng tôi qua email <a href="mailto:privacy@gametradezone.com" class="privacy-link">privacy@gametradezone.com</a> để thực hiện các quyền này.
            </p>
          </div>
  
          <!-- Section 7: Cookie và công nghệ theo dõi -->
          <div class="privacy-section">
            <h2 class="section-title">7. Cookie và Công Nghệ Theo Dõi</h2>
            <p class="section-text">
              Chúng tôi sử dụng cookie và các công nghệ theo dõi khác để cải thiện trải nghiệm của bạn. Bạn có thể quản lý cookie trong trình duyệt của mình, nhưng việc vô hiệu hóa có thể ảnh hưởng đến một số tính năng của website.
            </p>
          </div>
  
          <!-- Section 8: Liên hệ -->
          <div class="privacy-section">
            <h2 class="section-title">8. Liên Hệ</h2>
            <p class="section-text">
              Nếu bạn có bất kỳ câu hỏi nào về chính sách này hoặc cách chúng tôi xử lý thông tin của bạn, vui lòng liên hệ với chúng tôi qua:
              <ul class="section-list">
                <li>Email: <a href="mailto:privacy@gametradezone.com" class="privacy-link">privacy@gametradezone.com</a></li>
                <li>Điện thoại: +1-800-GAMEZONE</li>
                <li>Địa chỉ: GameTradeZone, Vũ Trụ Số 123, Hệ Mặt Trời Runeterra</li>
              </ul>
            </p>
          </div>
  
          <!-- Section 9: Cập nhật chính sách -->
          <div class="privacy-section">
            <h2 class="section-title">9. Cập Nhật Chính Sách</h2>
            <p class="section-text">
              Chúng tôi có thể cập nhật Chính sách Bảo mật này định kỳ để phản ánh sự thay đổi trong dịch vụ hoặc quy định pháp luật. Chúng tôi sẽ thông báo cho bạn qua email hoặc thông báo trên website trước khi áp dụng các thay đổi quan trọng. Phiên bản mới nhất sẽ được đăng tại đây, với ngày cập nhật cuối cùng.
            </p>
          </div>
        </div>
  
        <!-- Footer của trang chính sách -->
        <footer class="privacy-footer">
          <p class="footer-text">Chính sách Bảo mật này được cập nhật lần cuối vào: 26/02/2025</p>
          <p class="footer-text">© 2025 GameTradeZone. Tất cả quyền được bảo lưu.</p>
        </footer>
      </section>
    </div>
  </template>
  
  <style scoped>
  /* Tổng thể */
  .privacy-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Đồng bộ gradient với navbar */
    font-family: 'Arial', sans-serif; /* Đồng bộ font với navbar */
    color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    position: relative;
    overflow: hidden;
  }
  
  /* Particle Background */
  .particle-background {
    position: absolute;
    inset: 0;
    z-index: -1;
    background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Giữ gradient nhưng đồng bộ */
  }
  
  .particle {
    position: absolute;
    width: 5px;
    height: 5px;
    background: rgba(0, 179, 224, 0.5); /* Màu cyan, đồng bộ với navbar */
    border-radius: 50%;
    animation: float 10s infinite ease-in-out;
  }
  
  .particle:nth-child(odd) {
    background: rgba(255, 0, 255, 0.5); /* Màu magenta, đồng bộ với navbar */
  }
  
  .particle:nth-child(1) { left: 10%; top: 20%; animation-duration: 12s; }
  .particle:nth-child(2) { left: 20%; top: 80%; animation-duration: 15s; }
  .particle:nth-child(3) { left: 30%; top: 50%; animation-duration: 8s; }
  .particle:nth-child(4) { left: 40%; top: 10%; animation-duration: 10s; }
  .particle:nth-child(5) { left: 50%; top: 70%; animation-duration: 13s; }
  .particle:nth-child(6) { left: 60%; top: 30%; animation-duration: 9s; }
  .particle:nth-child(7) { left: 70%; top: 90%; animation-duration: 11s; }
  .particle:nth-child(8) { left: 80%; top: 40%; animation-duration: 14s; }
  .particle:nth-child(9) { left: 90%; top: 60%; animation-duration: 7s; }
  .particle:nth-child(10) { left: 15%; top: 25%; animation-duration: 16s; }
  .particle:nth-child(11) { left: 25%; top: 85%; animation-duration: 12s; }
  .particle:nth-child(12) { left: 35%; top: 45%; animation-duration: 10s; }
  .particle:nth-child(13) { left: 45%; top: 15%; animation-duration: 8s; }
  .particle:nth-child(14) { left: 55%; top: 75%; animation-duration: 13s; }
  .particle:nth-child(15) { left: 65%; top: 35%; animation-duration: 9s; }
  .particle:nth-child(16) { left: 75%; top: 95%; animation-duration: 11s; }
  .particle:nth-child(17) { left: 85%; top: 55%; animation-duration: 14s; }
  .particle:nth-child(18) { left: 95%; top: 65%; animation-duration: 7s; }
  .particle:nth-child(19) { left: 5%; top: 40%; animation-duration: 15s; }
  .particle:nth-child(20) { left: 15%; top: 60%; animation-duration: 10s; }
  
  @keyframes float {
    0% { transform: translateY(0) scale(1); opacity: 0.8; }
    50% { transform: translateY(-100vh) scale(1.5); opacity: 0.3; }
    100% { transform: translateY(0) scale(1); opacity: 0.8; }
  }
  
  /* Main Content */
  .privacy-content {
    max-width: 1200px;
    margin: 0 auto;
    padding: 60px 40px;
    position: relative;
    z-index: 1;
  }
  
  /* Titles */
  .privacy-title {
    font-size: 3.5rem;
    font-weight: 800;
    color: #f8f8f8; /* Màu chữ kem nhạt, đồng bộ với navbar */
    text-align: center;
    margin-bottom: 20px;
    text-shadow: 0 0 15px #00b3e0, 0 0 5px #ff00ff; /* Glow giống navbar */
    transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55); /* Đồng bộ transition với navbar */
  }
  
  .privacy-title:hover {
    transform: scale(1.05); /* Hiệu ứng zoom nhẹ, đồng bộ với navbar */
  }
  
  .privacy-subtitle {
    font-size: 1.4rem;
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    text-align: center;
    margin-bottom: 40px;
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
  }
  
  /* Sections */
  .privacy-sections {
    display: flex;
    flex-direction: column;
    gap: 40px;
  }
  
  .privacy-section {
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
    border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
    padding: 20px;
    box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
    transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  }
  
  .privacy-section:hover {
    transform: translateY(-5px); /* Hiệu ứng nâng nhẹ, đồng bộ với navbar */
    box-shadow: 0 0 20px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
  }
  
  .section-title {
    font-size: 1.8rem;
    font-weight: 700;
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    margin-bottom: 15px;
    text-shadow: 0 0 10px #00b3e0, 0 0 5px #ff00ff; /* Glow giống navbar */
    transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  }
  
  .section-title:hover {
    transform: scale(1.05); /* Hiệu ứng zoom nhẹ, đồng bộ với navbar */
  }
  
  .section-text {
    font-size: 1.1rem;
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    line-height: 1.6;
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
  }
  
  .section-list {
    list-style: none;
    padding-left: 20px;
    margin: 10px 0;
  }
  
  .section-list li {
    position: relative;
    padding-left: 15px;
    margin-bottom: 10px;
  }
  
  .section-list li::before {
    content: '•';
    position: absolute;
    left: 0;
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    font-size: 1.2rem;
  }
  
  .privacy-link {
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    text-decoration: none;
    transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  }
  
  .privacy-link:hover {
    color: #ff00ff; /* Màu magenta, đồng bộ với navbar */
    text-shadow: 0 0 5px #ff00ff; /* Glow khi hover, đồng bộ với navbar */
  }
  
  /* Footer */
  .privacy-footer {
    margin-top: 40px;
    text-align: center;
    padding: 20px;
    background: rgba(13, 27, 42, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
    border-radius: 8px;
    box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
  }
  
  .footer-text {
    font-size: 1rem;
    color: #b0b0b0; /* Màu chữ xám nhạt hơn, đồng bộ với navbar */
    margin: 5px 0;
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
  }
  
  /* Responsive */
  @media (max-width: 768px) {
    .privacy-content {
      padding: 30px 20px;
    }
  
    .privacy-title {
      font-size: 2.5rem;
    }
  
    .privacy-subtitle {
      font-size: 1.2rem;
    }
  
    .section-title {
      font-size: 1.5rem;
    }
  
    .section-text {
      font-size: 1rem;
    }
  
    .privacy-section {
      padding: 15px;
    }
  
    .footer-text {
      font-size: 0.9rem;
    }
  
    .particle { 
      width: 3px; 
      height: 3px; 
      animation-duration: 8s; 
    }
  }
  </style>