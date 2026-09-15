<template>
  <q-dialog v-model="isOpen" transition-show="scale" transition-hide="scale" class="admin-comment-detail-dialog">
    <div v-if="comment" class="comment-modal-container">
      <!-- 1. Header Bar -->
      <header class="modal-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="isOpen = false">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="modal-kicker">CHI TIẾT BÌNH LUẬN #{{ comment.id }}</span>
            <h3 class="modal-main-title">
              {{ comment.authorName }}
            </h3>
          </div> -->
        </div>

        <div class="header-actions">
          <!-- Status Selector Pill -->
          <div class="status-selector-wrap">
            <button type="button" class="status-opt-btn opt-approved" :class="{ active: comment.status === 'Approved' }"
              @click="changeStatus('Approved')">
              <q-icon name="fa-solid fa-check" size="10px" />
              <span>Duyệt</span>
            </button>
            <button type="button" class="status-opt-btn opt-pending" :class="{ active: comment.status === 'Pending' }"
              @click="changeStatus('Pending')">
              <q-icon name="fa-solid fa-hourglass-half" size="10px" />
              <span>Chờ</span>
            </button>
            <button type="button" class="status-opt-btn opt-spam"
              :class="{ active: comment.status === 'Spam' || comment.status === 'Rejected' }"
              @click="changeStatus('Spam')">
              <q-icon name="fa-solid fa-ban" size="10px" />
              <span>Spam</span>
            </button>
          </div>
        </div>
      </header>

      <!-- 2. Modal Body -->
      <main class="comment-detail-body">
        <!-- Reader / Author Box -->
        <div class="detail-section-card">
          <div class="author-profile-row">
            <div class="author-avatar-box">
              <img v-if="comment.authorAvatar" :src="comment.authorAvatar" :alt="comment.authorName"
                class="author-avatar-img" />
              <div v-else class="author-avatar-fallback">
                {{ comment.authorName.charAt(0).toUpperCase() }}
              </div>
            </div>

            <div class="author-profile-meta">
              <div class="author-name-row">
                <span class="author-fullname">{{ comment.authorName }}</span>
                <span class="author-role-badge" :class="comment.userId ? 'role-member' : 'role-guest'">
                  <q-icon :name="comment.userId ? 'fa-solid fa-user-check' : 'fa-solid fa-user-clock'" size="10px"
                    class="q-mr-xs" />
                  {{ comment.userId ? 'Thành viên' : 'Khách vãng lai' }}
                </span>
              </div>

              <div class="author-sub-info">
                <span v-if="comment.authorEmail" class="email-info">
                  <q-icon name="fa-solid fa-envelope" size="10px" class="q-mr-xs" />
                  {{ comment.authorEmail }}
                </span>
                <span class="meta-dot">·</span>
                <span class="date-info">
                  <q-icon name="fa-solid fa-clock" size="10px" class="q-mr-xs" />
                  {{ formatDateTime(comment.createdAt) }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Target Post Card -->
        <div class="detail-section-card">
          <div class="section-label-mono">
            BÀI VIẾT BÌNH LUẬN
          </div>
          <div class="target-post-row">
            <div class="post-title-text">{{ comment.postTitle }}</div>
            <a :href="`/posts/${comment.postSlug}`" target="_blank" class="btn-view-post" title="Xem bài viết gốc">
              <span>Xem bài</span>
              <q-icon name="fa-solid fa-arrow-up-right-from-square" size="10px" />
            </a>
          </div>
        </div>

        <!-- Comment Content Card -->
        <div class="detail-section-card content-card">
          <div class="section-label-mono">
            NỘI DUNG BÌNH LUẬN
          </div>
          <div class="comment-content-text">
            {{ comment.content }}
          </div>
        </div>
      </main>

      <!-- 3. Modal Footer -->
      <footer class="modal-footer-bar">
        <button type="button" class="btn-delete-comment" @click="handleDelete">
          <q-icon name="fa-solid fa-trash-can" size="12px" class="q-mr-xs" />
          <span>Xóa bình luận này</span>
        </button>
        <button type="button" class="btn-close-modal" @click="isOpen = false">
          Đóng
        </button>
      </footer>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { AdminCommentSummary, AdminCommentDetail } from "@/types/admin-comment";
import { adminCommentService } from "@/services/admin-comment.service";
import { swalConfirm, swalSuccess, swalError, swalToast } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  comment: AdminCommentSummary | AdminCommentDetail | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "status-changed", newStatus: string): void;
  (e: "deleted", commentId: number): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

async function changeStatus(status: "Approved" | "Pending" | "Spam") {
  if (!props.comment) return;
  try {
    await adminCommentService.updateStatus(props.comment.id, status);
    props.comment.status = status;
    emit("status-changed", status);
    const label = status === "Approved" ? "Đã duyệt" : status === "Pending" ? "Chuyển chờ duyệt" : "Đã đánh dấu Spam";
    swalToast(`${label} bình luận thành công!`);
  } catch (err: any) {
    swalError("Không thể cập nhật trạng thái", err.response?.data?.message || "Lỗi hệ thống.");
  }
}

async function handleDelete() {
  if (!props.comment) return;

  const confirmed = await swalConfirm({
    title: "Xóa bình luận này?",
    text: `Bạn có chắc chắn muốn xóa vĩnh viễn bình luận của "${props.comment.authorName}"? Thao tác này không thể hoàn tác.`,
    confirmButtonText: "Xóa vĩnh viễn",
    cancelButtonText: "Hủy bỏ",
    isDanger: true,
    icon: "warning"
  });

  if (!confirmed) return;

  try {
    await adminCommentService.deleteComment(props.comment.id);
    swalSuccess("Đã xóa bình luận!");
    emit("deleted", props.comment.id);
    isOpen.value = false;
  } catch (err: any) {
    swalError("Lỗi khi xóa bình luận", err.response?.data?.message || "Không thể xóa bình luận.");
  }
}

function formatDateTime(dateStr: string): string {
  if (!dateStr) return "—";
  try {
    const d = new Date(dateStr);
    return d.toLocaleString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit"
    });
  } catch {
    return dateStr;
  }
}
</script>

<style scoped lang="scss">
.admin-comment-detail-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.comment-modal-container {
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  width: 640px;
  max-width: 95vw;
  max-height: 90vh;
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(0, 0, 0, 0.06);
  overflow: hidden;
}

/* Header */
.modal-header-bar {
  background: #ffffff;
  border-bottom: 1px solid #e2e8f0;
  height: 64px;
  padding: 0 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-shrink: 0;

  .header-left {
    display: flex;
    align-items: center;
    gap: 14px;
    min-width: 0;

    .btn-back-circle {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: #f1f5f9;
      border: 1px solid #e2e8f0;
      color: #475569;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      flex-shrink: 0;
      transition: all 0.2s ease;

      &:hover {
        background: #df266a;
        color: #ffffff;
        border-color: #df266a;
      }
    }

    .header-title-box {
      min-width: 0;

      .modal-kicker {
        font-family: var(--font-mono, monospace);
        font-size: 10px;
        font-weight: 700;
        color: #df266a;
        letter-spacing: 0.06em;
        display: block;
      }

      .modal-main-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 15px;
        font-weight: 700;
        color: #0b1326;
        margin: 0;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 280px;
      }
    }
  }

  .status-selector-wrap {
    display: flex;
    background: #f1f5f9;
    padding: 3px;
    border-radius: 9999px;
    border: 1px solid #e2e8f0;

    .status-opt-btn {
      background: transparent;
      border: none;
      padding: 5px 12px;
      border-radius: 9999px;
      font-family: var(--font-headline, sans-serif);
      font-size: 11.5px;
      font-weight: 600;
      color: #64748b;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 5px;
      transition: all 0.2s ease;

      &.opt-approved.active {
        background: #ecfdf5;
        color: #059669;
        font-weight: 700;
        box-shadow: 0 2px 6px rgba(5, 150, 105, 0.15);
      }

      &.opt-pending.active {
        background: #fffbeb;
        color: #d97706;
        font-weight: 700;
        box-shadow: 0 2px 6px rgba(217, 119, 6, 0.15);
      }

      &.opt-spam.active {
        background: #fff1f2;
        color: #e11d48;
        font-weight: 700;
        box-shadow: 0 2px 6px rgba(225, 29, 72, 0.15);
      }
    }
  }
}

/* Body */
.comment-detail-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  overflow-y: auto;

  .detail-section-card {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 8px;

    .section-label-mono {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      color: #64748b;
      letter-spacing: 0.05em;
    }
  }

  /* Author Profile */
  .author-profile-row {
    display: flex;
    align-items: center;
    gap: 14px;

    .author-avatar-box {
      width: 46px;
      height: 46px;
      border-radius: 50%;
      overflow: hidden;
      flex-shrink: 0;

      .author-avatar-img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }

      .author-avatar-fallback {
        width: 100%;
        height: 100%;
        background: #eef2ff;
        color: #4f46e5;
        font-family: var(--font-headline, sans-serif);
        font-size: 18px;
        font-weight: 800;
        display: flex;
        align-items: center;
        justify-content: center;
      }
    }

    .author-profile-meta {
      display: flex;
      flex-direction: column;
      gap: 3px;
      min-width: 0;

      .author-name-row {
        display: flex;
        align-items: center;
        gap: 8px;

        .author-fullname {
          font-family: var(--font-headline, sans-serif);
          font-size: 15px;
          font-weight: 700;
          color: #0b1326;
        }

        .author-role-badge {
          font-family: var(--font-mono, monospace);
          font-size: 10px;
          font-weight: 700;
          padding: 2px 7px;
          border-radius: 4px;

          &.role-member {
            background: #ecfdf5;
            color: #059669;
            border: 1px solid #a7f3d0;
          }

          &.role-guest {
            background: #f1f5f9;
            color: #64748b;
            border: 1px solid #e2e8f0;
          }
        }
      }

      .author-sub-info {
        display: flex;
        align-items: center;
        gap: 6px;
        font-size: 11.5px;
        color: #64748b;

        .meta-dot {
          color: #cbd5e1;
        }
      }
    }
  }

  /* Target Post */
  .target-post-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 12px;

    .post-title-text {
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      color: #0b1326;
      line-height: 1.4;
    }

    .btn-view-post {
      background: #f8fafc;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
      padding: 5px 10px;
      font-family: var(--font-headline, sans-serif);
      font-size: 11.5px;
      font-weight: 600;
      color: #475569;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      text-decoration: none;
      flex-shrink: 0;
      transition: all 0.2s ease;

      &:hover {
        background: #df266a;
        color: #ffffff;
        border-color: #df266a;
      }
    }
  }

  /* Comment Text */
  .content-card {
    .comment-content-text {
      font-family: var(--font-body, sans-serif);
      font-size: 14px;
      line-height: 1.6;
      color: #1e293b;
      white-space: pre-wrap;
      word-break: break-word;
      background: #f8fafc;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      padding: 12px 16px;
      margin-top: 4px;
    }
  }
}

/* Footer */
.modal-footer-bar {
  background: #ffffff;
  border-top: 1px solid #e2e8f0;
  height: 60px;
  padding: 0 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-shrink: 0;

  .btn-delete-comment {
    background: transparent;
    border: none;
    color: #e11d48;
    font-family: var(--font-headline, sans-serif);
    font-size: 12.5px;
    font-weight: 600;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    transition: all 0.2s ease;

    &:hover {
      text-decoration: underline;
    }
  }

  .btn-close-modal {
    background: #f1f5f9;
    border: 1px solid #e2e8f0;
    padding: 7px 18px;
    border-radius: 9999px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: #475569;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      background: #e2e8f0;
      color: #0b1326;
    }
  }
}
</style>
