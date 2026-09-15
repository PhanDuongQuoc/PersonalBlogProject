<template>
  <q-layout view="lHh Lpr lFf" class="admin-layout">
    <!-- Admin Header -->
    <q-header class="admin-header">
      <q-toolbar class="admin-toolbar">
        <q-btn
          flat
          dense
          round
          icon="fa-solid fa-bars"
          aria-label="Toggle Sidebar"
          @click="leftDrawerOpen = !leftDrawerOpen"
          class="drawer-toggle-btn"
        />

        <!-- Show Brand in Header only when drawer is closed on desktop or on mobile -->
        <div class="header-left-section">
          <router-link v-if="!leftDrawerOpen" to="/admin" class="brand gt-sm q-mr-md">
            PDQ<span class="brand-suffix">Portfolio</span>
            <span class="brand-badge">ADMIN</span>
          </router-link>

          <div class="page-breadcrumb">
            <q-icon name="fa-solid fa-shield-halved" size="14px" class="breadcrumb-icon text-primary" />
            <span class="breadcrumb-title">{{ currentRouteTitle }}</span>
          </div>
        </div>

        <q-space />

        <!-- Quick actions & User menu -->
        <div class="header-actions">
          <!-- View Public Website Button -->
          <router-link to="/" class="view-site-link">
            <q-icon name="fa-solid fa-arrow-up-right-from-square" size="13px" />
            <span class="gt-xs">Xem Blog</span>
          </router-link>

          <q-separator vertical class="header-sep" />

          <!-- User Profile Dropdown -->
          <div class="user-chip-menu">
            <q-btn flat no-caps class="user-btn">
              <q-avatar size="34px" class="user-avatar">
                <img v-if="authStore.userAvatar" :src="authStore.userAvatar" alt="Avatar" />
                <span v-else class="avatar-fallback">{{ userInitials }}</span>
              </q-avatar>
              <div class="user-meta gt-sm">
                <span class="user-name">{{ authStore.userDisplayName }}</span>
                <span class="user-role">{{ authStore.userRole }}</span>
              </div>
              <q-icon name="fa-solid fa-chevron-down" size="10px" class="gt-xs text-grey-7" />

              <!-- User Menu Dropdown -->
              <q-menu anchor="bottom right" self="top right" class="user-dropdown-menu">
                <div class="dropdown-header">
                  <div class="user-full-name">{{ authStore.userDisplayName }}</div>
                  <div class="user-full-email">{{ authStore.user?.email || 'admin@pdq.dev' }}</div>
                </div>
                <q-separator class="q-my-xs" />
                <q-list dense class="dropdown-list">
                  <q-item clickable v-close-popup to="/admin/profile" class="dropdown-item">
                    <q-item-section avatar class="item-avatar">
                      <q-icon name="fa-solid fa-user-pen" size="14px" />
                    </q-item-section>
                    <q-item-section class="item-label">Hồ sơ tác giả</q-item-section>
                  </q-item>
                  <q-item clickable v-close-popup to="/admin/security" class="dropdown-item">
                    <q-item-section avatar class="item-avatar">
                      <q-icon name="fa-solid fa-key" size="14px" />
                    </q-item-section>
                    <q-item-section class="item-label">Đổi mật khẩu & Bảo mật</q-item-section>
                  </q-item>
                  <q-item clickable v-close-popup to="/" class="dropdown-item">
                    <q-item-section avatar class="item-avatar">
                      <q-icon name="fa-solid fa-globe" size="14px" />
                    </q-item-section>
                    <q-item-section class="item-label">Về trang chủ Blog</q-item-section>
                  </q-item>
                  <q-separator class="q-my-xs" />
                  <q-item clickable v-close-popup @click="handleLogout" class="dropdown-item logout-item">
                    <q-item-section avatar class="item-avatar">
                      <q-icon name="fa-solid fa-right-from-bracket" size="14px" />
                    </q-item-section>
                    <q-item-section class="item-label">Đăng xuất</q-item-section>
                  </q-item>
                </q-list>
              </q-menu>
            </q-btn>
          </div>
        </div>
      </q-toolbar>
    </q-header>

    <!-- Admin Drawer / Sidebar -->
    <q-drawer
      v-model="leftDrawerOpen"
      show-if-above
      bordered
      :width="265"
      class="admin-drawer"
    >
      <!-- Sidebar Brand Header -->
      <div class="drawer-header">
        <router-link to="/admin" class="brand">
          PDQ<span class="brand-suffix">Portfolio</span>
          <span class="brand-badge">ADMIN</span>
        </router-link>
      </div>

      <q-separator class="drawer-sep" />

      <!-- Sidebar Navigation Menu with all Management categories -->
      <q-scroll-area class="drawer-scroll-area">
        <q-list class="drawer-menu">
          <!-- SECTION: OVERVIEW -->
          <q-item-label header class="menu-header">TỔNG QUAN</q-item-label>
          <q-item clickable to="/admin" exact active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-pink">
                <q-icon name="fa-solid fa-table-columns" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Bảng điều khiển</q-item-section>
          </q-item>

          <q-item clickable to="/admin/analytics" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-cyan">
                <q-icon name="fa-solid fa-chart-line" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Thống kê & Báo cáo</q-item-section>
          </q-item>

          <!-- SECTION: CONTENT MANAGEMENT -->
          <q-item-label header class="menu-header q-mt-md">QUẢN LÝ NỘI DUNG</q-item-label>
          <q-item clickable to="/admin/posts" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-rose">
                <q-icon name="fa-solid fa-newspaper" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Quản lý Bài viết</q-item-section>
          </q-item>

          <q-item clickable to="/admin/categories" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-indigo">
                <q-icon name="fa-solid fa-layer-group" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Chủ đề & Danh mục</q-item-section>
          </q-item>

          <q-item clickable to="/admin/tags" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-amber">
                <q-icon name="fa-solid fa-tags" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Thẻ bài viết (Tags)</q-item-section>
          </q-item>

          <q-item clickable to="/admin/comments" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-teal">
                <q-icon name="fa-solid fa-comments" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Bình luận độc giả</q-item-section>
          </q-item>

          <!-- SECTION: AUTHOR & PROFILE -->
          <q-item-label header class="menu-header q-mt-md">HỒ SƠ & TÁC GIẢ</q-item-label>
          <q-item clickable to="/admin/profile" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-purple">
                <q-icon name="fa-solid fa-user-pen" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Thông tin tác giả</q-item-section>
          </q-item>

          <q-item clickable to="/admin/skills" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-violet">
                <q-icon name="fa-solid fa-graduation-cap" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Kỹ năng & Học vấn</q-item-section>
          </q-item>

          <!-- SECTION: SYSTEM & SECURITY -->
          <q-item-label header class="menu-header q-mt-md">HỆ THỐNG & BẢO MẬT</q-item-label>
          <q-item clickable to="/admin/security" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-emerald">
                <q-icon name="fa-solid fa-shield-halved" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Bảo mật & Tài khoản</q-item-section>
          </q-item>

          <q-item clickable to="/admin/settings" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-slate">
                <q-icon name="fa-solid fa-sliders" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Cài đặt hệ thống</q-item-section>
          </q-item>

          <q-separator class="drawer-sep q-my-md" />

          <!-- LOGOUT -->
          <q-item clickable @click="handleLogout" class="nav-item logout-nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-red">
                <q-icon name="fa-solid fa-right-from-bracket" size="13px" color="negative" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title text-negative">Đăng xuất</q-item-section>
          </q-item>
        </q-list>
      </q-scroll-area>
    </q-drawer>

    <!-- Page Content Container -->
    <q-page-container class="admin-container">
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '@/stores/auth.store'

const $q = useQuasar()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const leftDrawerOpen = ref(true)

const userInitials = computed(() => {
  const name = authStore.userDisplayName || 'Admin'
  return name.charAt(0).toUpperCase()
})

const currentRouteTitle = computed(() => {
  const path = route.path
  if (path === '/admin') return 'Bảng điều khiển Quản trị'
  if (path.includes('/analytics')) return 'Thống kê & Báo cáo'
  if (path.includes('/posts')) return 'Quản lý Bài viết'
  if (path.includes('/categories')) return 'Chủ đề & Danh mục'
  if (path.includes('/tags')) return 'Quản lý Thẻ (Tags)'
  if (path.includes('/comments')) return 'Quản lý Bình luận'
  if (path.includes('/profile')) return 'Hồ sơ tác giả'
  if (path.includes('/skills')) return 'Kỹ năng & Học vấn'
  if (path.includes('/security')) return 'Bảo mật & Tài khoản'
  if (path.includes('/settings')) return 'Cài đặt hệ thống'
  return 'Quản trị hệ thống'
})

async function handleLogout() {
  authStore.logout()
  $q.notify({
    type: 'info',
    message: 'Đã đăng xuất thành công.',
    position: 'top',
    timeout: 2000
  })
  await router.push('/login')
}
</script>

<style scoped lang="scss">
.admin-layout {
  background-color: #f8fafc;
  min-height: 100vh;
  color: #0b1326;
}

/* Header */
.admin-header {
  background: #ffffff;
  border-bottom: 1px solid #e2e8f0;
  color: #0b1326;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.03);
}

.admin-toolbar {
  min-height: 64px;
  padding: 0 20px;
}

.drawer-toggle-btn {
  color: #475569;
  margin-right: 12px;

  &:hover {
    color: #df266a;
    background: #fdf2f6;
  }
}

.header-left-section {
  display: flex;
  align-items: center;
}

.page-breadcrumb {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  padding: 5px 14px;
  border-radius: 9999px;
  font-size: 13px;
  font-weight: 600;
  color: #334155;

  .breadcrumb-icon {
    color: #df266a;
  }
}

/* Brand matching PortfolioHeader */
.brand {
  font-family: var(--font-headline, sans-serif);
  color: #df266a;
  font-size: 1.25rem;
  font-weight: 800;
  letter-spacing: -0.04em;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 5px;
  transition: opacity 0.2s ease;

  &:hover {
    opacity: 0.9;
  }

  .brand-suffix {
    color: #0b1326;
    font-weight: 600;
    font-size: 1.2rem;
    letter-spacing: -0.03em;
  }

  .brand-badge {
    font-family: var(--font-mono, monospace);
    font-size: 9px;
    font-weight: 700;
    color: #df266a;
    background: #fdf2f6;
    border: 1px solid rgba(223, 38, 106, 0.25);
    padding: 2px 6px;
    border-radius: 4px;
    letter-spacing: 0.08em;
    margin-left: 2px;
  }
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 14px;
}

.view-site-link {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-family: var(--font-headline, sans-serif);
  font-size: 13px;
  font-weight: 600;
  color: #475569;
  text-decoration: none;
  padding: 6px 14px;
  border-radius: 9999px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;

  &:hover {
    color: #df266a;
    border-color: #df266a;
    background: #fdf2f6;
  }
}

.header-sep {
  height: 24px;
  background: #e2e8f0;
}

.user-btn {
  padding: 4px 8px;
  border-radius: 10px;
  transition: background 0.2s ease;

  &:hover {
    background: #f1f5f9;
  }

  .user-avatar {
    background: linear-gradient(135deg, #df266a, #6366f1);
    color: #ffffff;
    font-weight: 700;
    font-size: 14px;
  }

  .user-meta {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    margin-left: 10px;
    margin-right: 6px;
    line-height: 1.15;

    .user-name {
      font-size: 13px;
      font-weight: 700;
      color: #0b1326;
    }

    .user-role {
      font-size: 10.5px;
      color: #df266a;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }
  }
}

/* User Dropdown */
.user-dropdown-menu {
  background: #ffffff !important;
  border: 1px solid #e2e8f0 !important;
  border-radius: 12px !important;
  min-width: 230px;
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.08) !important;

  .dropdown-header {
    padding: 14px 16px;

    .user-full-name {
      font-weight: 700;
      font-size: 14px;
      color: #0b1326;
    }

    .user-full-email {
      font-size: 12px;
      color: #64748b;
      margin-top: 2px;
    }
  }

  .dropdown-list {
    padding: 6px;

    :deep(.q-item) {
      border-radius: 8px;
      margin-bottom: 2px;
      font-size: 13px;
      font-weight: 500;
    }
  }
}

/* Drawer / Sidebar */
.admin-drawer {
  background-color: #ffffff !important;
  border-right: 1px solid #e2e8f0 !important;
  display: flex;
  flex-direction: column;
}

.drawer-header {
  height: 64px;
  padding: 0 20px;
  display: flex;
  align-items: center;
}

.drawer-sep {
  background: #f1f5f9;
}

.drawer-scroll-area {
  height: calc(100vh - 65px);
}

.drawer-menu {
  padding: 12px 14px 28px;

  .menu-header {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    letter-spacing: 0.08em;
    color: #94a3b8;
    padding: 10px 10px 4px 10px;
  }

  .nav-item {
    border-radius: 10px;
    color: #475569;
    margin-bottom: 4px;
    min-height: 42px;
    padding: 8px 12px;
    transition: all 0.2s ease;

    .nav-avatar {
      min-width: 36px;
      padding-right: 10px;
    }

    .sidebar-icon-box {
      width: 28px;
      height: 28px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);

      &.icon-pink {
        background: #fdf2f6;
        color: #df266a;
        border: 1px solid #fce7f3;
      }
      &.icon-cyan {
        background: #f0f9ff;
        color: #0284c7;
        border: 1px solid #e0f2fe;
      }
      &.icon-rose {
        background: #fff1f2;
        color: #e11d48;
        border: 1px solid #ffe4e6;
      }
      &.icon-indigo {
        background: #eef2ff;
        color: #4f46e5;
        border: 1px solid #e0e7ff;
      }
      &.icon-amber {
        background: #fffbeb;
        color: #d97706;
        border: 1px solid #fef3c7;
      }
      &.icon-teal {
        background: #f0fdfa;
        color: #0d9488;
        border: 1px solid #ccfbf1;
      }
      &.icon-purple {
        background: #faf5ff;
        color: #9333ea;
        border: 1px solid #f3e8ff;
      }
      &.icon-violet {
        background: #f5f3ff;
        color: #6366f1;
        border: 1px solid #ede9fe;
      }
      &.icon-emerald {
        background: #ecfdf5;
        color: #059669;
        border: 1px solid #d1fae5;
      }
      &.icon-slate {
        background: #f1f5f9;
        color: #475569;
        border: 1px solid #e2e8f0;
      }
      &.icon-red {
        background: #fef2f2;
        color: #ef4444;
        border: 1px solid #fee2e2;
      }
    }

    .nav-title {
      font-size: 13.5px;
      font-weight: 500;
      color: #334155;
    }

    &:hover {
      background: #f8fafc;

      .nav-title {
        color: #0b1326;
        font-weight: 600;
      }

      .sidebar-icon-box {
        transform: scale(1.08);
      }
    }

    &.nav-item-active {
      background: #fdf2f6;
      border-left: 3px solid #df266a;

      .nav-title {
        color: #df266a;
        font-weight: 700;
      }

      .sidebar-icon-box {
        box-shadow: 0 2px 6px rgba(223, 38, 106, 0.2);
      }
    }
  }

  .logout-nav-item {
    &:hover {
      background: #fef2f2;

      .nav-title {
        color: #ef4444;
      }
    }
  }
}

.admin-container {
  padding: 32px 40px;

  @media (max-width: 768px) {
    padding: 20px 16px;
  }
}
</style>

<!-- Global / Unscoped style for Teleported Dropdown Menu -->
<style lang="scss">
.user-dropdown-menu {
  background: #ffffff !important;
  border: 1px solid #e2e8f0 !important;
  border-radius: 14px !important;
  min-width: 240px !important;
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.1) !important;
  color: #0b1326 !important;

  .dropdown-header {
    padding: 14px 18px 10px;

    .user-full-name {
      font-weight: 800;
      font-size: 14.5px;
      color: #0b1326 !important;
    }

    .user-full-email {
      font-size: 12px;
      color: #64748b !important;
      margin-top: 2px;
    }
  }

  .dropdown-list {
    padding: 6px;

    .dropdown-item {
      border-radius: 8px;
      margin-bottom: 2px;
      min-height: 40px;
      color: #0b1326 !important;
      font-weight: 600;
      font-size: 13.5px;
      transition: all 0.2s ease;

      .item-avatar {
        min-width: 28px;
        padding-right: 8px;
        color: #df266a !important;
      }

      .item-label,
      .q-item__section--main {
        color: #0b1326 !important;
        font-size: 13.5px;
        font-weight: 600;
      }

      &:hover {
        background: #fdf2f6 !important;
        color: #df266a !important;

        .item-label,
        .q-item__section--main {
          color: #df266a !important;
        }

        .item-avatar {
          color: #df266a !important;
        }
      }

      &.logout-item {
        color: #ef4444 !important;

        .item-avatar,
        .item-label,
        .q-item__section--main {
          color: #ef4444 !important;
        }

        &:hover {
          background: #fef2f2 !important;
        }
      }
    }
  }
}
</style>

