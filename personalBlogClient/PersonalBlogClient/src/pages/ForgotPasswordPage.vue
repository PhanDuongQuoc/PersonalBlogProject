<template>
  <q-page class="forgot-page">
    <div class="forgot-wrapper">
      <!-- Glow effect behind the card -->
      <div class="card-glow-backdrop" aria-hidden="true"></div>

      <!-- Main Card -->
      <div class="forgot-card">
        <!-- Status Badge -->
        <div class="card-top-badge">
          <span class="badge-dot"></span>
          <span class="badge-text">ACCOUNT RECOVERY</span>
        </div>

        <!-- ========================================== -->
        <!-- STEP 1: ENTER EMAIL                        -->
        <!-- ========================================== -->
        <template v-if="currentStep === 1">
          <div class="card-header text-center">
            <h1 class="forgot-title">Quên mật khẩu?</h1>
            <p class="forgot-subtitle">
              Nhập email tài khoản của bạn để nhận mã xác nhận OTP đặt lại mật khẩu.
            </p>
          </div>

          <!-- Error Banner -->
          <transition name="fade">
            <div v-if="authStore.errorMessage" class="error-banner">
              <q-icon name="fa-solid fa-circle-exclamation" size="16px" class="error-icon" />
              <span class="error-text">{{ authStore.errorMessage }}</span>
            </div>
          </transition>

          <q-form @submit.prevent="handleSendOtp" class="forgot-form">
            <div class="form-group">
              <label class="form-label" for="recovery-email">Địa chỉ Email</label>
              <q-input
                id="recovery-email"
                v-model="email"
                type="email"
                outlined
                dense
                class="custom-input"
                placeholder="phanduongquoc@example.com"
                :rules="[
                  (val) => !!val?.trim() || 'Vui lòng nhập email',
                  (val) => /.+@.+\..+/.test(val) || 'Email không hợp lệ'
                ]"
                lazy-rules
                no-error-icon
                autocomplete="email"
              >
                <template #prepend>
                  <q-icon name="fa-solid fa-envelope" size="15px" class="input-icon" />
                </template>
              </q-input>
            </div>

            <div class="form-action">
              <button
                type="submit"
                class="submit-btn"
                :disabled="authStore.isLoading"
              >
                <q-spinner-tail v-if="authStore.isLoading" size="20px" />
                <template v-else>
                  <span>Gửi mã xác nhận OTP</span>
                  <q-icon name="fa-solid fa-arrow-right" size="14px" class="btn-arrow" />
                </template>
              </button>
            </div>
          </q-form>

          <div class="card-back-link text-center">
            <router-link to="/login" class="back-link">
              <q-icon name="fa-solid fa-arrow-left" size="13px" />
              <span>Quay lại trang Đăng nhập</span>
            </router-link>
          </div>
        </template>

        <!-- ========================================== -->
        <!-- STEP 2: ENTER OTP & NEW PASSWORD           -->
        <!-- ========================================== -->
        <template v-else-if="currentStep === 2">
          <div class="card-header text-center">
            <h1 class="forgot-title">Đặt lại mật khẩu mới</h1>
            <p class="forgot-subtitle">
              Mã OTP đã được gửi đến <strong>{{ email }}</strong>.
            </p>
          </div>

          <!-- Dev OTP Hint Box -->
          <div v-if="receivedOtp" class="dev-otp-box">
            <div class="otp-hint-title">
              <q-icon name="fa-solid fa-circle-info" size="15px" />
              <span>Mã OTP xác thực của bạn:</span>
            </div>
            <div class="otp-code-highlight">{{ receivedOtp }}</div>
          </div>

          <!-- Error Banner -->
          <transition name="fade">
            <div v-if="authStore.errorMessage" class="error-banner">
              <q-icon name="fa-solid fa-circle-exclamation" size="16px" class="error-icon" />
              <span class="error-text">{{ authStore.errorMessage }}</span>
            </div>
          </transition>

          <q-form @submit.prevent="handleResetPassword" class="forgot-form">
            <!-- OTP Input -->
            <div class="form-group">
              <label class="form-label" for="recovery-otp">Mã xác thực OTP (6 số)</label>
              <q-input
                id="recovery-otp"
                v-model="resetForm.otp"
                outlined
                dense
                maxlength="6"
                class="custom-input otp-input"
                placeholder="123456"
                :rules="[
                  (val) => !!val?.trim() || 'Vui lòng nhập mã OTP',
                  (val) => val.trim().length === 6 || 'Mã OTP gồm đúng 6 chữ số'
                ]"
                lazy-rules
                no-error-icon
              >
                <template #prepend>
                  <q-icon name="fa-solid fa-hashtag" size="15px" class="input-icon" />
                </template>
              </q-input>
            </div>

            <!-- New Password Input -->
            <div class="form-group">
              <label class="form-label" for="recovery-new-password">Mật khẩu mới</label>
              <q-input
                id="recovery-new-password"
                v-model="resetForm.newPassword"
                :type="isPwdVisible ? 'text' : 'password'"
                outlined
                dense
                class="custom-input"
                placeholder="Tối thiểu 6 ký tự"
                :rules="[
                  (val) => !!val || 'Vui lòng nhập mật khẩu mới',
                  (val) => val.length >= 6 || 'Mật khẩu phải từ 6 ký tự trở lên'
                ]"
                lazy-rules
                no-error-icon
                autocomplete="new-password"
              >
                <template #prepend>
                  <q-icon name="fa-solid fa-lock" size="15px" class="input-icon" />
                </template>
                <template #append>
                  <q-btn
                    flat
                    round
                    dense
                    size="sm"
                    :icon="isPwdVisible ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"
                    class="toggle-pwd-btn"
                    @click="isPwdVisible = !isPwdVisible"
                  />
                </template>
              </q-input>
            </div>

            <!-- Confirm Password Input -->
            <div class="form-group">
              <label class="form-label" for="recovery-confirm-password">Xác nhận mật khẩu mới</label>
              <q-input
                id="recovery-confirm-password"
                v-model="resetForm.confirmPassword"
                :type="isConfirmPwdVisible ? 'text' : 'password'"
                outlined
                dense
                class="custom-input"
                placeholder="Nhập lại mật khẩu mới"
                :rules="[
                  (val) => !!val || 'Vui lòng xác nhận mật khẩu mới',
                  (val) => val === resetForm.newPassword || 'Mật khẩu xác nhận không khớp'
                ]"
                lazy-rules
                no-error-icon
                autocomplete="new-password"
              >
                <template #prepend>
                  <q-icon name="fa-solid fa-circle-check" size="15px" class="input-icon" />
                </template>
                <template #append>
                  <q-btn
                    flat
                    round
                    dense
                    size="sm"
                    :icon="isConfirmPwdVisible ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"
                    class="toggle-pwd-btn"
                    @click="isConfirmPwdVisible = !isConfirmPwdVisible"
                  />
                </template>
              </q-input>
            </div>

            <div class="form-action">
              <button
                type="submit"
                class="submit-btn"
                :disabled="authStore.isLoading"
              >
                <q-spinner-tail v-if="authStore.isLoading" size="20px" />
                <template v-else>
                  <span>Xác nhận đặt lại mật khẩu</span>
                  <q-icon name="fa-solid fa-check" size="14px" class="btn-arrow" />
                </template>
              </button>
            </div>
          </q-form>

          <div class="card-back-link text-center">
            <a href="javascript:void(0)" class="back-link" @click="currentStep = 1">
              <q-icon name="fa-solid fa-arrow-left" size="13px" />
              <span>Đổi email hoặc gửi lại OTP</span>
            </a>
          </div>
        </template>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '@/stores/auth.store'

const $q = useQuasar()
const router = useRouter()
const authStore = useAuthStore()

const currentStep = ref<1 | 2>(1)
const email = ref('')
const receivedOtp = ref('')

const isPwdVisible = ref(false)
const isConfirmPwdVisible = ref(false)

const resetForm = reactive({
  otp: '',
  newPassword: '',
  confirmPassword: ''
})

async function handleSendOtp() {
  if (!email.value.trim()) return

  const result = await authStore.forgotPassword(email.value.trim())
  if (result.success) {
    if (result.otp) {
      receivedOtp.value = result.otp
      resetForm.otp = result.otp // Auto-fill for convenience
    }

    $q.notify({
      type: 'positive',
      message: result.message || 'Mã OTP đã được tạo thành công!',
      position: 'top',
      timeout: 3000
    })

    currentStep.value = 2
  } else {
    $q.notify({
      type: 'negative',
      message: result.message || 'Không thể tạo mã OTP. Vui lòng thử lại!',
      position: 'top',
      timeout: 3500
    })
  }
}

async function handleResetPassword() {
  if (!resetForm.otp || !resetForm.newPassword || !resetForm.confirmPassword) {
    return
  }

  if (resetForm.newPassword !== resetForm.confirmPassword) {
    $q.notify({
      type: 'warning',
      message: 'Mật khẩu xác nhận không trùng khớp.',
      position: 'top'
    })
    return
  }

  const result = await authStore.resetPassword({
    email: email.value.trim(),
    otp: resetForm.otp.trim(),
    newPassword: resetForm.newPassword
  })

  if (result.success) {
    $q.notify({
      type: 'positive',
      message: 'Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay.',
      position: 'top',
      timeout: 3000
    })

    await router.push('/login')
  } else {
    $q.notify({
      type: 'negative',
      message: result.message || 'Đặt lại mật khẩu thất bại. Vui lòng thử lại!',
      position: 'top',
      timeout: 3500
    })
  }
}
</script>

<style scoped lang="scss">
.forgot-page {
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

.forgot-wrapper {
  position: relative;
  width: 100%;
  max-width: 420px;
  margin: auto;
}

/* Subtle Glow Backdrop */
.card-glow-backdrop {
  position: absolute;
  inset: -8px;
  background: radial-gradient(circle, rgba(223, 38, 106, 0.1) 0%, rgba(99, 102, 241, 0.06) 50%, transparent 80%);
  border-radius: 24px;
  filter: blur(16px);
  pointer-events: none;
  z-index: 1;
}

/* Main Card */
.forgot-card {
  position: relative;
  z-index: 2;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 18px;
  padding: 28px 28px 24px 28px;
  box-shadow: 0 16px 36px -12px rgba(0, 0, 0, 0.08), 0 0 1px 1px rgba(0, 0, 0, 0.03);
}

/* Top Status Badge */
.card-top-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 9999px;
  background: #fdf2f6;
  border: 1px solid rgba(223, 38, 106, 0.2);
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
  margin-bottom: 16px;

  .forgot-title {
    font-family: var(--font-headline, sans-serif);
    font-size: 22px;
    font-weight: 800;
    letter-spacing: -0.025em;
    color: #0b1326;
    margin: 0 0 4px 0;
    line-height: 1.2;
  }

  .forgot-subtitle {
    font-family: var(--font-body, sans-serif);
    font-size: 13px;
    color: #64748b;
    margin: 0;
    line-height: 1.45;
  }
}

/* Dev OTP Hint Box */
.dev-otp-box {
  background: #f0fdf4;
  border: 1px solid #bbf7d0;
  border-radius: 10px;
  padding: 10px 14px;
  margin-bottom: 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;

  .otp-hint-title {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 12px;
    font-weight: 600;
    color: #166534;
  }

  .otp-code-highlight {
    font-family: var(--font-mono, monospace);
    font-size: 16px;
    font-weight: 800;
    letter-spacing: 0.12em;
    color: #15803d;
    background: #dcfce7;
    padding: 2px 8px;
    border-radius: 6px;
  }
}

/* Error Banner */
.error-banner {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-radius: 8px;
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
  font-size: 12.5px;
  font-weight: 500;
  margin-bottom: 14px;

  .error-icon {
    color: #ef4444;
    flex-shrink: 0;
  }
}

/* Form */
.forgot-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 700;
    color: #1e293b;
    letter-spacing: -0.01em;
  }
}

/* Custom Quasar Input */
:deep(.custom-input) {
  .q-field__control {
    background: #f8fafc !important;
    border-radius: 9px !important;
    border: 1px solid #cbd5e1 !important;
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
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.15) !important;
    background: #ffffff !important;
  }

  .q-field__native,
  input {
    color: #0f172a !important;
    font-size: 13.5px !important;
    font-family: var(--font-body, sans-serif);
    font-weight: 500;

    &::placeholder {
      color: #94a3b8 !important;
    }
  }

  .input-icon {
    color: #64748b;
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
  color: #64748b;

  &:hover {
    color: #0b1326;
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
    box-shadow: 0 4px 12px rgba(223, 38, 106, 0.3);

    .btn-arrow {
      transition: transform 0.2s ease;
    }

    &:hover:not(:disabled) {
      background: linear-gradient(135deg, #f43f7e, #df266a);
      transform: translateY(-2px);
      box-shadow: 0 6px 18px rgba(223, 38, 106, 0.4);

      .btn-arrow {
        transform: translateX(4px);
      }
    }

    &:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }
  }
}

/* Back Link */
.card-back-link {
  margin-top: 16px;
  padding-top: 12px;
  border-top: 1px solid #f1f5f9;

  .back-link {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    font-size: 12.5px;
    font-weight: 600;
    color: #64748b;
    text-decoration: none;
    cursor: pointer;
    transition: color 0.2s ease;

    &:hover {
      color: #df266a;
    }
  }
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
</style>
