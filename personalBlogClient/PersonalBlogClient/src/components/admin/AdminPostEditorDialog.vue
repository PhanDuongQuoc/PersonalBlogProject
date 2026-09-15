<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale"
    class="admin-post-editor-dialog">
    <div class="editor-modal-container">
      <!-- 1. Top Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA BÀI VIẾT' : 'SOẠN THẢO BÀI VIẾT MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.title ? form.title : 'Chưa đặt tiêu đề...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <!-- Status Selector Pill -->
          <div class="status-selector-wrap">
            <button type="button" class="status-opt-btn" :class="{ active: form.status === 'Draft' }"
              @click="form.status = 'Draft'">
              <q-icon name="fa-solid fa-file-pen" size="11px" />
              <span>Bản nháp</span>
            </button>
            <button type="button" class="status-opt-btn" :class="{ active: form.status === 'Published' }"
              @click="form.status = 'Published'">
              <q-icon name="fa-solid fa-paper-plane" size="11px" />
              <span>Xuất bản</span>
            </button>
          </div>

          <!-- Save Button -->
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo bài viết' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Main Editor Body: 2 Columns Layout -->
      <main class="editor-content-grid">
        <!-- Left Column: Title, Excerpt, Content Markdown/Rich Area -->
        <div class="editor-primary-col">
          <!-- Post Title Input -->
          <div class="form-group-field">
            <label class="field-label-mono">
              TIÊU ĐỀ BÀI VIẾT <span class="text-danger">*</span>
            </label>
            <input v-model="form.title" type="text" placeholder="Nhập tiêu đề bài viết ấn tượng..."
              class="input-title-large" @input="onTitleChange" />
          </div>

          <!-- Slug Row with Auto-Generate Helper -->
          <div class="form-slug-row">
            <span class="slug-prefix">
              <q-icon name="fa-solid fa-link" size="11px" class="q-mr-xs" />
              Slug: /posts/
            </span>
            <input v-model="form.slug" type="text" placeholder="duong-dan-bai-viet" class="input-slug-field" />
            <button type="button" class="btn-regenerate-slug" title="Tạo lại slug tự động theo tiêu đề"
              @click="regenerateSlug">
              <q-icon name="fa-solid fa-rotate-right" size="11px" />
              <span>Tạo lại</span>
            </button>
          </div>

          <!-- Excerpt Field -->
          <div class="form-group-field">
            <label class="field-label-mono">
              TÓM TẮT BÀI VIẾT (EXCERPT)
            </label>
            <textarea v-model="form.excerpt" rows="2"
              placeholder="Nhập 1 - 2 câu tóm tắt nội dung hấp dẫn hiển thị trên trang chủ và SEO..."
              class="textarea-excerpt"></textarea>
          </div>

          <!-- Content Editor Toolbar & Textarea -->
          <div class="content-editor-wrapper">
            <div class="editor-toolbar">
              <span class="toolbar-title">
                <q-icon name="fa-solid fa-code" size="12px" class="q-mr-xs" />
                NỘI DUNG (MARKDOWN / HTML)
              </span>

              <div class="toolbar-actions">
                <button type="button" class="tool-btn" title="Tiêu đề H2" @click="insertText('## ', '')">
                  H2
                </button>
                <button type="button" class="tool-btn" title="Tiêu đề H3" @click="insertText('### ', '')">
                  H3
                </button>
                <button type="button" class="tool-btn" title="In đậm" @click="insertText('**', '**')">
                  <q-icon name="fa-solid fa-bold" size="11px" />
                </button>
                <button type="button" class="tool-btn" title="In nghiêng" @click="insertText('*', '*')">
                  <q-icon name="fa-solid fa-italic" size="11px" />
                </button>
                <button type="button" class="tool-btn" title="Khối mã code" @click="insertText('```csharp\n', '\n```')">
                  <q-icon name="fa-solid fa-code" size="11px" />
                </button>
                <button type="button" class="tool-btn" title="Trích dẫn" @click="insertText('> ', '')">
                  <q-icon name="fa-solid fa-quote-left" size="11px" />
                </button>
                <button type="button" class="tool-btn" title="Chèn liên kết" @click="insertText('[Tên link](', ')')">
                  <q-icon name="fa-solid fa-link" size="11px" />
                </button>
              </div>
            </div>

            <textarea ref="contentInputRef" v-model="form.content"
              placeholder="Bắt đầu viết nội dung bài viết kỹ thuật của bạn ở đây... Hỗ trợ Markdown, code block, hình ảnh và HTML."
              class="textarea-content-body"></textarea>
          </div>
        </div>

        <!-- Right Column: Settings, Category, Tags & Thumbnail Preview -->
        <div class="editor-sidebar-col">
          <!-- 1. Category Selection -->
          <div class="sidebar-box">
            <div class="sidebar-box-header">
              <q-icon name="fa-solid fa-layer-group" size="13px" class="header-icon" />
              <span>CHỦ ĐỀ & DANH MỤC <span class="text-danger">*</span></span>
            </div>
            <div class="sidebar-box-body">
              <select v-model="form.categoryId" class="custom-select-field">
                <option :value="0" disabled>-- Chọn danh mục bài viết --</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }}
                </option>
              </select>
            </div>
          </div>

          <!-- 2. Tags Selector -->
          <div class="sidebar-box">
            <div class="sidebar-box-header">
              <q-icon name="fa-solid fa-tags" size="13px" class="header-icon" />
              <span>THẺ BÀI VIẾT (TAGS)</span>
            </div>
            <div class="sidebar-box-body">
              <div class="tag-input-box">
                <input v-model="tagInput" type="text" placeholder="Nhập tên thẻ rồi ấn Enter..." class="input-tag"
                  @keydown.enter.prevent="addTag" />
                <button type="button" class="btn-add-tag" @click="addTag">
                  <q-icon name="fa-solid fa-plus" size="10px" />
                </button>
              </div>

              <!-- Render Tags List -->
              <div v-if="form.tagNames.length > 0" class="tags-pill-list">
                <span v-for="(tag, tIdx) in form.tagNames" :key="tIdx" class="tag-pill-item">
                  <span>#{{ tag }}</span>
                  <button type="button" class="btn-remove-tag" @click="removeTag(tIdx)">
                    <q-icon name="fa-solid fa-xmark" size="9px" />
                  </button>
                </span>
              </div>

              <!-- Suggested tags -->
              <div v-if="suggestedTags.length > 0" class="suggested-tags-row">
                <span class="suggest-label">Gợi ý:</span>
                <button v-for="sTag in suggestedTags" :key="sTag.id" type="button" class="suggest-tag-btn"
                  @click="addSuggestedTag(sTag.name)">
                  +{{ sTag.name }}
                </button>
              </div>
            </div>
          </div>

          <!-- 3. Thumbnail Preview -->
          <div class="sidebar-box">
            <div class="sidebar-box-header">
              <q-icon name="fa-solid fa-image" size="13px" class="header-icon" />
              <span>ẢNH ĐẠI DIỆN (THUMBNAIL)</span>
            </div>
            <div class="sidebar-box-body">
              <input v-model="form.thumbnailUrl" type="text" placeholder="https://images.unsplash.com/..."
                class="input-thumbnail-url" />

              <div class="thumbnail-preview-frame">
                <img v-if="form.thumbnailUrl" :src="form.thumbnailUrl" alt="Thumbnail Preview" class="preview-img"
                  @error="onImageError" />
                <div v-else class="preview-placeholder">
                  <q-icon name="fa-regular fa-image" size="24px" />
                  <span>Chưa có ảnh đại diện</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from "vue";
import type { AdminPostDetail, AdminPostSummary, AdminCategorySummary, AdminTagSummary, CreatePostPayload, UpdatePostPayload } from "@/types/admin-post";
import { adminPostService } from "@/services/admin-post.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  post?: AdminPostDetail | AdminPostSummary | null;
  categories: AdminCategorySummary[];
  availableTags?: AdminTagSummary[];
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", post: AdminPostDetail): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.post && props.post.id > 0);
const submitting = ref(false);
const tagInput = ref("");
const contentInputRef = ref<HTMLTextAreaElement | null>(null);

const form = reactive<{
  title: string;
  slug: string;
  excerpt: string;
  thumbnailUrl: string;
  status: "Draft" | "Published";
  categoryId: number;
  content: string;
  tagNames: string[];
}>({
  title: "",
  slug: "",
  excerpt: "",
  thumbnailUrl: "",
  status: "Draft",
  categoryId: 0,
  content: "",
  tagNames: []
});

// Helper create slug in JS
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

function onTitleChange() {
  if (!isEdit.value || !form.slug) {
    form.slug = slugify(form.title);
  }
}

function regenerateSlug() {
  form.slug = slugify(form.title || "bai-viet");
}

function addTag() {
  const t = tagInput.value.trim().replace(/^#/, "");
  if (t && !form.tagNames.includes(t)) {
    form.tagNames.push(t);
  }
  tagInput.value = "";
}

function addSuggestedTag(name: string) {
  if (!form.tagNames.includes(name)) {
    form.tagNames.push(name);
  }
}

function removeTag(index: number) {
  form.tagNames.splice(index, 1);
}

const suggestedTags = computed(() => {
  if (!props.availableTags) return [];
  return props.availableTags.filter((t) => !form.tagNames.includes(t.name)).slice(0, 6);
});

function insertText(before: string, after: string) {
  const textarea = contentInputRef.value;
  if (!textarea) return;

  const start = textarea.selectionStart;
  const end = textarea.selectionEnd;
  const selected = form.content.substring(start, end);
  const replacement = `${before}${selected}${after}`;

  form.content = form.content.substring(0, start) + replacement + form.content.substring(end);

  setTimeout(() => {
    textarea.focus();
    textarea.setSelectionRange(start + before.length, start + before.length + selected.length);
  }, 50);
}

function onImageError(e: Event) {
  (e.target as HTMLImageElement).src =
    "https://images.unsplash.com/photo-1499750310107-5fef28a66643?w=800&auto=format&fit=crop&q=60";
}

// Watch dialog opening to load post data
watch(
  () => props.modelValue,
  async (newVal) => {
    if (newVal) {
      if (props.post && props.post.id > 0) {
        // Load full detail if needed
        try {
          const detail = await adminPostService.getPostById(props.post.id);
          form.title = detail.title;
          form.slug = detail.slug;
          form.excerpt = detail.excerpt || "";
          form.thumbnailUrl = detail.thumbnailUrl || "";
          form.status = (detail.status === "Published" ? "Published" : "Draft") as "Draft" | "Published";
          form.categoryId = detail.category?.id || (detail as any).categoryId || 0;
          form.content = detail.content || "";
          form.tagNames = detail.tags?.map((t) => t.name) || [];
        } catch {
          // Fallback to shallow post data
          form.title = props.post.title;
          form.slug = props.post.slug;
          form.excerpt = props.post.excerpt || "";
          form.thumbnailUrl = props.post.thumbnailUrl || "";
          form.status = props.post.status === "Published" ? "Published" : "Draft";
          form.categoryId = (props.post as any).categoryId || (props.post as any).category?.id || 0;
          form.content = (props.post as any).content || "";
          form.tagNames = Array.isArray(props.post.tags) ? props.post.tags.map((t: any) => (typeof t === "string" ? t : t.name)) : [];
        }
      } else {
        // Reset form for create
        form.title = "";
        form.slug = "";
        form.excerpt = "";
        form.thumbnailUrl = "";
        form.status = "Draft";
        form.categoryId = props.categories.length > 0 && props.categories[0] ? props.categories[0].id : 0;
        form.content = "";
        form.tagNames = [];
      }
    }
  }
);

async function handleCancel() {
  if (form.title || form.content) {
    const ok = await swalConfirm({
      title: "Đóng trình soạn thảo?",
      text: "Các thay đổi chưa lưu sẽ bị mất. Bạn có chắc chắn muốn thoát?",
      confirmButtonText: "Thoát không lưu",
      cancelButtonText: "Tiếp tục viết",
      icon: "warning"
    });
    if (!ok) return;
  }
  isOpen.value = false;
}

async function handleSubmit() {
  if (!form.title.trim()) {
    swalError("Thiếu tiêu đề bài viết", "Vui lòng nhập tiêu đề cho bài viết trước khi lưu.");
    return;
  }

  if (form.categoryId <= 0) {
    swalError("Chưa chọn danh mục", "Vui lòng chọn danh mục phù hợp cho bài viết.");
    return;
  }

  try {
    submitting.value = true;
    let savedPost: AdminPostDetail;

    if (isEdit.value && props.post) {
      const payload: UpdatePostPayload = {
        title: form.title.trim(),
        slug: form.slug.trim(),
        content: form.content,
        excerpt: form.excerpt.trim(),
        thumbnailUrl: form.thumbnailUrl.trim(),
        status: form.status,
        categoryId: form.categoryId,
        tagNames: form.tagNames
      };
      savedPost = await adminPostService.updatePost(props.post.id, payload);
      swalSuccess("Cập nhật bài viết thành công!");
    } else {
      const payload: CreatePostPayload = {
        title: form.title.trim(),
        slug: form.slug.trim(),
        content: form.content,
        excerpt: form.excerpt.trim(),
        thumbnailUrl: form.thumbnailUrl.trim(),
        status: form.status,
        categoryId: form.categoryId,
        tagNames: form.tagNames
      };
      savedPost = await adminPostService.createPost(payload);
      swalSuccess("Tạo bài viết mới thành công!");
    }

    emit("saved", savedPost);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã có lỗi xảy ra khi lưu bài viết. Vui lòng thử lại.";
    swalError("Lỗi lưu bài viết", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-post-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.editor-modal-container {
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  width: 1180px;
  max-width: 95vw;
  height: 88vh;
  max-height: 88vh;
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(0, 0, 0, 0.06);
  overflow: hidden;
}

/* 1. Top Header */
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
        max-width: 450px;
      }
    }
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: 12px;

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
        font-size: 12px;
        font-weight: 600;
        color: #64748b;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        gap: 6px;
        transition: all 0.2s ease;

        &.active {
          background: #ffffff;
          color: #0b1326;
          font-weight: 700;
          box-shadow: 0 2px 6px rgba(0, 0, 0, 0.06);
        }
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

/* 2. Grid Layout */
.editor-content-grid {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 20px;
  padding: 20px 24px;
  flex: 1;
  overflow-y: auto;
}

/* Primary Column */
.editor-primary-col {
  display: flex;
  flex-direction: column;
  gap: 16px;

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

  .input-title-large {
    width: 100%;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 12px 18px;
    font-family: var(--font-headline, sans-serif);
    font-size: 18px;
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

      &:hover {
        background: #e2e8f0;
        color: #0b1326;
      }
    }
  }

  .textarea-excerpt {
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

  .content-editor-wrapper {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    display: flex;
    flex-direction: column;
    flex: 1;
    min-height: 400px;
    overflow: hidden;

    .editor-toolbar {
      background: #f8fafc;
      border-bottom: 1px solid #e2e8f0;
      padding: 8px 14px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 10px;
      flex-wrap: wrap;

      .toolbar-title {
        font-family: var(--font-mono, monospace);
        font-size: 11px;
        font-weight: 700;
        color: #64748b;
        letter-spacing: 0.05em;
      }

      .toolbar-actions {
        display: flex;
        align-items: center;
        gap: 6px;

        .tool-btn {
          background: #ffffff;
          border: 1px solid #cbd5e1;
          border-radius: 6px;
          height: 28px;
          min-width: 28px;
          padding: 0 8px;
          font-size: 11.5px;
          font-weight: 700;
          color: #475569;
          cursor: pointer;
          display: inline-flex;
          align-items: center;
          justify-content: center;
          transition: all 0.15s ease;

          &:hover {
            border-color: #df266a;
            color: #df266a;
            background: #fdf2f6;
          }
        }
      }
    }

    .textarea-content-body {
      width: 100%;
      flex: 1;
      min-height: 380px;
      border: none;
      outline: none;
      padding: 16px 20px;
      font-family: var(--font-mono, monospace);
      font-size: 13.5px;
      line-height: 1.7;
      color: #0b1326;
      resize: none;
      background: #ffffff;
    }
  }
}

/* Sidebar Column */
.editor-sidebar-col {
  display: flex;
  flex-direction: column;
  gap: 16px;

  .sidebar-box {
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    overflow: hidden;

    .sidebar-box-header {
      background: #f8fafc;
      border-bottom: 1px solid #e2e8f0;
      padding: 10px 14px;
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 700;
      color: #475569;
      letter-spacing: 0.06em;
      display: flex;
      align-items: center;
      gap: 8px;

      .header-icon {
        color: #df266a;
      }

      .text-danger {
        color: #e11d48;
      }
    }

    .sidebar-box-body {
      padding: 14px;
      display: flex;
      flex-direction: column;
      gap: 10px;
    }
  }

  .custom-select-field {
    width: 100%;
    height: 38px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    padding: 0 10px;
    font-family: var(--font-headline, sans-serif);
    font-size: 13px;
    font-weight: 600;
    color: #0b1326;
    outline: none;

    &:focus {
      border-color: #df266a;
    }
  }

  .tag-input-box {
    display: flex;
    align-items: center;
    gap: 6px;

    .input-tag {
      flex: 1;
      height: 34px;
      background: #ffffff;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      padding: 0 10px;
      font-size: 12.5px;
      outline: none;

      &:focus {
        border-color: #df266a;
      }
    }

    .btn-add-tag {
      width: 34px;
      height: 34px;
      background: #eef2ff;
      border: 1px solid #c7d2fe;
      color: #4f46e5;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;

      &:hover {
        background: #4f46e5;
        color: #ffffff;
      }
    }
  }

  .tags-pill-list {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
    margin-top: 4px;

    .tag-pill-item {
      font-family: var(--font-mono, monospace);
      font-size: 11px;
      font-weight: 600;
      background: #fdf2f6;
      border: 1px solid #fce7f3;
      color: #df266a;
      padding: 3px 8px;
      border-radius: 9999px;
      display: inline-flex;
      align-items: center;
      gap: 6px;

      .btn-remove-tag {
        background: transparent;
        border: none;
        color: #be185d;
        cursor: pointer;
        padding: 0;
        display: flex;
        align-items: center;
      }
    }
  }

  .suggested-tags-row {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: 6px;
    margin-top: 6px;

    .suggest-label {
      font-size: 11px;
      color: #64748b;
    }

    .suggest-tag-btn {
      background: #f1f5f9;
      border: 1px dashed #cbd5e1;
      border-radius: 4px;
      padding: 2px 6px;
      font-size: 10.5px;
      color: #475569;
      cursor: pointer;

      &:hover {
        border-color: #df266a;
        color: #df266a;
      }
    }
  }

  .input-thumbnail-url {
    width: 100%;
    height: 36px;
    background: #ffffff;
    border: 1px solid #e2e8f0;
    border-radius: 8px;
    padding: 0 10px;
    font-size: 12.5px;
    outline: none;

    &:focus {
      border-color: #df266a;
    }
  }

  .thumbnail-preview-frame {
    width: 100%;
    height: 160px;
    border-radius: 8px;
    background: #f1f5f9;
    border: 1px dashed #cbd5e1;
    overflow: hidden;
    display: flex;
    align-items: center;
    justify-content: center;

    .preview-img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .preview-placeholder {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 6px;
      color: #94a3b8;
      font-size: 12px;
      font-weight: 500;
    }
  }
}

@media (max-width: 1000px) {
  .editor-content-grid {
    grid-template-columns: 1fr;
  }
}
</style>
