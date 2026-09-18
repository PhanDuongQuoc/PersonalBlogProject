<template>
  <q-page class="login-page">
    <div class="login-wrapper">
      <!-- Glow effect behind the card -->
      <div class="card-glow-backdrop" aria-hidden="true"></div>

      <!-- Main Login Card (Light White Theme) -->
      <div class="login-card">
        <!-- Top Status Badge -->
        <!-- <div class="card-top-badge">
          <span class="badge-dot"></span>
          <span class="badge-text">ADMINISTRATION PORTAL</span>
        </div> -->

        <!-- Header -->
        <div class="card-header text-center">
          <h1 class="login-title">Đăng nhập Quản trị</h1>
          <p class="login-subtitle">
            Nhập tài khoản để truy cập bảng điều khiển và quản lý Blog.
          </p>
        </div>

        <!-- Error Banner -->
        <transition name="fade">
          <div v-if="authStore.errorMessage" class="error-banner">
            <q-icon name="fa-solid fa-circle-exclamation" size="16px" class="error-icon" />
            <span class="error-text">{{ authStore.errorMessage }}</span>
          </div>
        </transition>

        <!-- Login Form -->
        <q-form @submit.prevent="handleLogin" class="login-form">
          <!-- Username / Email Input -->
          <div class="form-group">
            <label class="form-label" for="login-identifier">Tên đăng nhập hoặc Email</label>
            <q-input id="login-identifier" v-model="form.usernameOrEmail" outlined dense class="custom-input"
              placeholder="admin hoặc admin@example.com"
              :rules="[(val) => !!val?.trim() || 'Vui lòng nhập tên đăng nhập hoặc email']" lazy-rules no-error-icon
              autocomplete="username">
              <template #prepend>
                <q-icon name="fa-solid fa-user" size="16px" class="input-icon" />
              </template>
            </q-input>
          </div>

          <!-- Password Input -->
          <div class="form-group">
            <div class="form-label-row">
              <label class="form-label" for="login-password">Mật khẩu</label>
            </div>
            <q-input id="login-password" v-model="form.password" :type="isPasswordVisible ? 'text' : 'password'"
              outlined dense class="custom-input" placeholder="••••••••"
              :rules="[(val) => !!val || 'Vui lòng nhập mật khẩu']" lazy-rules no-error-icon
              autocomplete="current-password">
              <template #prepend>
                <q-icon name="fa-solid fa-lock" size="16px" class="input-icon" />
              </template>
              <template #append>
                <q-btn flat round dense size="sm" :icon="isPasswordVisible ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"
                  class="toggle-pwd-btn" @click="isPasswordVisible = !isPasswordVisible"
                  :aria-label="isPasswordVisible ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'" />
              </template>
            </q-input>
          </div>

          <!-- Remember Me & Forgot Password -->
          <div class="form-options">
            <q-checkbox v-model="form.rememberMe" dense label="Ghi nhớ đăng nhập" class="remember-checkbox" />
            <router-link to="/forgot-password" class="forgot-link">
              Quên mật khẩu?
            </router-link>
          </div>

          <!-- Submit Button -->
          <div class="form-action">
            <button type="submit" class="submit-btn" :disabled="authStore.isLoading">
              <q-spinner-tail v-if="authStore.isLoading" size="20px" />
              <template v-else>
                <span>Đăng nhập vào Dashboard</span>
                <q-icon name="fa-solid fa-arrow-right" size="14px" class="btn-arrow" />
              </template>
            </button>
          </div>
        </q-form>

        <!-- Security Footer Notice -->
        <div class="card-footer-notice">
          <q-icon name="fa-solid fa-shield-halved" size="13px" />
          <span>Khu vực bảo mật dành riêng cho Quản trị viên & Tác giả</span>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '@/stores/auth.store'

const $q = useQuasar()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const isPasswordVisible = ref(false)

const form = reactive({
  usernameOrEmail: '',
  password: '',
  rememberMe: true
})

async function handleLogin() {
  if (!form.usernameOrEmail.trim() || !form.password) {
    $q.notify({
      type: 'warning',
      message: 'Vui lòng điền đầy đủ tài khoản và mật khẩu.',
      position: 'top'
    })
    return
  }

  const result = await authStore.login({
    usernameOrEmail: form.usernameOrEmail.trim(),
    password: form.password,
    rememberMe: form.rememberMe
  })

  if (result.success) {
    $q.notify({
      type: 'positive',
      message: `Đăng nhập thành công! Chào mừng ${authStore.userDisplayName}.`,
      position: 'top',
      timeout: 2500
    })

    const redirectPath = (route.query.redirect as string) || '/admin'
    await router.push(redirectPath)
  } else {
    $q.notify({
      type: 'negative',
      message: result.message || 'Đăng nhập thất bại. Vui lòng thử lại!',
      position: 'top',
      timeout: 3500
    })
  }
}
</script>

<style scoped lang="scss">
.login-page {
  width: 100%;
  height: 100%;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  overflow: hidden;
  box-sizing: border-box;
}

.login-wrapper {
  position: relative;
  width: 100%;
  max-width: 410px;
  margin: auto;
}

/* Subtle Glow Backdrop */
.card-glow-backdrop {
  position: absolute;
  inset: -8px;
  background: radial-gradient(circle, rgba(223, 38, 106, 0.15) 0%, rgba(99, 102, 241, 0.08) 50%, transparent 80%);
  border-radius: 24px;
  filter: blur(16px);
  pointer-events: none;
  z-index: 1;
}

/* Main Login Card */
.login-card {
  position: relative;
  z-index: 2;
  background: var(--bg-surface, #0b1326);
  border: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  border-radius: 18px;
  padding: 28px 28px 24px 28px;
  box-shadow: var(--shadow-card, 0 20px 45px rgba(0, 0, 0, 0.5));
}

/* Top Status Badge */
.card-top-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 9999px;
  background: var(--accent-primary-container, rgba(223, 38, 106, 0.12));
  border: 1px solid rgba(223, 38, 106, 0.25);
  margin-bottom: 14px;

  .badge-dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background: #df266a;
    box-shadow: 0 0 6px rgba(223, 38, 106, 0.6);
  }

  .badge-text {
    font-family: var(--font-mono, monospace);
    font-size: 10px;
    font-weight: 700;
    letter-spacing: 0.08em;
    color: #df266a;
  }
}

/* Card Header */
.card-header {
  margin-bottom: 18px;

  .login-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 23px;
    font-weight: 800;
    letter-spacing: -0.025em;
    color: var(--text-primary, #dae2fd);
    margin: 0 0 4px 0;
    line-height: 1.2;
  }

  .login-subtitle {
    font-family: var(--font-body, sans-serif);
    font-size: 13px;
    color: var(--text-secondary, #94a3b8);
    margin: 0;
    line-height: 1.45;
  }
}

/* Error Banner */
.error-banner {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-radius: 8px;
  background: rgba(239, 68, 68, 0.15);
  border: 1px solid rgba(239, 68, 68, 0.3);
  color: #fca5a5;
  font-size: 12.5px;
  font-weight: 500;
  margin-bottom: 14px;

  .error-icon {
    color: #ef4444;
    flex-shrink: 0;
  }
}

/* Form Styles */
.login-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;

  .form-label-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    color: var(--text-primary, #dae2fd);
    letter-spacing: -0.01em;
  }
}

/* Custom Quasar Input Styling */
:deep(.custom-input) {
  .q-field__control {
    background: var(--bg-surface-container, #131b2e) !important;
    border-radius: 9px !important;
    border: 1px solid var(--border-subtle, rgba(248, 250, 252, 0.12)) !important;
    height: 44px !important;
    min-height: 44px !important;
    transition: all 0.2s ease;
  }

  .q-field__control:before,
  .q-field__control:after {
    display: none !important;
  }

  &.q-field--focused .q-field__control {
    border-color: #df266a !important;
    box-shadow: 0 0 0 3px var(--accent-primary-container, rgba(223, 38, 106, 0.18)) !important;
    background: var(--bg-surface-high, #171f33) !important;
  }

  .q-field__native,
  input {
    color: var(--text-primary, #dae2fd) !important;
    font-size: 13.5px !important;
    font-family: var(--font-body, sans-serif);
    font-weight: 500;

    &::placeholder {
      color: var(--text-muted, #64748b) !important;
    }

    &:-webkit-autofill,
    &:-webkit-autofill:hover, 
    &:-webkit-autofill:focus, 
    &:-webkit-autofill:active {
      -webkit-box-shadow: 0 0 0 1000px var(--bg-surface-container, #131b2e) inset !important;
      -webkit-text-fill-color: var(--text-primary, #dae2fd) !important;
      caret-color: var(--text-primary, #dae2fd) !important;
      transition: background-color 5000s ease-in-out 0s !important;
    }
  }

  &.q-field--focused input:-webkit-autofill {
    -webkit-box-shadow: 0 0 0 1000px var(--bg-surface-high, #171f33) inset !important;
    -webkit-text-fill-color: var(--text-primary, #dae2fd) !important;
  }

  .input-icon {
    color: var(--text-muted, #64748b);
    font-size: 20px;
    transition: color 0.2s ease;
  }

  &.q-field--focused .input-icon {
    color: #df266a;
  }

  .q-field__bottom {
    padding: 3px 2px 0 2px !important;
    font-size: 11px !important;
    color: #ef4444 !important;
    font-weight: 500;
  }
}

.toggle-pwd-btn {
  color: var(--text-muted, #64748b);

  &:hover {
    color: #ffffff;
  }
}

/* Form Options */
.form-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: -2px;

  :deep(.remember-checkbox) {
    .q-checkbox__label {
      font-size: 12.5px;
      color: var(--text-secondary, #94a3b8);
      font-weight: 500;
    }

    .q-checkbox__inner--truthy {
      color: #df266a !important;
    }

    .q-checkbox__bg {
      border-color: var(--border-subtle, rgba(248, 250, 252, 0.2));
    }
  }

  .forgot-link {
    font-size: 12px;
    font-weight: 600;
    color: #df266a;
    text-decoration: none;
    transition: opacity 0.2s ease;

    &:hover {
      opacity: 0.8;
      text-decoration: underline;
    }
  }
}

/* Submit Button */
.form-action {
  margin-top: 4px;

  .submit-btn {
    width: 100%;
    height: 44px;
    border-radius: 9px;
    background: linear-gradient(135deg, #df266a, #c01b55);
    color: #ffffff;
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    font-weight: 700;
    letter-spacing: -0.01em;
    border: none;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.35);

    .btn-arrow {
      transition: transform 0.2s ease;
    }

    &:hover:not(:disabled) {
      background: linear-gradient(135deg, #f43f7e, #df266a);
      transform: translateY(-2px);
      box-shadow: 0 6px 18px rgba(223, 38, 106, 0.5);

      .btn-arrow {
        transform: translateX(4px);
      }
    }

    &:active:not(:disabled) {
      transform: translateY(0);
    }

    &:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }
  }
}

/* Card Footer Notice */
.card-footer-notice {
  margin-top: 18px;
  padding-top: 14px;
  border-top: 1px solid var(--border-hairline, rgba(248, 250, 252, 0.08));
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  color: var(--text-muted, #64748b);
  font-size: 11.5px;
  font-family: var(--font-body, sans-serif);
  text-align: center;
}

/* Transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}

@media (max-width: 480px) {
  .login-card {
    padding: 24px 20px 20px 20px;
  }

  .card-header .login-title {
    font-size: 21px;
  }
}
</style>
