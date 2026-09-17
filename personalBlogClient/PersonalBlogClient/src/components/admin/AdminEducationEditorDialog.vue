<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale"
    class="admin-education-editor-dialog">
    <div class="edu-modal-container">
      <!-- 1. Header Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA HỌC VẤN & BẰNG CẤP' : 'THÊM HỌC VẤN MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.degree ? `${form.degree} - ${form.institution}` : 'Chưa đặt bằng cấp...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <button type="button" class="btn-cancel-flat" @click="handleCancel">
            Hủy bỏ
          </button>
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo học vấn' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Modal Body Form -->
      <main class="edu-form-body">
        <div class="form-row-2col">
          <!-- Institution -->
          <div class="form-group-field">
            <label class="field-label-mono">
              TRƯỜNG / CƠ SỞ ĐÀO TẠO <span class="text-danger">*</span>
            </label>
            <input v-model="form.institution" type="text" placeholder="Ví dụ: Đại học Bách Khoa, FPT University..."
              class="input-custom-field" />
          </div>

          <!-- Degree -->
          <div class="form-group-field">
            <label class="field-label-mono">
              BẰNG CẤP / CHUYÊN NGÀNH <span class="text-danger">*</span>
            </label>
            <input v-model="form.degree" type="text" placeholder="Ví dụ: Kỹ sư Công nghệ Thông tin..."
              class="input-custom-field" />
          </div>
        </div>

        <div class="form-row-3col">
          <!-- Start Year -->
          <div class="form-group-field">
            <label class="field-label-mono">
              NĂM BẮT ĐẦU
            </label>
            <input v-model="form.startYear" type="text" placeholder="2019" class="input-custom-field" />
          </div>

          <!-- End Year -->
          <div class="form-group-field">
            <label class="field-label-mono">
              NĂM TỐT NGHIỆP
            </label>
            <input v-model="form.endYear" type="text" placeholder="2023" class="input-custom-field" />
          </div>

          <!-- Display Order -->
          <div class="form-group-field">
            <label class="field-label-mono">
              THỨ TỰ
            </label>
            <input v-model.number="form.displayOrder" type="number" min="0" placeholder="0"
              class="input-custom-field" />
          </div>
        </div>

        <!-- Description -->
        <div class="form-group-field">
          <label class="field-label-mono">
            MÔ TẢ THÀNH TÍCH / XẾP LOẠI TỐT NGHIỆP
          </label>
          <textarea v-model="form.description" rows="3"
            placeholder="Ví dụ: Tốt nghiệp loại Giỏi, GPA 3.6/4.0, Đạt giải Nhất cuộc thi Nghiên cứu Khoa học Sinh viên..."
            class="textarea-custom-field"></textarea>
        </div>
      </main>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from "vue";
import type { AdminEducation, CreateEducationPayload, UpdateEducationPayload } from "@/types/admin-profile";
import { adminProfileService } from "@/services/admin-profile.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  education?: AdminEducation | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", education: AdminEducation): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.education && props.education.id > 0);
const submitting = ref(false);

const form = reactive<{
  institution: string;
  degree: string;
  startYear: string;
  endYear: string;
  description: string;
  displayOrder: number;
}>({
  institution: "",
  degree: "",
  startYear: "",
  endYear: "",
  description: "",
  displayOrder: 0
});

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal) {
      if (props.education && props.education.id > 0) {
        form.institution = props.education.institution;
        form.degree = props.education.degree;
        form.startYear = props.education.startYear || "";
        form.endYear = props.education.endYear || "";
        form.description = props.education.description || "";
        form.displayOrder = props.education.displayOrder;
      } else {
        form.institution = "";
        form.degree = "";
        form.startYear = "";
        form.endYear = "";
        form.description = "";
        form.displayOrder = 0;
      }
    }
  }
);

async function handleCancel() {
  if (form.institution.trim() || form.degree.trim()) {
    const ok = await swalConfirm({
      title: "Hủy thay đổi?",
      text: "Thông tin học vấn bạn vừa nhập sẽ không được lưu lại. Bạn có chắc muốn thoát?",
      confirmButtonText: "Thoát không lưu",
      cancelButtonText: "Tiếp tục chỉnh sửa",
      icon: "warning"
    });
    if (!ok) return;
  }
  isOpen.value = false;
}

async function handleSubmit() {
  if (!form.institution.trim()) {
    swalError("Thiếu tên trường học", "Vui lòng nhập tên trường hoặc cơ sở đào tạo.");
    return;
  }

  if (!form.degree.trim()) {
    swalError("Thiếu bằng cấp", "Vui lòng nhập văn bằng hoặc chuyên ngành tốt nghiệp.");
    return;
  }

  try {
    submitting.value = true;
    let savedEdu: AdminEducation;

    if (isEdit.value && props.education) {
      const payload: UpdateEducationPayload = {
        institution: form.institution.trim(),
        degree: form.degree.trim(),
        startYear: form.startYear.trim() || undefined,
        endYear: form.endYear.trim() || undefined,
        description: form.description.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedEdu = await adminProfileService.updateEducation(props.education.id, payload);
      swalSuccess("Cập nhật thành công!", `Học vấn tại "${savedEdu.institution}" đã được cập nhật.`);
    } else {
      const payload: CreateEducationPayload = {
        institution: form.institution.trim(),
        degree: form.degree.trim(),
        startYear: form.startYear.trim() || undefined,
        endYear: form.endYear.trim() || undefined,
        description: form.description.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedEdu = await adminProfileService.createEducation(payload);
      swalSuccess("Thêm mới thành công!", `Học vấn tại "${savedEdu.institution}" đã được tạo.`);
    }

    emit("saved", savedEdu);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã có lỗi xảy ra khi lưu học vấn.";
    swalError("Lỗi lưu học vấn", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-education-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.edu-modal-container {
  background: #f8fafc;
  display: flex;
  flex-direction: column;
  width: 580px;
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
        max-width: 280px;
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
.edu-form-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  overflow-y: auto;
}

.form-row-2col {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-row-3col {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 16px;
}

.form-group-field {
  display: flex;
  flex-direction: column;
  gap: 6px;

  .field-label-mono {
    font-family: var(--font-mono, monospace);
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.05em;
    color: #475569;

    .text-danger {
      color: #e11d48;
    }
  }
}

.input-custom-field {
  width: 100%;
  background: var(--bg-surface-lowest, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 10px;
  padding: 10px 14px;
  font-family: var(--font-headline, sans-serif);
  font-size: 13.5px;
  color: var(--text-primary, #0b1326);
  outline: none;
  transition: all 0.2s ease;

  &:focus {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }
}

.textarea-custom-field {
  width: 100%;
  background: var(--bg-surface-lowest, #ffffff);
  border: 1px solid var(--border-hairline, #e2e8f0);
  border-radius: 10px;
  padding: 12px 14px;
  font-family: var(--font-body, sans-serif);
  font-size: 13.5px;
  line-height: 1.5;
  color: var(--text-primary, #0b1326);
  outline: none;
  resize: vertical;
  transition: all 0.2s ease;

  &:focus {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }
}
</style>
