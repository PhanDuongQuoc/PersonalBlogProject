<template>
  <div class="security-card change-password-card">
    <div class="card-header">
      <div class="header-left">
        <div class="header-icon-box icon-rose">
          <q-icon name="fa-solid fa-key" size="15px" />
        </div>
        <div>
          <h3 class="card-title">Đổi Mật Khẩu Quản Trị</h3>
          <p class="card-subtitle">Cập nhật mật khẩu định kỳ để nâng cao an toàn tài khoản</p>
        </div>
      </div>
      <div class="header-right">
        <span class="encryption-badge">
          <q-icon name="fa-solid fa-lock" size="10px" class="q-mr-xs" />
          Mã hóa BCrypt
        </span>
      </div>
    </div>

    <!-- Change Password Form -->
    <form @submit.prevent="handleSubmit" class="password-form">
      <!-- 1. Current Password -->
      <div class="form-group">
        <label class="form-label">
          <span>Mật khẩu hiện tại</span>
          <span class="required-star">*</span>
        </label>
        <div class="input-wrapper">
          <q-icon name="fa-solid fa-shield" size="14px" class="input-leading-icon" />
          <input
            v-model="form.currentPassword"
            :type="showCurrentPassword ? 'text' : 'password'"
            placeholder="Nhập mật khẩu bạn đang sử dụng"
            class="form-input"
            autocomplete="current-password"
            required
          />
          <button
            type="button"
            class="toggle-eye-btn"
            @click="showCurrentPassword = !showCurrentPassword"
            tabindex="-1"
            title="Ẩn / Hiện mật khẩu"
          >
            <q-icon :name="showCurrentPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'" size="14px" />
          </button>
        </div>
      </div>

      <!-- 2. New Password -->
      <div class="form-group">
        <label class="form-label">
          <span>Mật khẩu mới</span>
          <span class="required-star">*</span>
        </label>
        <div class="input-wrapper">
          <q-icon name="fa-solid fa-lock" size="14px" class="input-leading-icon" />
          <input
            v-model="form.newPassword"
            :type="showNewPassword ? 'text' : 'password'"
            placeholder="Nhập mật khẩu mới an toàn"
            class="form-input"
            autocomplete="new-password"
            required
            @input="checkPasswordStrength"
          />
          <button
            type="button"
            class="toggle-eye-btn"
            @click="showNewPassword = !showNewPassword"
            tabindex="-1"
            title="Ẩn / Hiện mật khẩu"
          >
            <q-icon :name="showNewPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'" size="14px" />
          </button>
        </div>

        <!-- Password Strength Meter -->
        <div v-if="form.newPassword" class="strength-meter-box">
          <div class="strength-header">
            <span class="strength-label">Độ mạnh mật khẩu:</span>
            <span class="strength-level" :class="strengthClass">{{ strengthLabel }}</span>
          </div>
          <div class="strength-bar-track">
            <div
              class="strength-bar-fill"
              :class="strengthClass"
              :style="{ width: `${strengthScore}%` }"
            ></div>
          </div>
        </div>

        <!-- Requirements Check List -->
        <div class="requirements-grid">
          <div class="req-item" :class="{ met: reqs.minLength }">
            <q-icon :name="reqs.minLength ? 'fa-solid fa-circle-check' : 'fa-regular fa-circle'" size="11px" />
            <span>Tối thiểu 8 ký tự</span>
          </div>
          <div class="req-item" :class="{ met: reqs.hasUpperLower }">
            <q-icon :name="reqs.hasUpperLower ? 'fa-solid fa-circle-check' : 'fa-regular fa-circle'" size="11px" />
            <span>Chữ hoa & chữ thường</span>
          </div>
          <div class="req-item" :class="{ met: reqs.hasNumber }">
            <q-icon :name="reqs.hasNumber ? 'fa-solid fa-circle-check' : 'fa-regular fa-circle'" size="11px" />
            <span>Ít nhất 1 số (0-9)</span>
          </div>
          <div class="req-item" :class="{ met: reqs.hasSpecial }">
            <q-icon :name="reqs.hasSpecial ? 'fa-solid fa-circle-check' : 'fa-regular fa-circle'" size="11px" />
            <span>Ký tự đặc biệt (!@#$...)</span>
          </div>
        </div>
      </div>

      <!-- 3. Confirm Password -->
      <div class="form-group">
        <label class="form-label">
          <span>Xác nhận mật khẩu mới</span>
          <span class="required-star">*</span>
        </label>
        <div class="input-wrapper" :class="{ 'input-mismatch': isConfirmMismatch }">
          <q-icon name="fa-solid fa-check-double" size="14px" class="input-leading-icon" />
          <input
            v-model="form.confirmPassword"
            :type="showConfirmPassword ? 'text' : 'password'"
            placeholder="Nhập lại mật khẩu mới"
            class="form-input"
            autocomplete="new-password"
            required
          />
          <button
            type="button"
            class="toggle-eye-btn"
            @click="showConfirmPassword = !showConfirmPassword"
            tabindex="-1"
            title="Ẩn / Hiện mật khẩu"
          >
            <q-icon :name="showConfirmPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'" size="14px" />
          </button>
        </div>

        <div v-if="isConfirmMismatch" class="mismatch-msg">
          <q-icon name="fa-solid fa-triangle-exclamation" size="11px" class="q-mr-xs" />
          Mật khẩu xác nhận chưa trùng khớp!
        </div>
        <div v-else-if="form.confirmPassword && form.confirmPassword === form.newPassword" class="match-msg">
          <q-icon name="fa-solid fa-circle-check" size="11px" class="q-mr-xs" />
          Mật khẩu xác nhận hoàn toàn trùng khớp.
        </div>
      </div>

      <!-- Actions Button -->
      <div class="form-actions">
        <button
          type="submit"
          class="btn-save-password"
          :disabled="submitting || isConfirmMismatch || !isFormValid"
        >
          <q-spinner-tail v-if="submitting" size="16px" color="white" />
          <q-icon v-else name="fa-solid fa-floppy-disk" size="13px" />
          <span>{{ submitting ? 'Đang lưu mật khẩu...' : 'Cập nhật Mật khẩu' }}</span>
        </button>

        <button
          type="button"
          class="btn-reset-form"
          :disabled="submitting"
          @click="resetForm"
        >
          <span>Hủy / Đặt lại</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from "vue";
import { adminSecurityService } from "@/services/admin-security.service";
import { swalToast, swalError } from "@/utils/swal";

const emit = defineEmits<{
  (e: "password-changed"): void;
}>();

const form = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: ""
});

const showCurrentPassword = ref(false);
const showNewPassword = ref(false);
const showConfirmPassword = ref(false);
const submitting = ref(false);

const reqs = reactive({
  minLength: false,
  hasUpperLower: false,
  hasNumber: false,
  hasSpecial: false
});

function checkPasswordStrength() {
  const pwd = form.newPassword || "";
  reqs.minLength = pwd.length >= 8;
  reqs.hasUpperLower = /[a-z]/.test(pwd) && /[A-Z]/.test(pwd);
  reqs.hasNumber = /[0-9]/.test(pwd);
  reqs.hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(pwd);
}

const strengthScore = computed(() => {
  if (!form.newPassword) return 0;
  let score = 0;
  if (form.newPassword.length >= 6) score += 20;
  if (reqs.minLength) score += 20;
  if (reqs.hasUpperLower) score += 25;
  if (reqs.hasNumber) score += 20;
  if (reqs.hasSpecial) score += 15;
  return Math.min(score, 100);
});

const strengthLabel = computed(() => {
  const s = strengthScore.value;
  if (s === 0) return "Chưa nhập";
  if (s < 40) return "Yếu";
  if (s < 70) return "Trung bình";
  if (s < 90) return "Mạnh";
  return "Rất mạnh (Tối ưu)";
});

const strengthClass = computed(() => {
  const s = strengthScore.value;
  if (s < 40) return "level-weak";
  if (s < 70) return "level-medium";
  if (s < 90) return "level-strong";
  return "level-excellent";
});

const isConfirmMismatch = computed(() => {
  return form.confirmPassword.length > 0 && form.newPassword !== form.confirmPassword;
});

const isFormValid = computed(() => {
  return (
    form.currentPassword.length > 0 &&
    form.newPassword.length >= 6 &&
    form.newPassword === form.confirmPassword
  );
});

function resetForm() {
  form.currentPassword = "";
  form.newPassword = "";
  form.confirmPassword = "";
  checkPasswordStrength();
}

async function handleSubmit() {
  if (!isFormValid.value) return;

  try {
    submitting.value = true;
    const res = await adminSecurityService.changePassword({
      currentPassword: form.currentPassword,
      newPassword: form.newPassword,
      confirmPassword: form.confirmPassword
    });

    swalToast(res.message || "Đổi mật khẩu thành công!", "success");
    resetForm();
    emit("password-changed");
  } catch (err: any) {
    const msg = err.response?.data?.message || "Không thể đổi mật khẩu. Vui lòng thử lại!";
    swalError("Đổi mật khẩu thất bại", msg);
  } finally {
    submitting.value = false;
  }
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

      &.icon-rose {
        background: rgba(223, 38, 106, 0.12);
        color: #df266a;
        border: 1px solid rgba(223, 38, 106, 0.25);
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

  .encryption-badge {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    background: var(--bg-surface-lowest, #f8fafc);
    color: var(--text-secondary, #475569);
    border: 1px solid var(--border-hairline, #e2e8f0);
    padding: 3px 8px;
    border-radius: 6px;
  }
}

/* Password Form */
.password-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;

  .form-label {
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 700;
    color: var(--text-secondary, #334155);
    display: flex;
    align-items: center;
    gap: 4px;

    .required-star {
      color: #df266a;
    }
  }

  .input-wrapper {
    position: relative;
    display: flex;
    align-items: center;
    background: var(--bg-surface-lowest, #f8fafc);
    border: 1px solid var(--border-hairline, #e2e8f0);
    border-radius: 9px;
    transition: all 0.2s ease;

    &:focus-within {
      background: var(--bg-surface-low, #ffffff);
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }

    .input-leading-icon {
      position: absolute;
      left: 12px;
      color: var(--text-muted, #94a3b8);
      pointer-events: none;
    }

    .form-input {
      width: 100%;
      height: 42px;
      background: transparent;
      border: none;
      border-radius: 9px;
      padding: 0 38px 0 36px;
      font-family: var(--font-body, sans-serif);
      font-size: 13.5px;
      color: var(--text-primary, #0b1326);
      outline: none;

      &::placeholder {
        color: var(--text-muted, #94a3b8);
        font-size: 13px;
      }
    }

    &.input-mismatch {
      border-color: #ef4444;
      background: rgba(239, 68, 68, 0.08);

      &:focus-within {
        box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.15);
      }
    }

    .toggle-eye-btn {
      position: absolute;
      right: 10px;
      background: none;
      border: none;
      color: var(--text-muted, #64748b);
      cursor: pointer;
      padding: 4px 6px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: color 0.2s ease;

      &:hover {
        color: var(--text-primary, #0b1326);
      }
    }
  }

  .mismatch-msg {
    font-size: 11.5px;
    font-weight: 600;
    color: #dc2626;
    margin-top: 2px;
  }

  .match-msg {
    font-size: 11.5px;
    font-weight: 600;
    color: #059669;
    margin-top: 2px;
  }
}

/* Strength Meter */
.strength-meter-box {
  margin-top: 8px;
  padding: 10px 12px;
  background: var(--bg-surface-lowest, #f8fafc);
  border-radius: 8px;
  border: 1px solid var(--border-hairline, #f1f5f9);

  .strength-header {
    display: flex;
    justify-content: space-between;
    font-size: 11.5px;
    margin-bottom: 6px;

    .strength-label {
      color: var(--text-muted, #64748b);
    }

    .strength-level {
      font-weight: 700;
      font-family: var(--font-mono, monospace);

      &.level-weak { color: #dc2626; }
      &.level-medium { color: #d97706; }
      &.level-strong { color: #059669; }
      &.level-excellent { color: #4f46e5; }
    }
  }

  .strength-bar-track {
    height: 5px;
    background: var(--border-hairline, #e2e8f0);
    border-radius: 9999px;
    overflow: hidden;

    .strength-bar-fill {
      height: 100%;
      border-radius: 9999px;
      transition: all 0.3s ease;

      &.level-weak { background: #ef4444; }
      &.level-medium { background: #f59e0b; }
      &.level-strong { background: #10b981; }
      &.level-excellent { background: linear-gradient(90deg, #10b981, #4f46e5); }
    }
  }
}

/* Requirements Check List */
.requirements-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 6px 12px;
  margin-top: 8px;

  @media (max-width: 540px) {
    grid-template-columns: 1fr;
  }

  .req-item {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 11.5px;
    color: var(--text-muted, #94a3b8);
    transition: color 0.2s ease;

    &.met {
      color: #059669;
      font-weight: 600;
    }
  }
}

/* Actions */
.form-actions {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 10px;

  .btn-save-password {
    height: 42px;
    padding: 0 20px;
    background: #df266a;
    color: #ffffff;
    border: none;
    border-radius: 10px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13.5px;
    font-weight: 700;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    box-shadow: 0 4px 14px rgba(223, 38, 106, 0.25);
    transition: all 0.2s ease;

    &:hover:not(:disabled) {
      background: #be185d;
      transform: translateY(-1px);
      box-shadow: 0 6px 18px rgba(223, 38, 106, 0.35);
    }

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
      box-shadow: none;
    }
  }

  .btn-reset-form {
    height: 42px;
    padding: 0 16px;
    background: var(--bg-surface-low, #ffffff);
    border: 1px solid var(--border-hairline, #e2e8f0);
    color: var(--text-secondary, #475569);
    border-radius: 10px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover:not(:disabled) {
      border-color: var(--border-subtle, #cbd5e1);
      background: var(--bg-surface-lowest, #f8fafc);
      color: var(--text-primary, #0b1326);
    }
  }
}
</style>
