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
          <!-- Theme Switcher Button -->
          <q-btn
            flat
            round
            dense
            class="theme-toggle-btn"
            :icon="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"
            :title="isDark ? 'Chuyển sang giao diện sáng' : 'Chuyển sang giao diện tối'"
            :aria-label="isDark ? 'Chuyển sang giao diện sáng' : 'Chuyển sang giao diện tối'"
            @click="toggleTheme"
          />

          <!-- Notification Bell Dropdown Button -->
          <div class="notification-bell-wrap">
            <q-btn
              flat
              round
              dense
              class="notification-bell-btn"
              :class="{ 'has-unread': unreadContactsCount > 0, 'bell-ringing': isBellRinging }"
              aria-label="Thông báo hệ thống"
            >
              <q-icon name="fa-solid fa-bell" size="15px" class="bell-icon" />
              <span v-if="unreadContactsCount > 0" class="bell-badge">
                {{ unreadContactsCount > 99 ? '99+' : unreadContactsCount }}
              </span>

              <!-- Notification Dropdown Menu -->
              <q-menu
                anchor="bottom right"
                self="top right"
                class="notification-dropdown-menu"
                :offset="[0, 10]"
                @show="handleNotificationMenuOpen"
              >
                <div class="notif-menu-header">
                  <div class="notif-header-left">
                    <div class="notif-header-title">
                      <q-icon name="fa-solid fa-bell" size="13px" class="text-rose q-mr-xs" />
                      Thông báo
                    </div>
                    <span v-if="unreadContactsCount > 0" class="notif-count-pill">
                      {{ unreadContactsCount }} mới
                    </span>
                  </div>
                  <button
                    type="button"
                    class="btn-notif-refresh"
                    title="Làm mới thông báo"
                    @click.stop="fetchRecentNotifications"
                  >
                    <q-icon name="fa-solid fa-rotate-right" size="11px" :class="{ 'fa-spin': loadingNotifications }" />
                  </button>
                </div>

                <q-separator class="notif-menu-sep" />

                <!-- Menu Body -->
                <div class="notif-menu-body">
                  <!-- Loading -->
                  <div v-if="loadingNotifications" class="notif-loading">
                    <q-spinner-dots size="24px" color="pink-7" />
                    <span>Đang tải thông báo...</span>
                  </div>

                  <!-- Empty State -->
                  <div v-else-if="recentNotifications.length === 0" class="notif-empty">
                    <div class="notif-empty-icon">
                      <q-icon name="fa-regular fa-bell-slash" size="26px" />
                    </div>
                    <div class="notif-empty-title">Không có thông báo mới</div>
                    <div class="notif-empty-sub">Hộp thư liên hệ của bạn đang trống</div>
                  </div>

                  <!-- List of recent items -->
                  <div v-else class="notif-items-list">
                    <div
                      v-for="item in recentNotifications"
                      :key="item.id"
                      class="notif-item-card"
                      :class="{ 'item-is-unread': item.status === 'Unread' }"
                      @click="openNotificationItem(item)"
                      v-close-popup
                    >
                      <div class="notif-avatar" :style="{ background: getAvatarColor(item.name) }">
                        {{ getInitials(item.name) }}
                      </div>

                      <div class="notif-content-box">
                        <div class="notif-sender-row">
                          <span class="notif-sender-name">{{ item.name }}</span>
                          <span class="notif-time">{{ formatTimeAgo(item.createdAt) }}</span>
                        </div>
                        <div class="notif-subject-line">{{ item.subject || 'Tin nhắn liên hệ mới' }}</div>
                        <div class="notif-preview-snippet">{{ item.message }}</div>
                      </div>

                      <div v-if="item.status === 'Unread'" class="notif-unread-dot-col">
                        <span class="notif-unread-dot"></span>
                      </div>
                    </div>
                  </div>
                </div>

                <q-separator class="notif-menu-sep" />

                <!-- Menu Footer -->
                <div class="notif-menu-footer">
                  <router-link to="/admin/contacts" class="notif-footer-link" v-close-popup>
                    <span>Xem tất cả hộp thư</span>
                    <q-icon name="fa-solid fa-arrow-right" size="11px" />
                  </router-link>
                </div>
              </q-menu>
            </q-btn>
          </div>

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

          <q-item clickable to="/admin/contacts" active-class="nav-item-active" class="nav-item">
            <q-item-section avatar class="nav-avatar">
              <div class="sidebar-icon-box icon-rose">
                <q-icon name="fa-solid fa-inbox" size="13px" />
              </div>
            </q-item-section>
            <q-item-section class="nav-title">Hộp thư liên hệ</q-item-section>
            <q-item-section v-if="unreadContactsCount > 0" side>
              <q-badge color="pink-7" rounded :label="unreadContactsCount" class="q-ml-auto" />
            </q-item-section>
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
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '@/stores/auth.store'
import { adminContactService } from '@/services/admin-contact.service'
import { signalRService } from '@/services/signalr.service'

import type { ContactMessage } from '@/types/admin-contact'
import { formatTimeAgo } from '@/utils/date'

const $q = useQuasar()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const leftDrawerOpen = ref(true)
const unreadContactsCount = ref(0)
const recentNotifications = ref<ContactMessage[]>([])
const loadingNotifications = ref(false)
const isBellRinging = ref(false)

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
  if (path.includes('/contacts')) return 'Hộp thư liên hệ'
  if (path.includes('/profile')) return 'Hồ sơ tác giả'
  if (path.includes('/skills')) return 'Kỹ năng & Học vấn'
  if (path.includes('/security')) return 'Bảo mật & Tài khoản'
  if (path.includes('/settings')) return 'Cài đặt hệ thống'
  return 'Quản trị hệ thống'
})

async function fetchUnreadCount() {
  try {
    const summary = await adminContactService.getSummary()
    unreadContactsCount.value = summary.unreadMessages
  } catch (e) {
    // Ignore error if not logged in yet
  }
}

async function fetchRecentNotifications() {
  try {
    loadingNotifications.value = true
    const res = await adminContactService.getContacts({ pageIndex: 1, pageSize: 6 })
    recentNotifications.value = res.items || []
  } catch (e) {
    console.warn('Failed to fetch recent notifications:', e)
  } finally {
    loadingNotifications.value = false
  }
}

function handleNotificationMenuOpen() {
  fetchRecentNotifications()
}

function openNotificationItem(item: ContactMessage) {
  router.push('/admin/contacts')
}

function playBellChime() {
  try {
    const audioCtx = new (window.AudioContext || (window as any).webkitAudioContext)()
    const osc = audioCtx.createOscillator()
    const gain = audioCtx.createGain()
    osc.type = 'sine'
    osc.frequency.setValueAtTime(587.33, audioCtx.currentTime) // D5
    osc.frequency.setValueAtTime(880, audioCtx.currentTime + 0.1) // A5
    gain.gain.setValueAtTime(0.15, audioCtx.currentTime)
    gain.gain.exponentialRampToValueAtTime(0.01, audioCtx.currentTime + 0.3)
    osc.connect(gain)
    gain.connect(audioCtx.destination)
    osc.start()
    osc.stop(audioCtx.currentTime + 0.35)
  } catch (e) {
    // Audio Context might be restricted
  }
}

function handleGlobalNewContact(msg: ContactMessage) {
  unreadContactsCount.value++
  isBellRinging.value = true
  playBellChime()
  setTimeout(() => {
    isBellRinging.value = false
  }, 1500)

  // Prepend to recent list if already fetched
  if (recentNotifications.value.length > 0) {
    recentNotifications.value.unshift(msg)
    if (recentNotifications.value.length > 6) {
      recentNotifications.value.pop()
    }
  }
}

function handleGlobalContactStatusUpdate() {
  fetchUnreadCount()
  if (recentNotifications.value.length > 0) {
    fetchRecentNotifications()
  }
}

function getInitials(name: string): string {
  if (!name) return 'U'
  const parts = name.trim().split(/\s+/).filter(Boolean)
  if (parts.length === 0) return 'U'
  const first = parts[0] || 'U'
  if (parts.length === 1) return first.charAt(0).toUpperCase()
  const last = parts[parts.length - 1] || ''
  return (first.charAt(0) + last.charAt(0)).toUpperCase()
}

function getAvatarColor(name: string): string {
  const colors = ['#df266a', '#4f46e5', '#059669', '#d97706', '#0284c7', '#7c3aed']
  let hash = 0
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash)
  }
  const index = Math.abs(hash) % colors.length
  return colors[index] || '#df266a'
}


const isDark = ref(true)

function applyTheme(dark: boolean) {
  isDark.value = dark
  document.body.classList.toggle('portfolio-light', !dark)
  localStorage.setItem('portfolio-theme', dark ? 'dark' : 'light')
}

function toggleTheme() {
  applyTheme(!isDark.value)
}

onMounted(() => {
  const savedTheme = localStorage.getItem('portfolio-theme')
  const preferDark = savedTheme !== 'light'
  applyTheme(preferDark)

  fetchUnreadCount()
  signalRService.start()
  signalRService.onNewContactMessage(handleGlobalNewContact)
  signalRService.onContactStatusUpdate(handleGlobalContactStatusUpdate)
})

onUnmounted(() => {
  signalRService.offNewContactMessage(handleGlobalNewContact)
  signalRService.offContactStatusUpdate(handleGlobalContactStatusUpdate)
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
  gap: 12px;
}

/* Theme Switcher Button */
.theme-toggle-btn {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  color: #64748b;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;

  &:hover {
    color: #df266a;
    border-color: #df266a;
    background: #fdf2f6;
    transform: rotate(15deg);
  }
}

/* Notification Bell Button */
.notification-bell-wrap {
  display: flex;
  align-items: center;
}

.notification-bell-btn {
  position: relative;
  width: 36px;
  height: 36px;
  border-radius: 10px;
  color: #64748b;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;

  &:hover {
    color: #df266a;
    border-color: #df266a;
    background: #fdf2f6;
  }

  &.has-unread {
    color: #df266a;
    background: #fff1f2;
    border-color: #ffe4e6;
  }

  &.bell-ringing {
    animation: bell-shake 0.8s cubic-bezier(0.36, 0.07, 0.19, 0.97) both;
  }

  .bell-badge {
    position: absolute;
    top: -4px;
    right: -4px;
    min-width: 17px;
    height: 17px;
    padding: 0 4px;
    border-radius: 999px;
    background: linear-gradient(135deg, #df266a, #e11d48);
    color: #ffffff;
    font-family: var(--font-mono, monospace);
    font-size: 9.5px;
    font-weight: 800;
    display: flex;
    align-items: center;
    justify-content: center;
    border: 2px solid #ffffff;
    box-shadow: 0 2px 6px rgba(223, 38, 106, 0.4);
    line-height: 1;
  }
}

@keyframes bell-shake {
  0%, 100% { transform: rotate(0); }
  15% { transform: rotate(14deg); }
  30% { transform: rotate(-14deg); }
  45% { transform: rotate(10deg); }
  60% { transform: rotate(-10deg); }
  75% { transform: rotate(4deg); }
  85% { transform: rotate(-4deg); }
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

/* Notification Dropdown Menu */
.notification-dropdown-menu {
  background: #ffffff !important;
  border: 1px solid #e2e8f0 !important;
  border-radius: 14px !important;
  width: 360px;
  max-width: 90vw;
  box-shadow: 0 14px 32px rgba(0, 0, 0, 0.09) !important;
  overflow: hidden;

  .notif-menu-header {
    padding: 12px 16px;
    display: flex;
    align-items: center;
    justify-content: space-between;

    .notif-header-left {
      display: flex;
      align-items: center;
      gap: 8px;

      .notif-header-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 14px;
        font-weight: 800;
        color: #0b1326;
        display: flex;
        align-items: center;
      }

      .notif-count-pill {
        padding: 2px 8px;
        border-radius: 999px;
        background: #fff1f2;
        border: 1px solid #ffe4e6;
        color: #df266a;
        font-family: var(--font-mono, monospace);
        font-size: 10.5px;
        font-weight: 700;
      }
    }

    .btn-notif-refresh {
      width: 26px;
      height: 26px;
      border-radius: 6px;
      background: transparent;
      border: 1px solid #e2e8f0;
      color: #64748b;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.15s ease;

      &:hover {
        color: #df266a;
        border-color: #df266a;
      }
    }
  }

  .notif-menu-sep {
    background: #f1f5f9;
  }

  .notif-menu-body {
    max-height: 340px;
    overflow-y: auto;

    .notif-loading {
      padding: 28px 16px;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      color: #64748b;
      font-size: 12.5px;
    }

    .notif-empty {
      padding: 32px 16px;
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;

      .notif-empty-icon {
        width: 44px;
        height: 44px;
        border-radius: 50%;
        background: #f8fafc;
        color: #94a3b8;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-bottom: 8px;
      }

      .notif-empty-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 13.5px;
        font-weight: 700;
        color: #334155;
      }

      .notif-empty-sub {
        font-size: 11.5px;
        color: #94a3b8;
        margin-top: 2px;
      }
    }

    .notif-items-list {
      display: flex;
      flex-direction: column;
    }

    .notif-item-card {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 12px 16px;
      cursor: pointer;
      transition: background 0.15s ease;
      border-bottom: 1px solid #f8fafc;

      &:last-child {
        border-bottom: none;
      }

      &:hover {
        background: #f8fafc;
      }

      &.item-is-unread {
        background: #fff8f9;

        &:hover {
          background: #fff1f4;
        }

        .notif-subject-line {
          font-weight: 700;
          color: #0b1326;
        }
      }

      .notif-avatar {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        color: #ffffff;
        font-family: var(--font-headline, sans-serif);
        font-size: 12px;
        font-weight: 800;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        text-transform: uppercase;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      }

      .notif-content-box {
        flex: 1;
        min-width: 0;

        .notif-sender-row {
          display: flex;
          align-items: center;
          justify-content: space-between;
          gap: 6px;
          margin-bottom: 2px;

          .notif-sender-name {
            font-family: var(--font-headline, sans-serif);
            font-size: 12.5px;
            font-weight: 700;
            color: #0b1326;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
          }

          .notif-time {
            font-family: var(--font-mono, monospace);
            font-size: 10.5px;
            color: #94a3b8;
            flex-shrink: 0;
          }
        }

        .notif-subject-line {
          font-family: var(--font-headline, sans-serif);
          font-size: 12.5px;
          font-weight: 600;
          color: #334155;
          margin-bottom: 2px;
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
        }

        .notif-preview-snippet {
          font-family: var(--font-body, sans-serif);
          font-size: 11.5px;
          color: #64748b;
          line-height: 1.35;
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
        }
      }

      .notif-unread-dot-col {
        display: flex;
        align-items: center;
        padding-top: 4px;

        .notif-unread-dot {
          width: 6px;
          height: 6px;
          border-radius: 50%;
          background: #df266a;
          box-shadow: 0 0 6px rgba(223, 38, 106, 0.8);
        }
      }
    }
  }

  .notif-menu-footer {
    padding: 8px 12px;
    background: #fafbfc;
    text-align: center;

    .notif-footer-link {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
      font-family: var(--font-headline, sans-serif);
      font-size: 12px;
      font-weight: 700;
      color: #df266a;
      text-decoration: none;
      padding: 6px 12px;
      border-radius: 8px;
      transition: all 0.15s ease;

      &:hover {
        background: #fdf2f6;
      }
    }
  }
}

/* Drawer / Sidebar */
.admin-drawer {
  background-color: #ffffff;
  border-right: 1px solid #e2e8f0;
  display: flex;
  flex-direction: column;
}

:global(body:not(.portfolio-light)) .admin-drawer {
  background-color: #0b1326 !important;
  border-right: 1px solid rgba(248, 250, 252, 0.08) !important;
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
  border-radius: 14px !important;
  min-width: 240px !important;
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.1) !important;

  .dropdown-header {
    padding: 14px 18px 10px;

    .user-full-name {
      font-weight: 800;
      font-size: 14.5px;
    }

    .user-full-email {
      font-size: 12px;
      margin-top: 2px;
    }
  }

  .dropdown-list {
    padding: 6px;

    .dropdown-item {
      border-radius: 8px;
      margin-bottom: 2px;
      min-height: 40px;
      font-weight: 600;
      font-size: 13.5px;
      transition: all 0.2s ease;

      .item-avatar {
        min-width: 28px;
        padding-right: 8px;
        color: #df266a;
      }

      .item-label,
      .q-item__section--main {
        font-size: 13.5px;
        font-weight: 600;
      }

      &.logout-item {
        color: #ef4444 !important;

        .item-avatar,
        .item-label,
        .q-item__section--main {
          color: #ef4444 !important;
        }

        &:hover {
          background: rgba(239, 68, 68, 0.12) !important;
        }
      }
    }
  }
}

body.portfolio-light .user-dropdown-menu {
  background: #ffffff !important;
  border: 1px solid #e2e8f0 !important;
  color: #0b1326 !important;

  .dropdown-header {
    .user-full-name {
      color: #0b1326 !important;
    }
    .user-full-email {
      color: #64748b !important;
    }
  }

  .dropdown-list {
    .dropdown-item {
      color: #0b1326 !important;

      .item-label,
      .q-item__section--main {
        color: #0b1326 !important;
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
    }
  }
}
</style>

