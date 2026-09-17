<template>
  <q-page class="admin-dashboard-page">
    <!-- Editorial Header Card with Tasteful Gradient Top Accent -->
    <section class="dashboard-hero-card">
      <div class="hero-content">
        <div class="kicker-row">
          <span class="kicker-tag">
            <q-icon name="fa-solid fa-layer-group" size="11px" class="q-mr-xs" />
            HỆ THỐNG QUẢN TRỊ · TỔNG QUAN
          </span>
          <span class="status-indicator">
            <span class="indicator-dot"></span>
            <span>HỆ THỐNG HOẠT ĐỘNG</span>
          </span>
        </div>

        <h1 class="hero-title">Xin chào, {{ authStore.userDisplayName }}</h1>
        <p class="hero-description">
          Chào mừng bạn quay trở lại. Tại đây bạn có thể theo dõi tổng quan hệ thống, phân loại chủ đề kỹ thuật và quản
          lý các bài viết trên Portfolio.
        </p>

        <!-- Quick Editorial Actions with Color Accents -->
        <div class="hero-actions">
          <router-link to="/admin/posts" class="action-btn-solid">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Viết bài mới</span>
          </router-link>

          <router-link to="/admin/categories" class="action-btn-indigo">
            <q-icon name="fa-solid fa-layer-group" size="13px" />
            <span>Quản lý Danh mục</span>
          </router-link>

          <router-link to="/" class="action-btn-ghost">
            <span>Xem trang công khai</span>
            <q-icon name="fa-solid fa-arrow-up-right-from-square" size="12px" />
          </router-link>
        </div>
      </div>

      <!-- Author Identity Box with Rich Gradient -->
      <div class="author-identity-box">
        <div class="author-avatar-wrap">
          <img v-if="authStore.userAvatar" :src="authStore.userAvatar" alt="Avatar" />
          <span v-else class="avatar-letter">{{ userInitial }}</span>
        </div>
        <div class="author-meta">
          <div class="author-name">{{ authStore.userDisplayName }}</div>
          <div class="author-role-badge">{{ authStore.userRole }}</div>
          <div class="author-email">{{ authStore.user?.email }}</div>
        </div>
      </div>
    </section>

    <!-- Navigation & Quick Overview Tiles -->
    <section class="tiles-section">
      <div class="section-label-row">
        <span class="section-accent-bar"></span>
        <span class="section-label">TRUY CẬP NHANH CÁC MỤC QUẢN LÝ</span>
      </div>

      <div class="tiles-grid">
        <!-- Post Management Tile (Rose Theme) -->
        <router-link to="/admin/posts" class="management-tile tile-rose">
          <div class="tile-top">
            <div class="tile-icon-box icon-rose">
              <q-icon name="fa-solid fa-newspaper" size="18px" />
            </div>
            <span class="tile-badge badge-rose">Nội dung</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Quản lý Bài viết</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Soạn thảo, chỉnh sửa và xuất bản nội dung bài viết kỹ thuật.</div>
          </div>
        </router-link>

        <!-- Categories & Topics Tile (Indigo Theme) -->
        <router-link to="/admin/categories" class="management-tile tile-indigo">
          <div class="tile-top">
            <div class="tile-icon-box icon-indigo">
              <q-icon name="fa-solid fa-layer-group" size="18px" />
            </div>
            <span class="tile-badge badge-indigo">Chủ đề</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Chủ đề & Danh mục</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Phân loại bài viết theo chuyên môn và công nghệ.</div>
          </div>
        </router-link>

        <!-- Author Profile Tile (Purple Theme) -->
        <router-link to="/admin/profile" class="management-tile tile-purple">
          <div class="tile-top">
            <div class="tile-icon-box icon-purple">
              <q-icon name="fa-solid fa-user-pen" size="18px" />
            </div>
            <span class="tile-badge badge-purple">Tác giả</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Hồ sơ Tác giả</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Cập nhật tiểu sử cá nhân, chức danh và các kênh mạng xã hội.</div>
          </div>
        </router-link>

        <!-- Security & Settings Tile (Emerald Theme) -->
        <router-link to="/admin/security" class="management-tile tile-emerald">
          <div class="tile-top">
            <div class="tile-icon-box icon-emerald">
              <q-icon name="fa-solid fa-shield-halved" size="18px" />
            </div>
            <span class="tile-badge badge-emerald">Bảo mật</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Bảo mật & Tài khoản</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Quản lý mật khẩu đăng nhập và thông tin phiên xác thực JWT.</div>
          </div>
        </router-link>
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

const authStore = useAuthStore()

const userInitial = computed(() => {
  const name = authStore.userDisplayName || 'A'
  return name.charAt(0).toUpperCase()
})
</script>

<style scoped lang="scss">
.admin-dashboard-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 32px;
}

/* Hero Card with Rose-to-Indigo Top Accent */
.dashboard-hero-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-top: 3px solid #df266a;
  border-radius: 16px;
  padding: 32px 36px;
  margin-bottom: 32px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 32px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.025);
  position: relative;
}

.hero-content {
  flex: 1;
  max-width: 680px;

  .kicker-row {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 14px;
    flex-wrap: wrap;

    .kicker-tag {
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.06em;
      color: #df266a;
      background: #fdf2f6;
      border: 1px solid rgba(223, 38, 106, 0.2);
      padding: 3px 10px;
      border-radius: 9999px;
      text-transform: uppercase;
      display: inline-flex;
      align-items: center;
    }

    .status-indicator {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      color: #059669;
      background: #ecfdf5;
      border: 1px solid #a7f3d0;
      padding: 3px 10px;
      border-radius: 9999px;

      .indicator-dot {
        width: 6px;
        height: 6px;
        border-radius: 50%;
        background: #10b981;
        box-shadow: 0 0 6px rgba(16, 185, 129, 0.5);
      }
    }
  }

  .hero-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 26px;
    font-weight: 800;
    color: var(--text-primary, #0b1326);
    letter-spacing: -0.025em;
    margin: 0 0 10px;
    line-height: 1.25;
  }

  .hero-description {
    font-family: var(--font-body, sans-serif);
    font-size: 14.5px;
    color: var(--text-secondary, #475569);
    line-height: 1.6;
    margin: 0 0 24px;
  }

  .hero-actions {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;

    .action-btn-solid {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: #ffffff;
      background: #df266a;
      padding: 9px 18px;
      border-radius: 9999px;
      text-decoration: none;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.28);
      transition: all 0.2s ease;

      &:hover {
        background: #be185d;
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
      }
    }

    .action-btn-indigo {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: #818cf8;
      background: rgba(99, 102, 241, 0.15);
      border: 1px solid rgba(99, 102, 241, 0.3);
      padding: 9px 16px;
      border-radius: 9999px;
      text-decoration: none;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(99, 102, 241, 0.25);
        border-color: #818cf8;
        transform: translateY(-1px);
      }
    }

    .action-btn-ghost {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 600;
      color: var(--text-muted, #64748b);
      padding: 9px 12px;
      text-decoration: none;
      transition: all 0.2s ease;

      &:hover {
        color: #df266a;
      }
    }
  }
}

.author-identity-box {
  display: flex;
  align-items: center;
  gap: 16px;
  background: var(--bg-surface-low, #f8fafc);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 14px;
  padding: 18px 22px;
  min-width: 250px;

  .author-avatar-wrap {
    width: 48px;
    height: 48px;
    border-radius: 12px;
    background: linear-gradient(135deg, #df266a 0%, #7c3aed 100%);
    color: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 800;
    font-size: 18px;
    flex-shrink: 0;
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.22);

    img {
      width: 100%;
      height: 100%;
      border-radius: 12px;
      object-fit: cover;
    }
  }

  .author-meta {
    display: flex;
    flex-direction: column;

    .author-name {
      font-size: 14.5px;
      font-weight: 700;
      color: var(--text-primary, #0b1326);
    }

    .author-role-badge {
      font-family: var(--font-mono, monospace);
      font-size: 10px;
      font-weight: 700;
      color: #df266a;
      background: rgba(223, 38, 106, 0.12);
      border: 1px solid rgba(223, 38, 106, 0.2);
      padding: 1px 6px;
      border-radius: 4px;
      display: inline-block;
      width: fit-content;
      letter-spacing: 0.05em;
      margin: 2px 0;
      text-transform: uppercase;
    }

    .author-email {
      font-size: 11.5px;
      color: var(--text-muted, #64748b);
    }
  }
}

/* Management Tiles Section */
.tiles-section {
  .section-label-row {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 16px;

    .section-accent-bar {
      width: 4px;
      height: 14px;
      border-radius: 2px;
      background: #df266a;
    }

    .section-label {
      font-family: var(--font-mono, monospace);
      font-size: 11.5px;
      font-weight: 700;
      letter-spacing: 0.06em;
      color: var(--text-muted, #64748b);
      text-transform: uppercase;
    }
  }

  .tiles-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
    gap: 20px;
  }
}

.management-tile {
  background: var(--bg-surface-low, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 14px;
  padding: 22px 24px;
  display: flex;
  flex-direction: column;
  text-decoration: none;
  transition: all 0.22s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);

  .tile-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;

    .tile-icon-box {
      width: 44px;
      height: 44px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s ease;

      &.icon-rose {
        background: rgba(223, 38, 106, 0.12);
        color: #df266a;
        border: 1px solid rgba(223, 38, 106, 0.25);
      }

      &.icon-indigo {
        background: rgba(99, 102, 241, 0.12);
        color: #818cf8;
        border: 1px solid rgba(99, 102, 241, 0.25);
      }

      &.icon-purple {
        background: rgba(168, 85, 247, 0.12);
        color: #c084fc;
        border: 1px solid rgba(168, 85, 247, 0.25);
      }

      &.icon-emerald {
        background: rgba(16, 185, 129, 0.12);
        color: #10b981;
        border: 1px solid rgba(16, 185, 129, 0.25);
      }
    }

    .tile-badge {
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      padding: 3px 8px;
      border-radius: 6px;

      &.badge-rose {
        background: rgba(223, 38, 106, 0.12);
        color: #df266a;
        border: 1px solid rgba(223, 38, 106, 0.25);
      }

      &.badge-indigo {
        background: rgba(99, 102, 241, 0.12);
        color: #818cf8;
        border: 1px solid rgba(99, 102, 241, 0.25);
      }

      &.badge-purple {
        background: rgba(168, 85, 247, 0.12);
        color: #c084fc;
        border: 1px solid rgba(168, 85, 247, 0.25);
      }

      &.badge-emerald {
        background: rgba(16, 185, 129, 0.12);
        color: #10b981;
        border: 1px solid rgba(16, 185, 129, 0.25);
      }
    }
  }

  .tile-body {
    .tile-title-row {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 6px;

      .tile-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 15.5px;
        font-weight: 700;
        color: var(--text-primary, #0b1326);
        letter-spacing: -0.01em;
        transition: color 0.2s ease;
      }

      .tile-arrow {
        color: var(--text-muted, #cbd5e1);
        transition: all 0.2s ease;
      }
    }

    .tile-desc {
      font-family: var(--font-body, sans-serif);
      font-size: 13px;
      color: var(--text-muted, #64748b);
      line-height: 1.5;
    }
  }

  /* Specific Hover Accents per Tile */
  &.tile-rose:hover {
    border-color: #f43f5e;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(244, 63, 94, 0.12);

    .tile-title {
      color: #df266a;
    }

    .tile-arrow {
      color: #df266a;
      transform: translateX(4px);
    }
  }

  &.tile-indigo:hover {
    border-color: #6366f1;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(99, 102, 241, 0.12);

    .tile-title {
      color: #4f46e5;
    }

    .tile-arrow {
      color: #4f46e5;
      transform: translateX(4px);
    }
  }

  &.tile-purple:hover {
    border-color: #a855f7;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(168, 85, 247, 0.12);

    .tile-title {
      color: #9333ea;
    }

    .tile-arrow {
      color: #9333ea;
      transform: translateX(4px);
    }
  }

  &.tile-emerald:hover {
    border-color: #10b981;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(16, 185, 129, 0.12);

    .tile-title {
      color: #059669;
    }

    .tile-arrow {
      color: #059669;
      transform: translateX(4px);
    }
  }
}

@media (max-width: 900px) {
  .dashboard-hero-card {
    flex-direction: column;
    align-items: flex-start;
    padding: 24px;
    gap: 20px;
  }

  .author-identity-box {
    width: 100%;
  }
}
</style>
