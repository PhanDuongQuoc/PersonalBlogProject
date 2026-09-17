<template>
  <div class="security-card security-tips-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-amber">
          <q-icon name="fa-solid fa-lightbulb" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Nguyên tắc Bảo mật Tài khoản</h3>
          <p class="card-subtitle">Hướng dẫn giữ an toàn cho hệ thống quản trị Blog</p>
        </div>
      </div>
    </div>

    <!-- Tips List -->
    <div class="tips-list">
      <div class="tip-item">
        <div class="tip-icon-dot icon-emerald">
          <q-icon name="fa-solid fa-check" size="10px" />
        </div>
        <div class="tip-content">
          <div class="tip-title">Đổi mật khẩu định kỳ</div>
          <div class="tip-desc">Nên thay đổi mật khẩu quản trị ít nhất 60-90 ngày một lần để phòng ngừa rò rỉ thông tin.</div>
        </div>
      </div>

      <div class="tip-item">
        <div class="tip-icon-dot icon-rose">
          <q-icon name="fa-solid fa-fingerprint" size="10px" />
        </div>
        <div class="tip-content">
          <div class="tip-title">Không sử dụng mật khẩu quen thuộc</div>
          <div class="tip-desc">Tránh dùng ngày sinh, số điện thoại hoặc mật khẩu đã dùng cho các mạng xã hội khác.</div>
        </div>
      </div>

      <div class="tip-item">
        <div class="tip-icon-dot icon-indigo">
          <q-icon name="fa-solid fa-globe" size="10px" />
        </div>
        <div class="tip-content">
          <div class="tip-title">Đăng xuất trên thiết bị công cộng</div>
          <div class="tip-desc">Luôn nhớ đăng xuất khỏi hệ thống sau khi hoàn thành quản trị nội dung trên máy tính lạ.</div>
        </div>
      </div>
    </div>

    <!-- Quick Reminder Banner -->
    <div class="security-reminder-banner">
      <q-icon name="fa-solid fa-shield-virus" size="20px" class="banner-icon" />
      <div class="banner-text">
        <strong>Tài khoản Quản trị Viên</strong> nắm toàn quyền kiểm soát nội dung và dữ liệu blog. Hãy giữ kín thông tin đăng nhập và không chia sẻ cho bất kỳ ai.
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Security tips and guidelines
</script>

<style scoped lang="scss">
.security-card {
  background: var(--bg-surface-low, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(11, 19, 38, 0.03);
  transition: all 0.25s ease;
  display: flex;
  flex-direction: column;

  &:hover {
    box-shadow: 0 8px 26px rgba(11, 19, 38, 0.06);
    border-color: var(--border-subtle, #cbd5e1);
  }
}

.card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 20px;

  .header-left {
    display: flex;
    align-items: center;
    gap: 12px;

    .header-icon-box {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;

      &.icon-amber {
        background: rgba(245, 158, 11, 0.12);
        color: #d97706;
        border: 1px solid rgba(245, 158, 11, 0.25);
      }
    }

    .card-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
      margin: 0 0 2px;
    }

    .card-subtitle {
      font-family: var(--font-body, sans-serif);
      font-size: 12.5px;
      color: var(--text-muted, #64748b);
      margin: 0;
    }
  }
}

.tips-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
  margin-bottom: 20px;

  .tip-item {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    padding: 12px 14px;
    background: var(--bg-surface-lowest, #f8fafc);
    border: 1px solid var(--border-hairline, #f1f5f9);
    border-radius: 10px;
    transition: all 0.2s ease;

    &:hover {
      background: var(--bg-surface-low, #fdf2f6);
      border-color: var(--border-subtle, #fce7f3);
    }

    .tip-icon-dot {
      width: 22px;
      height: 22px;
      border-radius: 6px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      margin-top: 1px;

      &.icon-emerald { background: rgba(16, 185, 129, 0.15); color: #10b981; }
      &.icon-rose { background: rgba(223, 38, 106, 0.15); color: #df266a; }
      &.icon-indigo { background: rgba(79, 70, 229, 0.15); color: #6366f1; }
    }

    .tip-content {
      flex: 1;

      .tip-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13.5px;
        font-weight: 700;
        color: var(--text-primary, #0b1326);
        margin-bottom: 2px;
      }

      .tip-desc {
        font-family: var(--font-body, sans-serif);
        font-size: 12px;
        color: var(--text-muted, #64748b);
        line-height: 1.45;
      }
    }
  }
}

.security-reminder-banner {
  display: flex;
  align-items: center;
  gap: 12px;
  background: rgba(245, 158, 11, 0.1);
  border: 1px solid rgba(245, 158, 11, 0.25);
  border-radius: 10px;
  padding: 12px 16px;

  .banner-icon {
    color: #f59e0b;
    flex-shrink: 0;
  }

  .banner-text {
    font-family: var(--font-body, sans-serif);
    font-size: 12px;
    color: #fde68a;
    line-height: 1.45;

    strong {
      color: #fbbf24;
    }
  }
}
</style>
