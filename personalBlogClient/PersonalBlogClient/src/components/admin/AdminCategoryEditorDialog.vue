<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale"
    class="admin-category-editor-dialog">
    <div class="category-modal-container">
      <!-- 1. Header Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA CHỦ ĐỀ & DANH MỤC' : 'THÊM CHỦ ĐỀ & DANH MỤC MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.name ? form.name : 'Chưa đặt tên chủ đề...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <button type="button" class="btn-cancel-flat" @click="handleCancel">
            Hủy bỏ
          </button>
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo chủ đề' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Modal Body Form -->
      <main class="category-form-body">
        <!-- Category Name Field -->
        <div class="form-group-field">
          <label class="field-label-mono">
            TÊN CHỦ ĐỀ & DANH MỤC <span class="text-danger">*</span>
          </label>
          <input v-model="form.name" type="text" placeholder="Ví dụ: Kiến trúc Phần mềm, AI & Prompt Engineering..."
            class="input-name-field" @input="onNameChange" />
        </div>

        <!-- Slug Row with Auto-Generate Helper -->
        <div class="form-slug-row">
          <span class="slug-prefix">
            <q-icon name="fa-solid fa-link" size="11px" class="q-mr-xs" />
            Đường dẫn: /topics/
          </span>
          <input v-model="form.slug" type="text" placeholder="kien-truc-phan-mem" class="input-slug-field" />
          <button type="button" class="btn-regenerate-slug" title="Tạo lại slug tự động theo tên danh mục"
            @click="regenerateSlug">
            <q-icon name="fa-solid fa-rotate-right" size="11px" />
            <span>Tạo lại</span>
          </button>
        </div>

        <!-- Description Field -->
        <div class="form-group-field">
          <label class="field-label-mono">
            MÔ TẢ NGẮN (DESCRIPTION)
          </label>
          <textarea v-model="form.description" rows="3"
            placeholder="Mô tả tóm tắt nội dung của chủ đề này nhằm giúp độc giả dễ theo dõi và hỗ trợ chuẩn SEO..."
            class="textarea-description"></textarea>
        </div>

        <!-- Thumbnail URL & Live Preview -->
        <div class="form-group-field">
          <label class="field-label-mono">
            ẢNH BÌA / ĐẠI DIỆN CHỦ ĐỀ (THUMBNAIL URL)
          </label>
          <input v-model="form.thumbnailUrl" type="text" placeholder="https://images.unsplash.com/photo-..."
            class="input-thumb-url" />

          <!-- Live Preview Frame -->
          <div class="thumbnail-preview-frame">
            <img v-if="form.thumbnailUrl" :src="form.thumbnailUrl" alt="Category Thumbnail Preview" class="preview-img"
              @error="onImageError" />
            <div v-else class="preview-placeholder">
              <q-icon name="fa-regular fa-image" size="28px" class="placeholder-icon" />
              <div class="placeholder-text">Chưa có ảnh đại diện</div>
              <div class="placeholder-sub">Nhập đường dẫn URL ảnh Unsplash hoặc ảnh trực tuyến để xem trước</div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from "vue";
import type {
  AdminCategorySummary,
  AdminCategoryDetail,
  CreateCategoryPayload,
  UpdateCategoryPayload
} from "@/types/admin-category";
import { adminCategoryService } from "@/services/admin-category.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  category?: AdminCategorySummary | AdminCategoryDetail | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", category: AdminCategoryDetail): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.category && props.category.id > 0);
const submitting = ref(false);

const form = reactive<{
  name: string;
  slug: string;
  description: string;
  thumbnailUrl: string;
}>({
  name: "",
  slug: "",
  description: "",
  thumbnailUrl: ""
});

// Helper create slug
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
  if (!isEdit.value || !form.slug) {
    form.slug = slugify(form.name);
  }
}

function regenerateSlug() {
  form.slug = slugify(form.name || "chu-de");
}

function onImageError(e: Event) {
  (e.target as HTMLImageElement).src =
    "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=800&auto=format&fit=crop&q=60";
}

// Watch dialog opening to fill/reset form
watch(
  () => props.modelValue,
  async (newVal) => {
    if (newVal) {
      if (props.category && props.category.id > 0) {
        form.name = props.category.name;
        form.slug = props.category.slug;
        form.description = props.category.description || "";
        form.thumbnailUrl = props.category.thumbnailUrl || "";
      } else {
        form.name = "";
        form.slug = "";
        form.description = "";
        form.thumbnailUrl = "";
      }
    }
  }
);

async function handleCancel() {
  if (form.name.trim() || form.description.trim()) {
    const ok = await swalConfirm({
      title: "Hủy thay đổi?",
      text: "Các thông tin bạn vừa nhập sẽ không được lưu lại. Bạn có chắc muốn thoát?",
      confirmButtonText: "Thoát không lưu",
      cancelButtonText: "Tiếp tục chỉnh sửa",
      icon: "warning"
    });
    if (!ok) return;
  }
  isOpen.value = false;
}

async function handleSubmit() {
  if (!form.name.trim()) {
    swalError("Thiếu tên chủ đề", "Vui lòng nhập tên chủ đề & danh mục trước khi lưu.");
    return;
  }

  try {
    submitting.value = true;
    let savedCategory: AdminCategoryDetail;

    if (isEdit.value && props.category) {
      const payload: UpdateCategoryPayload = {
        name: form.name.trim(),
        slug: form.slug.trim(),
        description: form.description.trim() || undefined,
        thumbnailUrl: form.thumbnailUrl.trim() || undefined
      };
      savedCategory = await adminCategoryService.updateCategory(props.category.id, payload);
      swalSuccess("Cập nhật thành công!", `Chủ đề "${savedCategory.name}" đã được cập nhật.`);
    } else {
      const payload: CreateCategoryPayload = {
        name: form.name.trim(),
        slug: form.slug.trim(),
        description: form.description.trim() || undefined,
        thumbnailUrl: form.thumbnailUrl.trim() || undefined
      };
      savedCategory = await adminCategoryService.createCategory(payload);
      swalSuccess("Thêm mới thành công!", `Chủ đề "${savedCategory.name}" đã được tạo.`);
    }

    emit("saved", savedCategory);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã xảy ra lỗi khi lưu danh mục. Vui lòng thử lại.";
    swalError("Lỗi lưu chủ đề", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-category-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.category-modal-container {
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  width: 680px;
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
        max-width: 320px;
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

/* Body Form */
.category-form-body {
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

  .input-name-field {
    width: 100%;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 12px 16px;
    font-family: var(--font-headline, sans-serif);
    font-size: 16px;
    font-weight: 700;
    color: #0b1326;
    outline: none;
    transition: all 0.2s ease;

    &:focus {
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
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

  .textarea-description {
    width: 100%;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 10px;
    padding: 10px 14px;
    font-family: var(--font-body, sans-serif);
    font-size: 13.5px;
    color: #0b1326;
    outline: none;
    resize: vertical;
    transition: all 0.2s ease;

    &:focus {
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }
  }

  .input-thumb-url {
    width: 100%;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 10px;
    padding: 9px 14px;
    font-family: var(--font-mono, monospace);
    font-size: 12px;
    color: #0b1326;
    outline: none;
    transition: all 0.2s ease;

    &:focus {
      border-color: #df266a;
      box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
    }
  }

  .thumbnail-preview-frame {
    width: 100%;
    height: 160px;
    border-radius: 12px;
    overflow: hidden;
    background: #ffffff;
    border: 1px dashed #cbd5e1;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-top: 4px;

    .preview-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .preview-placeholder {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 6px;
      color: #94a3b8;
      text-align: center;
      padding: 16px;

      .placeholder-icon {
        color: #cbd5e1;
      }

      .placeholder-text {
        font-family: var(--font-headline, sans-serif);
        font-size: 13px;
        font-weight: 700;
        color: #64748b;
      }

      .placeholder-sub {
        font-size: 11px;
        color: #94a3b8;
      }
    }
  }
}
</style>
