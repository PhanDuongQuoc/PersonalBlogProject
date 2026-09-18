<template>
  <q-layout view="hHh lpR fFf" class="auth-layout">
    <!-- Ambient Background Lighting Orbs -->
    <div class="auth-bg-ambient" aria-hidden="true">
      <div class="ambient-orb ambient-orb-1"></div>
      <div class="ambient-orb ambient-orb-2"></div>
      <div class="ambient-grid-overlay"></div>
    </div>

    <!-- Auth Absolute Top Header -->
    <header class="auth-header">
      <div class="auth-header-container">
        <router-link to="/home" class="brand" aria-label="PDQ Portfolio">
          PDQ<span class="brand-suffix">Portfolio</span>
        </router-link>

        <router-link to="/" class="auth-back-btn">
          <q-icon name="fa-solid fa-arrow-left" size="14px" />
          <span>Về trang chủ</span>
        </router-link>
      </div>
    </header>

    <!-- Main Page Container (Perfect Centering) -->
    <q-page-container class="auth-page-container">
      <router-view />
    </q-page-container>

    <!-- Minimal Auth Absolute Bottom Footer -->
    <footer class="auth-footer">
      <div class="auth-footer-content">
        <span>&copy; {{ currentYear }} PDQ Personal Blog. Bảo mật và quản trị hệ thống.</span>
      </div>
    </footer>
  </q-layout>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const currentYear = computed(() => new Date().getFullYear())
</script>

<style scoped lang="scss">
.auth-layout {
  height: 100vh;
  min-height: 100vh;
  max-height: 100vh;
  background-color: var(--bg-canvas, #060e20);
  color: var(--text-primary, #dae2fd);
  position: relative;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Ambient Lighting & Grid Background */
.auth-bg-ambient {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;

  .ambient-orb {
    position: absolute;
    border-radius: 50%;
    filter: blur(120px);
    opacity: 0.18;
    animation: orb-float 12s ease-in-out infinite alternate;
  }

  .ambient-orb-1 {
    width: 480px;
    height: 480px;
    top: -100px;
    left: 15%;
    background: radial-gradient(circle, #df266a 0%, rgba(223, 38, 106, 0) 70%);
  }

  .ambient-orb-2 {
    width: 520px;
    height: 520px;
    bottom: -100px;
    right: 15%;
    background: radial-gradient(circle, #6366f1 0%, rgba(99, 102, 241, 0) 70%);
    animation-delay: -6s;
  }

  .ambient-grid-overlay {
    position: absolute;
    inset: 0;
    background-image: radial-gradient(rgba(248, 250, 252, 0.06) 1px, transparent 1px);
    background-size: 32px 32px;
    opacity: 0.8;
  }
}

@keyframes orb-float {
  0% {
    transform: translate(0, 0) scale(1);
  }
  100% {
    transform: translate(30px, 40px) scale(1.08);
  }
}

/* Fixed Floating Header */
.auth-header {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  z-index: 20;
  padding: 16px 28px;
  width: 100%;

  .auth-header-container {
    max-width: 1200px;
    margin: 0 auto;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  /* Brand matching PortfolioHeader */
  .brand {
    font-family: var(--font-headline, sans-serif);
    color: #df266a;
    font-size: 1.35rem;
    font-weight: 800;
    letter-spacing: -0.04em;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    gap: 4px;
    transition: opacity 0.2s ease;

    &:hover {
      opacity: 0.9;
    }
  }

  .brand-suffix {
    color: var(--text-primary, #dae2fd);
    font-weight: 600;
    font-size: 1.25rem;
    letter-spacing: -0.03em;
  }

  .auth-back-btn {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 600;
    color: var(--text-primary, #dae2fd);
    text-decoration: none;
    padding: 7px 16px;
    border-radius: 9999px;
    background: var(--bg-surface-container, #131b2e);
    border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.1));
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    transition: all 0.2s ease;

    &:hover {
      color: #df266a;
      border-color: #df266a;
      background: var(--accent-primary-container, rgba(223, 38, 106, 0.15));
      transform: translateX(-2px);
    }
  }
}

/* Page Container — Full Screen Center without Scrollbar */
.auth-page-container {
  position: relative;
  z-index: 10;
  width: 100%;
  height: 100vh;
  min-height: 100vh;
  max-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  padding: 0 !important;
}

/* Fixed Floating Footer */
.auth-footer {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 20;
  padding: 12px 20px;
  text-align: center;
  pointer-events: none;

  .auth-footer-content {
    font-family: var(--font-body, sans-serif);
    font-size: 11.5px;
    color: var(--text-muted, #64748b);
    max-width: 1200px;
    margin: 0 auto;
  }
}

@media (max-width: 600px) {
  .auth-header {
    padding: 12px 16px;

    .auth-back-btn span {
      display: none;
    }

    .auth-back-btn {
      padding: 6px;
    }
  }

  .auth-footer {
    padding: 8px 12px;
  }
}
</style>
