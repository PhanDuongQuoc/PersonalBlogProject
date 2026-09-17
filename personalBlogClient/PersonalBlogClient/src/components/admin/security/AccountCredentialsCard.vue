<template>
  <div class="security-card account-credentials-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-indigo">
          <q-icon name="fa-solid fa-id-card-clip" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Thông tin Tài khoản & Đăng nhập</h3>
          <p class="card-subtitle">Chi tiết định danh quản trị viên và phiên làm việc</p>
        </div>
      </div>
      <div class="header-right">
        <span class="status-indicator">
          <span class="status-dot"></span>
          <span class="status-text">ĐANG HOẠT ĐỘNG</span>
        </span>
      </div>
    </div>

    <!-- User Profile Header Summary -->
    <div class="user-hero-box">
      <div class="avatar-wrap">
        <img
          v-if="accountInfo?.avatarUrl"
          :src="accountInfo.avatarUrl"
          :alt="accountInfo.displayName || accountInfo.username"
          class="user-avatar-img"
        />
        <div v-else class="user-avatar-placeholder">
          {{ (accountInfo?.displayName || accountInfo?.username || 'A').charAt(0).toUpperCase() }}
        </div>
      </div>

      <div class="user-meta-info">
        <div class="user-display-name">
          {{ accountInfo?.displayName || accountInfo?.username }}
        </div>
        <div class="user-job-title">
          {{ accountInfo?.jobTitle || 'Quản trị viên Hệ thống Blog' }}
        </div>
        <div class="user-tags-row">
          <span class="role-pill">
            <q-icon name="fa-solid fa-shield-halved" size="10px" class="q-mr-xs" />
            {{ accountInfo?.role || 'Admin' }}
          </span>
          <span class="verified-pill">
            <q-icon name="fa-solid fa-circle-check" size="10px" class="q-mr-xs" />
            Đã xác thực
          </span>
        </div>
      </div>
    </div>

    <!-- Details Grid -->
    <div class="credentials-info-list">
      <!-- Item: Username -->
      <div class="info-row">
        <div class="info-label">
          <q-icon name="fa-solid fa-user" size="12px" class="info-icon text-grey-6" />
          <span>Tên đăng nhập</span>
        </div>
        <div class="info-value font-mono">
          {{ accountInfo?.username || '—' }}
        </div>
      </div>

      <!-- Item: Email -->
      <div class="info-row">
        <div class="info-label">
          <q-icon name="fa-solid fa-envelope" size="12px" class="info-icon text-grey-6" />
          <span>Email đăng nhập</span>
        </div>
        <div class="info-value font-mono">
          {{ accountInfo?.email || '—' }}
        </div>
      </div>

      <!-- Item: Created Date -->
      <div class="info-row">
        <div class="info-label">
          <q-icon name="fa-solid fa-calendar-plus" size="12px" class="info-icon text-grey-6" />
          <span>Ngày khởi tạo</span>
        </div>
        <div class="info-value">
          {{ formatDate(accountInfo?.createdAt) }}
        </div>
      </div>

      <!-- Item: Last Updated -->
      <div class="info-row">
        <div class="info-label">
          <q-icon name="fa-solid fa-clock-rotate-left" size="12px" class="info-icon text-grey-6" />
          <span>Cập nhật bảo mật</span>
        </div>
        <div class="info-value">
          {{ formatDate(accountInfo?.updatedAt) || 'Chưa cập nhật' }}
        </div>
      </div>
    </div>

    <!-- Active Session Snapshot -->
    <div class="session-snapshot-box">
      <div class="snapshot-header">
        <div class="snapshot-title">
          <q-icon name="fa-solid fa-laptop-code" size="13px" class="text-indigo-6 q-mr-xs" />
          <span>Phiên làm việc hiện tại</span>
        </div>
        <span class="current-device-badge">Thiết bị này</span>
      </div>
      <div class="snapshot-details">
        <div class="snapshot-item">
          <span class="snapshot-lbl">Trình duyệt:</span>
          <span class="snapshot-val">{{ accountInfo?.currentBrowser || 'Google Chrome' }}</span>
        </div>
        <div class="snapshot-item">
          <span class="snapshot-lbl">Địa chỉ IP:</span>
          <span class="snapshot-val font-mono">{{ accountInfo?.currentIp || '127.0.0.1' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AdminAccountInfo } from "@/types/admin-security";

defineProps<{
  accountInfo: AdminAccountInfo | null;
}>();

function formatDate(dateStr?: string | null): string {
  if (!dateStr) return "—";
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return "—";
  return d.toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
}
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
  margin-bottom: 22px;

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

      &.icon-indigo {
        background: rgba(79, 70, 229, 0.12);
        color: #6366f1;
        border: 1px solid rgba(79, 70, 229, 0.25);
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

  .status-indicator {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    background: rgba(16, 185, 129, 0.15);
    border: 1px solid rgba(16, 185, 129, 0.3);
    padding: 3px 8px;
    border-radius: 6px;

    .status-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background: #10b981;
      box-shadow: 0 0 0 2px rgba(16, 185, 129, 0.25);
    }

    .status-text {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      color: #10b981;
      letter-spacing: 0.04em;
    }
  }
}

/* User Hero Box */
.user-hero-box {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px 18px;
  background: var(--bg-surface-lowest, #f8fafc);
  border: 1px solid var(--border-hairline, #f1f5f9);
  border-radius: 12px;
  margin-bottom: 20px;

  .avatar-wrap {
    width: 54px;
    height: 54px;
    border-radius: 12px;
    overflow: hidden;
    flex-shrink: 0;
    border: 2px solid var(--border-hairline, #ffffff);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);

    .user-avatar-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .user-avatar-placeholder {
      width: 100%;
      height: 100%;
      background: linear-gradient(135deg, #4f46e5, #6366f1);
      color: #ffffff;
      display: flex;
      align-items: center;
      justify-content: center;
      font-family: var(--font-headline, sans-serif);
      font-size: 22px;
      font-weight: 800;
    }
  }

  .user-meta-info {
    flex: 1;
    min-width: 0;

    .user-display-name {
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
      margin-bottom: 2px;
    }

    .user-job-title {
      font-family: var(--font-body, sans-serif);
      font-size: 12.5px;
      color: var(--text-muted, #64748b);
      margin-bottom: 6px;
    }

    .user-tags-row {
      display: flex;
      align-items: center;
      gap: 6px;
      flex-wrap: wrap;

      .role-pill {
        font-family: var(--font-mono, monospace);
        font-size: 11px;
        font-weight: 700;
        background: rgba(99, 102, 241, 0.15);
        color: #818cf8;
        border: 1px solid rgba(99, 102, 241, 0.3);
        padding: 2px 8px;
        border-radius: 6px;
      }

      .verified-pill {
        font-family: var(--font-mono, monospace);
        font-size: 11px;
        font-weight: 700;
        background: rgba(16, 185, 129, 0.15);
        color: #34d399;
        border: 1px solid rgba(16, 185, 129, 0.3);
        padding: 2px 8px;
        border-radius: 6px;
      }
    }
  }
}

/* Credentials Info List */
.credentials-info-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 20px;

  .info-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 10px 14px;
    background: var(--bg-surface-lowest, #ffffff);
    border: 1px solid var(--border-hairline, #f1f5f9);
    border-radius: 8px;

    .info-label {
      display: flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-body, sans-serif);
      font-size: 13px;
      font-weight: 600;
      color: var(--text-secondary, #475569);
    }

    .info-value {
      font-family: var(--font-body, sans-serif);
      font-size: 13px;
      font-weight: 600;
      color: var(--text-primary, #0b1326);

      &.font-mono {
        font-family: var(--font-mono, monospace);
        font-size: 12.5px;
        color: var(--text-primary, #334155);
      }
    }
  }
}

/* Session Snapshot */
.session-snapshot-box {
  background: var(--bg-surface-lowest, #f8fafc);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 10px;
  padding: 14px 16px;

  .snapshot-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 8px;

    .snapshot-title {
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
      display: flex;
      align-items: center;
    }

    .current-device-badge {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      background: rgba(99, 102, 241, 0.15);
      color: #818cf8;
      padding: 1px 6px;
      border-radius: 4px;
    }
  }

  .snapshot-details {
    display: flex;
    align-items: center;
    gap: 16px;
    font-size: 12px;
    flex-wrap: wrap;

    .snapshot-item {
      display: flex;
      align-items: center;
      gap: 5px;

      .snapshot-lbl {
        color: var(--text-muted, #64748b);
      }

      .snapshot-val {
        color: var(--text-primary, #0b1326);
        font-weight: 600;

        &.font-mono {
          font-family: var(--font-mono, monospace);
        }
      }
    }
  }
}
</style>
