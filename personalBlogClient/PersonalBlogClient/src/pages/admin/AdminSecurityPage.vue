<template>
  <q-page class="admin-security-page">
    <!-- 1. Header Section -->
    <header class="security-header-section">
      <div class="header-actions">
        <button
          type="button"
          class="btn-action-refresh"
          title="Tải lại thông tin tài khoản"
          :disabled="loading"
          @click="fetchAccountInfo"
        >
          <q-icon name="fa-solid fa-rotate-right" size="13px" :class="{ 'fa-spin': loading }" />
          <span>Làm mới</span>
        </button>
      </div>
    </header>

    <!-- 2. Loading State -->
    <div v-if="loading && !accountInfo" class="page-loading-box">
      <q-spinner-tail color="pink-7" size="36px" />
      <span class="loading-text">Đang tải thông tin tài khoản...</span>
    </div>

    <!-- 3. Main Security Content Grid (2 Columns) -->
    <main v-else class="security-main-grid">
      <!-- Left Column: Change Password -->
      <div class="grid-col left-col">
        <ChangePasswordCard @password-changed="fetchAccountInfo" />
      </div>

      <!-- Right Column: Account Credentials & Security Tips -->
      <div class="grid-col right-col">
        <AccountCredentialsCard :account-info="accountInfo" />
        <SecurityTipsCard />
      </div>
    </main>
  </q-page>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import AccountCredentialsCard from "@/components/admin/security/AccountCredentialsCard.vue";
import ChangePasswordCard from "@/components/admin/security/ChangePasswordCard.vue";
import SecurityTipsCard from "@/components/admin/security/SecurityTipsCard.vue";
import type { AdminAccountInfo } from "@/types/admin-security";
import { adminSecurityService } from "@/services/admin-security.service";
import { swalToast, swalError } from "@/utils/swal";

const loading = ref(false);
const accountInfo = ref<AdminAccountInfo | null>(null);

async function fetchAccountInfo() {
  try {
    loading.value = true;
    accountInfo.value = await adminSecurityService.getAccountInfo();
  } catch (err: any) {
    console.error("Failed to load admin account info:", err);
    swalError("Lỗi tải dữ liệu", err.response?.data?.message || "Không thể tải thông tin tài khoản.");
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  fetchAccountInfo();
});
</script>

<style scoped lang="scss">
.admin-security-page {
  max-width: 100%;
  margin: 0 auto;
  padding: 8px 10px 40px;
  display: flex;
  flex-direction: column;
  min-height: calc(100vh - 84px);
}

/* 1. Header Section */
.security-header-section {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 16px;
  margin-bottom: 20px;
  flex-wrap: wrap;

  .header-actions {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-left: auto;

    .btn-action-refresh {
      height: 40px;
      padding: 0 16px;
      border-radius: 10px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 700;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      background: #ffffff;
      border: 1px solid #e2e8f0;
      color: #475569;
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        border-color: #df266a;
        color: #df266a;
        background: #fdf2f6;
      }
    }
  }
}

/* 2. Loading Box */
.page-loading-box {
  padding: 60px 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;

  .loading-text {
    font-family: var(--font-headline, sans-serif);
    font-size: 14px;
    color: #64748b;
  }
}

/* 3. Main Security Grid */
.security-main-grid {
  display: grid;
  grid-template-columns: 1.15fr 0.85fr;
  gap: 20px;
  align-items: start;

  @media (max-width: 1080px) {
    grid-template-columns: 1fr;
  }

  .grid-col {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }
}
</style>
