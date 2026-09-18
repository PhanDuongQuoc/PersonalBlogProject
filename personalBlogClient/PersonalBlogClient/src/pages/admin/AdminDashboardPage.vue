<template>
  <q-page class="admin-dashboard-page">
    <!-- 1. Editorial Header Card with Gradient Accent & Author Identity -->
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

        <h1 class="hero-title">Xin chào, {{ authStore.userDisplayName }} 👋</h1>
        <p class="hero-description">
          Chào mừng bạn quay trở lại trung tâm điều khiển. Tại đây bạn có thể quản lý toàn bộ nội dung bài viết, phân loại chủ đề, tương tác bình luận, hộp thư liên hệ và theo dõi thống kê tăng trưởng của Blog & Portfolio.
        </p>

        <!-- Quick Editorial Actions -->
        <div class="hero-actions">
          <router-link to="/admin/posts" class="action-btn-solid">
            <q-icon name="fa-solid fa-plus" size="13px" />
            <span>Viết bài mới</span>
          </router-link>

          <router-link to="/admin/analytics" class="action-btn-cyan">
            <q-icon name="fa-solid fa-chart-line" size="13px" />
            <span>Xem Báo cáo</span>
          </router-link>

          <router-link to="/admin/settings" class="action-btn-indigo">
            <q-icon name="fa-solid fa-sliders" size="13px" />
            <span>Cài đặt hệ thống</span>
          </router-link>

          <router-link to="/" class="action-btn-ghost">
            <span>Xem trang công khai</span>
            <q-icon name="fa-solid fa-arrow-up-right-from-square" size="12px" />
          </router-link>
        </div>
      </div>

      <!-- Author Identity Box -->
      <div class="author-identity-box">
        <div class="author-avatar-wrap">
          <img v-if="authStore.userAvatar" :src="authStore.userAvatar" alt="Avatar" />
          <span v-else class="avatar-letter">{{ userInitial }}</span>
        </div>
        <div class="author-meta">
          <div class="author-name">{{ authStore.userDisplayName }}</div>
          <div class="author-role-badge">{{ authStore.userRole }}</div>
          <div class="author-email">{{ authStore.user?.email || 'admin@pdq.dev' }}</div>
        </div>
      </div>
    </section>

    <!-- 2. Quick KPI Metrics Strip -->
    <section class="kpi-metrics-section">
      <div class="kpi-cards-grid">
        <!-- Metric: Posts -->
        <div class="kpi-metric-card card-rose">
          <div class="kpi-card-header">
            <div class="kpi-icon-box icon-rose">
              <q-icon name="fa-solid fa-newspaper" size="16px" />
            </div>
            <span class="kpi-badge badge-rose">Nội dung</span>
          </div>
          <div class="kpi-val">{{ summary?.totalPosts ?? 0 }}</div>
          <div class="kpi-label">Tổng số bài viết</div>
          <div class="kpi-sub-text">
            <span>{{ summary?.publishedPosts ?? 0 }} đã xuất bản</span>
            <span class="dot-sep">·</span>
            <span>{{ summary?.draftPosts ?? 0 }} bản nháp</span>
          </div>
        </div>

        <!-- Metric: Topics & Tags -->
        <div class="kpi-metric-card card-indigo">
          <div class="kpi-card-header">
            <div class="kpi-icon-box icon-indigo">
              <q-icon name="fa-solid fa-layer-group" size="16px" />
            </div>
            <span class="kpi-badge badge-indigo">Phân loại</span>
          </div>
          <div class="kpi-val">{{ summary?.totalCategories ?? 0 }}</div>
          <div class="kpi-label">Chủ đề & Danh mục</div>
          <div class="kpi-sub-text">
            <span>{{ summary?.totalTags ?? 0 }} thẻ bài viết (tags)</span>
          </div>
        </div>

        <!-- Metric: Comments -->
        <div class="kpi-metric-card card-teal">
          <div class="kpi-card-header">
            <div class="kpi-icon-box icon-teal">
              <q-icon name="fa-solid fa-comments" size="16px" />
            </div>
            <span class="kpi-badge badge-teal">Tương tác</span>
          </div>
          <div class="kpi-val">{{ summary?.totalComments ?? 0 }}</div>
          <div class="kpi-label">Bình luận độc giả</div>
          <div class="kpi-sub-text">
            <span>{{ summary?.approvedComments ?? 0 }} đã duyệt</span>
            <span v-if="(summary?.pendingComments ?? 0) > 0" class="pending-tag">
              ({{ summary?.pendingComments }} chờ duyệt)
            </span>
          </div>
        </div>

        <!-- Metric: Contacts / Inbox -->
        <div class="kpi-metric-card card-blue">
          <div class="kpi-card-header">
            <div class="kpi-icon-box icon-blue">
              <q-icon name="fa-solid fa-inbox" size="16px" />
            </div>
            <span class="kpi-badge badge-blue">Hộp thư</span>
          </div>
          <div class="kpi-val">{{ summary?.totalContacts ?? 0 }}</div>
          <div class="kpi-label">Tin nhắn liên hệ</div>
          <div class="kpi-sub-text">
            <span v-if="(summary?.unreadContacts ?? 0) > 0" class="unread-tag">
              {{ summary?.unreadContacts }} tin chưa đọc
            </span>
            <span v-else>Tất cả đã phản hồi</span>
          </div>
        </div>
      </div>
    </section>

    <!-- 3. Complete Navigation & Management Category Tiles Grid (10 Modules) -->
    <section class="tiles-section">
      <div class="section-label-row">
        <span class="section-accent-bar"></span>
        <span class="section-label">DANH MỤC QUẢN LÝ HỆ THỐNG</span>
      </div>

      <div class="tiles-grid">
        <!-- 1. Quản lý Bài viết (Rose) -->
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
            <div class="tile-desc">Soạn thảo, quản lý bản nháp, lên lịch và xuất bản bài viết kỹ thuật.</div>
          </div>
        </router-link>

        <!-- 2. Chủ đề & Danh mục (Indigo) -->
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
            <div class="tile-desc">Phân loại bài viết theo chuyên môn, công nghệ và phân mảng kiến thức.</div>
          </div>
        </router-link>

        <!-- 3. Thẻ bài viết / Tags (Amber) -->
        <router-link to="/admin/tags" class="management-tile tile-amber">
          <div class="tile-top">
            <div class="tile-icon-box icon-amber">
              <q-icon name="fa-solid fa-tags" size="18px" />
            </div>
            <span class="tile-badge badge-amber">Từ khóa</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Thẻ bài viết (Tags)</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Quản lý hệ thống thẻ đánh dấu, từ khóa công nghệ và liên kết bài viết.</div>
          </div>
        </router-link>

        <!-- 4. Bình luận độc giả (Teal) -->
        <router-link to="/admin/comments" class="management-tile tile-teal">
          <div class="tile-top">
            <div class="tile-icon-box icon-teal">
              <q-icon name="fa-solid fa-comments" size="18px" />
            </div>
            <span class="tile-badge badge-teal">Tương tác</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Bình luận độc giả</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Kiểm duyệt, phê duyệt phản hồi thảo luận và quản lý tương tác độc giả.</div>
          </div>
        </router-link>

        <!-- 5. Hộp thư liên hệ (Blue) -->
        <router-link to="/admin/contacts" class="management-tile tile-blue">
          <div class="tile-top">
            <div class="tile-icon-box icon-blue">
              <q-icon name="fa-solid fa-inbox" size="18px" />
            </div>
            <span class="tile-badge badge-blue">Hộp thư</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Hộp thư liên hệ</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Tiếp nhận, xử lý thư liên hệ hợp tác và tin nhắn phản hồi từ độc giả.</div>
          </div>
        </router-link>

        <!-- 6. Thống kê & Báo cáo (Cyan) -->
        <router-link to="/admin/analytics" class="management-tile tile-cyan">
          <div class="tile-top">
            <div class="tile-icon-box icon-cyan">
              <q-icon name="fa-solid fa-chart-line" size="18px" />
            </div>
            <span class="tile-badge badge-cyan">Báo cáo</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Thống kê & Báo cáo</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Theo dõi biểu đồ lượt xem, độc giả truy cập và xuất file báo cáo Excel/PPTX.</div>
          </div>
        </router-link>

        <!-- 7. Thông tin tác giả (Purple) -->
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
            <div class="tile-desc">Cập nhật tiểu sử cá nhân, chức danh, ảnh đại diện và các kênh mạng xã hội.</div>
          </div>
        </router-link>

        <!-- 8. Kỹ năng & Học vấn (Violet) -->
        <router-link to="/admin/skills" class="management-tile tile-violet">
          <div class="tile-top">
            <div class="tile-icon-box icon-violet">
              <q-icon name="fa-solid fa-graduation-cap" size="18px" />
            </div>
            <span class="tile-badge badge-violet">Chuyên môn</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Kỹ năng & Học vấn</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Quản lý cây kỹ năng công nghệ, chứng chỉ và lịch sử kinh nghiệm làm việc.</div>
          </div>
        </router-link>

        <!-- 9. Bảo mật & Tài khoản (Emerald) -->
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
            <div class="tile-desc">Đổi mật khẩu, quản lý phiên đăng nhập và cấu hình bảo mật tài khoản admin.</div>
          </div>
        </router-link>

        <!-- 10. Cài đặt hệ thống (Slate) -->
        <router-link to="/admin/settings" class="management-tile tile-slate">
          <div class="tile-top">
            <div class="tile-icon-box icon-slate">
              <q-icon name="fa-solid fa-sliders" size="18px" />
            </div>
            <span class="tile-badge badge-slate">Cấu hình</span>
          </div>
          <div class="tile-body">
            <div class="tile-title-row">
              <div class="tile-title">Cài đặt Hệ thống</div>
              <q-icon name="fa-solid fa-arrow-right" size="13px" class="tile-arrow" />
            </div>
            <div class="tile-desc">Cấu hình SEO, thông tin thương hiệu, kênh liên hệ và chế độ bảo trì website.</div>
          </div>
        </router-link>
      </div>
    </section>
  </q-page>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import type { AdminAnalyticsSummary } from '@/types/admin-analytics'
import { adminAnalyticsService } from '@/services/admin-analytics.service'

const authStore = useAuthStore()
const summary = ref<AdminAnalyticsSummary | null>(null)

const userInitial = computed(() => {
  const name = authStore.userDisplayName || 'A'
  return name.charAt(0).toUpperCase()
})

async function fetchSummary() {
  try {
    summary.value = await adminAnalyticsService.getSummary()
  } catch (err) {
    console.error('Failed to load dashboard analytics summary:', err)
  }
}

onMounted(() => {
  fetchSummary()
})
</script>

<style scoped lang="scss">
.admin-dashboard-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 36px;
}

/* 1. Hero Card */
.dashboard-hero-card {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-top: 3px solid #df266a;
  border-radius: 16px;
  padding: 30px 34px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 28px;
  box-shadow: var(--shadow-card, 0 4px 18px rgba(0, 0, 0, 0.25));
  position: relative;
}

.hero-content {
  flex: 1;
  max-width: 720px;

  .kicker-row {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 12px;
    flex-wrap: wrap;

    .kicker-tag {
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.06em;
      color: #df266a;
      background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
      border: 1px solid rgba(223, 38, 106, 0.25);
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
      color: #10b981;
      background: rgba(16, 185, 129, 0.12);
      border: 1px solid rgba(16, 185, 129, 0.25);
      padding: 3px 10px;
      border-radius: 9999px;

      .indicator-dot {
        width: 6px;
        height: 6px;
        border-radius: 50%;
        background: #10b981;
        box-shadow: 0 0 6px rgba(16, 185, 129, 0.6);
      }
    }
  }

  .hero-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 24px;
    font-weight: 800;
    color: var(--text-primary, #dae2fd);
    letter-spacing: -0.025em;
    margin: 0 0 8px;
    line-height: 1.25;
  }

  .hero-description {
    font-family: var(--font-body, sans-serif);
    font-size: 14px;
    color: var(--text-secondary, #94a3b8);
    line-height: 1.6;
    margin: 0 0 20px;
  }

  .hero-actions {
    display: flex;
    align-items: center;
    gap: 10px;
    flex-wrap: wrap;

    .action-btn-solid {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      color: #ffffff;
      background: #df266a;
      padding: 8px 16px;
      border-radius: 9999px;
      text-decoration: none;
      box-shadow: 0 4px 14px rgba(223, 38, 106, 0.28);
      transition: all 0.2s ease;

      &:hover {
        background: #f43f7e;
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(223, 38, 106, 0.4);
      }
    }

    .action-btn-cyan {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      color: #22d3ee;
      background: rgba(6, 182, 212, 0.12);
      border: 1px solid rgba(6, 182, 212, 0.25);
      padding: 8px 16px;
      border-radius: 9999px;
      text-decoration: none;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(6, 182, 212, 0.22);
        border-color: #22d3ee;
        transform: translateY(-1px);
      }
    }

    .action-btn-indigo {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      color: #818cf8;
      background: rgba(99, 102, 241, 0.12);
      border: 1px solid rgba(99, 102, 241, 0.25);
      padding: 8px 16px;
      border-radius: 9999px;
      text-decoration: none;
      transition: all 0.2s ease;

      &:hover {
        background: rgba(99, 102, 241, 0.22);
        border-color: #818cf8;
        transform: translateY(-1px);
      }
    }

    .action-btn-ghost {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 600;
      color: var(--text-muted, #64748b);
      padding: 8px 12px;
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
  gap: 14px;
  background: var(--bg-surface-container, #131b2e);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 14px;
  padding: 16px 20px;
  min-width: 240px;

  .author-avatar-wrap {
    width: 44px;
    height: 44px;
    border-radius: 12px;
    background: linear-gradient(135deg, #df266a 0%, #7c3aed 100%);
    color: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 800;
    font-size: 17px;
    flex-shrink: 0;
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.25);

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
      font-size: 14px;
      font-weight: 700;
      color: var(--text-primary, #dae2fd);
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
      font-size: 11px;
      color: var(--text-muted, #64748b);
    }
  }
}

/* 2. KPI Metrics Section */
.kpi-metrics-section {
  margin-bottom: 26px;

  .kpi-cards-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 16px;

    @media (max-width: 1100px) {
      grid-template-columns: repeat(2, 1fr);
    }

    @media (max-width: 580px) {
      grid-template-columns: 1fr;
    }
  }
}

.kpi-metric-card {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 14px;
  padding: 18px 20px;
  display: flex;
  flex-direction: column;
  box-shadow: var(--shadow-card, 0 2px 10px rgba(0, 0, 0, 0.15));
  transition: all 0.2s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: var(--shadow-card-hover, 0 8px 24px rgba(0, 0, 0, 0.25));
    border-color: var(--border-subtle, rgba(248, 250, 252, 0.15));
  }

  .kpi-card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 12px;

    .kpi-icon-box {
      width: 34px;
      height: 34px;
      border-radius: 9px;
      display: flex;
      align-items: center;
      justify-content: center;

      &.icon-rose { background: rgba(223, 38, 106, 0.12); color: #df266a; border: 1px solid rgba(223, 38, 106, 0.25); }
      &.icon-indigo { background: rgba(99, 102, 241, 0.12); color: #818cf8; border: 1px solid rgba(99, 102, 241, 0.25); }
      &.icon-teal { background: rgba(20, 184, 166, 0.12); color: #2dd4bf; border: 1px solid rgba(20, 184, 166, 0.25); }
      &.icon-blue { background: rgba(59, 130, 246, 0.12); color: #60a5fa; border: 1px solid rgba(59, 130, 246, 0.25); }
    }

    .kpi-badge {
      font-family: var(--font-mono, monospace);
      font-size: 10px;
      font-weight: 700;
      padding: 2px 7px;
      border-radius: 5px;

      &.badge-rose { background: rgba(223, 38, 106, 0.12); color: #df266a; }
      &.badge-indigo { background: rgba(99, 102, 241, 0.12); color: #818cf8; }
      &.badge-teal { background: rgba(20, 184, 166, 0.12); color: #2dd4bf; }
      &.badge-blue { background: rgba(59, 130, 246, 0.12); color: #60a5fa; }
    }
  }

  .kpi-val {
    font-family: var(--font-headline, sans-serif);
    font-size: 26px;
    font-weight: 800;
    color: var(--text-primary, #dae2fd);
    line-height: 1;
    margin-bottom: 4px;
    letter-spacing: -0.02em;
  }

  .kpi-label {
    font-family: var(--font-body, sans-serif);
    font-size: 12.5px;
    font-weight: 600;
    color: var(--text-secondary, #94a3b8);
    margin-bottom: 6px;
  }

  .kpi-sub-text {
    font-family: var(--font-mono, monospace);
    font-size: 11px;
    color: var(--text-muted, #64748b);
    display: flex;
    align-items: center;
    gap: 4px;
    flex-wrap: wrap;

    .dot-sep { opacity: 0.6; }
    .pending-tag { color: #f59e0b; font-weight: 600; }
    .unread-tag { color: #df266a; font-weight: 700; }
  }
}

/* 3. Management Tiles Section (Full 10 Modules) */
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
    grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
    gap: 18px;
  }
}

.management-tile {
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 14px;
  padding: 20px 22px;
  display: flex;
  flex-direction: column;
  text-decoration: none;
  transition: all 0.22s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: var(--shadow-card, 0 2px 8px rgba(0, 0, 0, 0.15));

  .tile-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 14px;

    .tile-icon-box {
      width: 42px;
      height: 42px;
      border-radius: 11px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s ease;

      &.icon-rose { background: rgba(223, 38, 106, 0.12); color: #df266a; border: 1px solid rgba(223, 38, 106, 0.25); }
      &.icon-indigo { background: rgba(99, 102, 241, 0.12); color: #818cf8; border: 1px solid rgba(99, 102, 241, 0.25); }
      &.icon-amber { background: rgba(245, 158, 11, 0.12); color: #fbbf24; border: 1px solid rgba(245, 158, 11, 0.25); }
      &.icon-teal { background: rgba(20, 184, 166, 0.12); color: #2dd4bf; border: 1px solid rgba(20, 184, 166, 0.25); }
      &.icon-blue { background: rgba(59, 130, 246, 0.12); color: #60a5fa; border: 1px solid rgba(59, 130, 246, 0.25); }
      &.icon-cyan { background: rgba(6, 182, 212, 0.12); color: #22d3ee; border: 1px solid rgba(6, 182, 212, 0.25); }
      &.icon-purple { background: rgba(168, 85, 247, 0.12); color: #c084fc; border: 1px solid rgba(168, 85, 247, 0.25); }
      &.icon-violet { background: rgba(139, 92, 246, 0.12); color: #a78bfa; border: 1px solid rgba(139, 92, 246, 0.25); }
      &.icon-emerald { background: rgba(16, 185, 129, 0.12); color: #10b981; border: 1px solid rgba(16, 185, 129, 0.25); }
      &.icon-slate { background: rgba(148, 163, 184, 0.12); color: #94a3b8; border: 1px solid rgba(148, 163, 184, 0.25); }
    }

    .tile-badge {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      padding: 2px 7px;
      border-radius: 6px;

      &.badge-rose { background: rgba(223, 38, 106, 0.12); color: #df266a; }
      &.badge-indigo { background: rgba(99, 102, 241, 0.12); color: #818cf8; }
      &.badge-amber { background: rgba(245, 158, 11, 0.12); color: #fbbf24; }
      &.badge-teal { background: rgba(20, 184, 166, 0.12); color: #2dd4bf; }
      &.badge-blue { background: rgba(59, 130, 246, 0.12); color: #60a5fa; }
      &.badge-cyan { background: rgba(6, 182, 212, 0.12); color: #22d3ee; }
      &.badge-purple { background: rgba(168, 85, 247, 0.12); color: #c084fc; }
      &.badge-violet { background: rgba(139, 92, 246, 0.12); color: #a78bfa; }
      &.badge-emerald { background: rgba(16, 185, 129, 0.12); color: #10b981; }
      &.badge-slate { background: rgba(148, 163, 184, 0.12); color: #94a3b8; }
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
        font-size: 15px;
        font-weight: 700;
        color: var(--text-primary, #dae2fd);
        letter-spacing: -0.01em;
        transition: color 0.2s ease;
      }

      .tile-arrow {
        color: var(--text-muted, #64748b);
        transition: all 0.2s ease;
      }
    }

    .tile-desc {
      font-family: var(--font-body, sans-serif);
      font-size: 12.5px;
      color: var(--text-secondary, #94a3b8);
      line-height: 1.5;
    }
  }

  /* Specific Hover Accents per Tile */
  &.tile-rose:hover {
    border-color: #df266a;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(223, 38, 106, 0.25);
    .tile-title, .tile-arrow { color: #df266a; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-indigo:hover {
    border-color: #6366f1;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(99, 102, 241, 0.25);
    .tile-title, .tile-arrow { color: #818cf8; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-amber:hover {
    border-color: #f59e0b;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(245, 158, 11, 0.25);
    .tile-title, .tile-arrow { color: #fbbf24; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-teal:hover {
    border-color: #14b8a6;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(20, 184, 166, 0.25);
    .tile-title, .tile-arrow { color: #2dd4bf; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-blue:hover {
    border-color: #3b82f6;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(59, 130, 246, 0.25);
    .tile-title, .tile-arrow { color: #60a5fa; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-cyan:hover {
    border-color: #06b6d4;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(6, 182, 212, 0.25);
    .tile-title, .tile-arrow { color: #22d3ee; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-purple:hover {
    border-color: #a855f7;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(168, 85, 247, 0.25);
    .tile-title, .tile-arrow { color: #c084fc; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-violet:hover {
    border-color: #8b5cf6;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(139, 92, 246, 0.25);
    .tile-title, .tile-arrow { color: #a78bfa; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-emerald:hover {
    border-color: #10b981;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(16, 185, 129, 0.25);
    .tile-title, .tile-arrow { color: #10b981; }
    .tile-arrow { transform: translateX(3px); }
  }

  &.tile-slate:hover {
    border-color: #94a3b8;
    transform: translateY(-2px);
    box-shadow: 0 10px 24px -4px rgba(148, 163, 184, 0.25);
    .tile-title, .tile-arrow { color: #cbd5e1; }
    .tile-arrow { transform: translateX(3px); }
  }
}

@media (max-width: 900px) {
  .dashboard-hero-card {
    flex-direction: column;
    align-items: flex-start;
    padding: 22px;
    gap: 18px;
  }

  .author-identity-box {
    width: 100%;
  }
}
</style>
