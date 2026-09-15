<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale" class="admin-tag-editor-dialog">
    <div class="tag-modal-container">
      <!-- 1. Header Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA THẺ BÀI VIẾT' : 'THÊM THẺ BÀI VIẾT MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.name ? `#${form.name}` : 'Chưa đặt tên thẻ...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <button type="button" class="btn-cancel-flat" @click="handleCancel">
            Hủy bỏ
          </button>
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo thẻ' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Modal Body Form -->
      <main class="tag-form-body">
        <!-- Tag Name Field -->
        <div class="form-group-field">
          <label class="field-label-mono">
            TÊN THẺ (TAG NAME) <span class="text-danger">*</span>
          </label>
          <div class="input-tag-name-wrap">
            <span class="tag-hash-prefix">#</span>
            <input v-model="form.name" type="text" placeholder="Ví dụ: vuejs, csharp, clean-code, architecture..."
              class="input-name-field" @input="onNameChange" />
          </div>
        </div>

        <!-- Slug Row with Auto-Generate Helper -->
        <div class="form-slug-row">
          <span class="slug-prefix">
            <q-icon name="fa-solid fa-link" size="11px" class="q-mr-xs" />
            Đường dẫn: /tags/
          </span>
          <input v-model="form.slug" type="text" placeholder="duong-dan-the" class="input-slug-field" />
          <button type="button" class="btn-regenerate-slug" title="Tạo lại slug tự động theo tên thẻ"
            @click="regenerateSlug">
            <q-icon name="fa-solid fa-rotate-right" size="11px" />
            <span>Tạo lại</span>
          </button>
        </div>

        <!-- Tag Preview Box -->
        <div class="tag-preview-card">
          <div class="preview-card-title">
            <q-icon name="fa-solid fa-tags" size="12px" class="q-mr-xs text-rose" />
            XEM TRƯỚC HIỂN THỊ
          </div>
          <div class="preview-card-content">
            <span v-if="form.name" class="live-tag-badge">
              #{{ form.name }}
            </span>
            <span v-else class="text-placeholder-muted">
              Nhập tên thẻ ở trên để xem trước giao diện thẻ bài viết...
            </span>
          </div>
        </div>
      </main>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from "vue";
import type {
  AdminTagSummary,
  AdminTagDetail,
  CreateTagPayload,
  UpdateTagPayload
} from "@/types/admin-tag";
import { adminTagService } from "@/services/admin-tag.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  tag?: AdminTagSummary | AdminTagDetail | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", tag: AdminTagDetail): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.tag && props.tag.id > 0);
const submitting = ref(false);

const form = reactive<{
  name: string;
  slug: string;
}>({
  name: "",
  slug: ""
});

function slugify(text: string): string {
  return text
    .toString()
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/đ/g, "d")
    .replace(/Đ/g, "d")
    .replace(/[^a-z0-9\s-]/g, "")
    .trim()
    .replace(/\s+/g, "-")
    .replace(/-+/g, "-");
}

function onNameChange() {
  form.name = form.name.replace(/^#+/, "");
  if (!isEdit.value || !form.slug) {
    form.slug = slugify(form.name);
  }
}

function regenerateSlug() {
  form.slug = slugify(form.name || "the-bai-viet");
}

watch(
  () => props.modelValue,
  async (newVal) => {
    if (newVal) {
      if (props.tag && props.tag.id > 0) {
        form.name = props.tag.name.replace(/^#+/, "");
        form.slug = props.tag.slug;
      } else {
        form.name = "";
        form.slug = "";
      }
    }
  }
);

async function handleCancel() {
  if (form.name.trim()) {
    const ok = await swalConfirm({
      title: "Hủy thay đổi?",
      text: "Thông tin thẻ bạn vừa nhập sẽ không được lưu lại. Bạn có chắc muốn thoát?",
      confirmButtonText: "Thoát không lưu",
      cancelButtonText: "Tiếp tục chỉnh sửa",
      icon: "warning"
    });
    if (!ok) return;
  }
  isOpen.value = false;
}

async function handleSubmit() {
  const cleanName = form.name.trim().replace(/^#+/, "");
  if (!cleanName) {
    swalError("Thiếu tên thẻ bài viết", "Vui lòng nhập tên thẻ trước khi lưu.");
    return;
  }

  try {
    submitting.value = true;
    let savedTag: AdminTagDetail;

    if (isEdit.value && props.tag) {
      const payload: UpdateTagPayload = {
        name: cleanName,
        slug: form.slug.trim() || undefined
      };
      savedTag = await adminTagService.updateTag(props.tag.id, payload);
      swalSuccess("Cập nhật thành công!", `Thẻ #${savedTag.name} đã được cập nhật.`);
    } else {
      const payload: CreateTagPayload = {
        name: cleanName,
        slug: form.slug.trim() || undefined
      };
      savedTag = await adminTagService.createTag(payload);
      swalSuccess("Thêm mới thành công!", `Thẻ #${savedTag.name} đã được tạo.`);
    }

    emit("saved", savedTag);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã xảy ra lỗi khi lưu thẻ bài viết. Vui lòng thử lại.";
    swalError("Lỗi lưu thẻ", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-tag-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.tag-modal-container {
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  width: 560px;
  max-width: 95vw;
  max-height: 90vh;
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(0, 0, 0, 0.06);
  overflow: hidden;
}

/* Header */
.editor-header-bar {
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

      .editor-kicker {
        font-family: var(--font-mono, monospace);
        font-size: 10px;
        font-weight: 700;
        color: #df266a;
        letter-spacing: 0.06em;
        display: block;
      }

      .editor-main-title {
        font-family: var(--font-headline, sans-serif);
        font-size: 15px;
        font-weight: 700;
        color: #0b1326;
        margin: 0;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 260px;
      }
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 10px;

    .btn-cancel-flat {
      background: transparent;
      border: 1px solid #e2e8f0;
      padding: 8px 16px;
      border-radius: 9999px;
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 600;
      color: #64748b;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: #f1f5f9;
        color: #0b1326;
      }
    }

    .btn-save-primary {
      background: #df266a;
      color: #ffffff;
      font-family: var(--font-headline, sans-serif);
      font-size: 13.5px;
      font-weight: 700;
      padding: 8px 18px;
      border-radius: 9999px;
      border: none;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 8px;
      box-shadow: 0 4px 12px rgba(223, 38, 106, 0.25);
      transition: all 0.2s ease;

      &:hover:not(:disabled) {
        background: #be185d;
        transform: translateY(-1px);
        box-shadow: 0 6px 16px rgba(223, 38, 106, 0.35);
      }

      &:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }
    }
  }
}

/* Body */
.tag-form-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 18px;
  overflow-y: auto;

  .form-group-field {
    display: flex;
    flex-direction: column;
    gap: 6px;
  }

  .field-label-mono {
    font-family: var(--font-mono, monospace);
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.06em;
    color: #475569;

    .text-danger {
      color: #e11d48;
    }
  }

  .input-tag-name-wrap {
    display: flex;
    align-items: center;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 0 14px;
    transition: all 0.2s ease;

    &:focus-within {
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }

    .tag-hash-prefix {
      font-family: var(--font-headline, sans-serif);
      font-size: 18px;
      font-weight: 800;
      color: #df266a;
      margin-right: 6px;
    }

    .input-name-field {
      flex: 1;
      border: none;
      outline: none;
      padding: 12px 0;
      font-family: var(--font-headline, sans-serif);
      font-size: 16px;
      font-weight: 700;
      color: #0b1326;
      background: transparent;
    }
  }

  .form-slug-row {
    display: flex;
    align-items: center;
    gap: 8px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    padding: 6px 12px;

    .slug-prefix {
      font-family: var(--font-mono, monospace);
      font-size: 11.5px;
      font-weight: 600;
      color: #64748b;
      white-space: nowrap;
    }

    .input-slug-field {
      flex: 1;
      border: none;
      outline: none;
      font-family: var(--font-mono, monospace);
      font-size: 12px;
      color: #0b1326;
      background: transparent;
    }

    .btn-regenerate-slug {
      background: #f1f5f9;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
      padding: 4px 8px;
      font-size: 11px;
      font-weight: 600;
      color: #475569;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 4px;
      transition: all 0.2s ease;

      &:hover {
        background: #e2e8f0;
        color: #0b1326;
      }
    }
  }

  .tag-preview-card {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 10px;

    .preview-card-title {
      font-family: var(--font-mono, monospace);
      font-size: 10.5px;
      font-weight: 700;
      color: #64748b;
      letter-spacing: 0.05em;
    }

    .preview-card-content {
      min-height: 38px;
      display: flex;
      align-items: center;

      .live-tag-badge {
        font-family: var(--font-mono, monospace);
        font-size: 13px;
        font-weight: 700;
        color: #059669;
        background: #ecfdf5;
        border: 1px solid #a7f3d0;
        padding: 6px 14px;
        border-radius: 6px;
        display: inline-flex;
        align-items: center;
      }

      .text-placeholder-muted {
        font-size: 12px;
        color: #94a3b8;
        font-style: italic;
      }
    }
  }
}
</style>
