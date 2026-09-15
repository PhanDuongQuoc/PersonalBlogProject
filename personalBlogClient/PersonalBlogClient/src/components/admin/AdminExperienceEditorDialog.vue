<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale"
    class="admin-experience-editor-dialog">
    <div class="exp-modal-container">
      <!-- 1. Header Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA KINH NGHIỆM LÀM VIỆC' : 'THÊM KINH NGHIỆM MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.role ? `${form.role} @ ${form.company}` : 'Chưa đặt chức danh...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <button type="button" class="btn-cancel-flat" @click="handleCancel">
            Hủy bỏ
          </button>
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo kinh nghiệm' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Modal Body Form -->
      <main class="exp-form-body">
        <div class="form-row-2col">
          <!-- Role -->
          <div class="form-group-field">
            <label class="field-label-mono">
              VỊ TRÍ / CHỨC DANH <span class="text-danger">*</span>
            </label>
            <input v-model="form.role" type="text" placeholder="Ví dụ: Senior Frontend Engineer, Fullstack Developer..."
              class="input-custom-field" />
          </div>

          <!-- Company -->
          <div class="form-group-field">
            <label class="field-label-mono">
              CÔNG TY / TỔ CHỨC <span class="text-danger">*</span>
            </label>
            <input v-model="form.company" type="text" placeholder="Ví dụ: FPT Software, VNG Corporation..."
              class="input-custom-field" />
          </div>
        </div>

        <div class="form-row-2col">
          <!-- Location -->
          <div class="form-group-field">
            <label class="field-label-mono">
              ĐỊA ĐIỂM
            </label>
            <input v-model="form.location" type="text" placeholder="Ví dụ: Hồ Chí Minh, Việt Nam (Hybrid)"
              class="input-custom-field" />
          </div>

          <!-- Display Order -->
          <div class="form-group-field">
            <label class="field-label-mono">
              THỨ TỰ HIỂN THỊ
            </label>
            <input v-model.number="form.displayOrder" type="number" min="0" placeholder="0"
              class="input-custom-field" />
          </div>
        </div>

        <!-- Date Range & IsCurrent -->
        <div class="date-range-box">
          <div class="form-row-2col">
            <div class="form-group-field">
              <label class="field-label-mono">
                THỜI GIAN BẮT ĐẦU <span class="text-danger">*</span>
              </label>
              <input v-model="form.startDate" type="text" placeholder="Ví dụ: 01/2023 hoặc 2023"
                class="input-custom-field" />
            </div>

            <div class="form-group-field">
              <label class="field-label-mono">
                THỜI GIAN KẾT THÚC
              </label>
              <input v-model="form.endDate" type="text" placeholder="Ví dụ: 12/2024 hoặc Để trống nếu đang làm"
                class="input-custom-field" :disabled="form.isCurrent" />
            </div>
          </div>

          <!-- IsCurrent Checkbox -->
          <label class="current-job-checkbox-label">
            <input v-model="form.isCurrent" type="checkbox" class="checkbox-custom" />
            <span class="checkbox-text">Hiện tại tôi đang làm việc ở vị trí này</span>
          </label>
        </div>

        <!-- Technologies -->
        <div class="form-group-field">
          <label class="field-label-mono">
            CÔNG NGHỆ ÁP DỤNG (TECHNOLOGIES)
          </label>
          <input v-model="form.technologies" type="text"
            placeholder="Ví dụ: Vue 3, Quasar, TypeScript, ASP.NET Core, PostgreSQL, Docker"
            class="input-custom-field" />
          <span class="field-hint">Phân tách các công nghệ bằng dấu phẩy để hệ thống tự tạo badge hashtag</span>
        </div>

        <!-- Description -->
        <div class="form-group-field">
          <label class="field-label-mono">
            MÔ TẢ CÔNG VIỆC & THÀNH TỰU
          </label>
          <textarea v-model="form.description" rows="4"
            placeholder="Mô tả các trách nhiệm chính, các bài toán kỹ thuật đã giải quyết và các dự án tiêu biểu đã đóng góp..."
            class="textarea-custom-field"></textarea>
        </div>
      </main>
    </div>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from "vue";
import type { AdminExperience, CreateExperiencePayload, UpdateExperiencePayload } from "@/types/admin-profile";
import { adminProfileService } from "@/services/admin-profile.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";

const props = defineProps<{
  modelValue: boolean;
  experience?: AdminExperience | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", experience: AdminExperience): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.experience && props.experience.id > 0);
const submitting = ref(false);

const form = reactive<{
  role: string;
  company: string;
  location: string;
  startDate: string;
  endDate: string;
  isCurrent: boolean;
  description: string;
  technologies: string;
  displayOrder: number;
}>({
  role: "",
  company: "",
  location: "",
  startDate: "",
  endDate: "",
  isCurrent: false,
  description: "",
  technologies: "",
  displayOrder: 0
});

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal) {
      if (props.experience && props.experience.id > 0) {
        form.role = props.experience.role;
        form.company = props.experience.company;
        form.location = props.experience.location || "";
        form.startDate = props.experience.startDate;
        form.endDate = props.experience.endDate || "";
        form.isCurrent = props.experience.isCurrent;
        form.description = props.experience.description || "";
        form.technologies = props.experience.technologies || "";
        form.displayOrder = props.experience.displayOrder;
      } else {
        form.role = "";
        form.company = "";
        form.location = "";
        form.startDate = "";
        form.endDate = "";
        form.isCurrent = false;
        form.description = "";
        form.technologies = "";
        form.displayOrder = 0;
      }
    }
  }
);

async function handleCancel() {
  if (form.role.trim() || form.company.trim()) {
    const ok = await swalConfirm({
      title: "Hủy thay đổi?",
      text: "Thông tin kinh nghiệm bạn vừa nhập sẽ không được lưu lại. Bạn có chắc muốn thoát?",
      confirmButtonText: "Thoát không lưu",
      cancelButtonText: "Tiếp tục chỉnh sửa",
      icon: "warning"
    });
    if (!ok) return;
  }
  isOpen.value = false;
}

async function handleSubmit() {
  if (!form.role.trim()) {
    swalError("Thiếu vị trí / chức danh", "Vui lòng nhập vị trí chức danh công việc.");
    return;
  }

  if (!form.company.trim()) {
    swalError("Thiếu tên công ty", "Vui lòng nhập tên công ty hoặc tổ chức.");
    return;
  }

  if (!form.startDate.trim()) {
    swalError("Thiếu thời gian bắt đầu", "Vui lòng nhập thời gian bắt đầu làm việc.");
    return;
  }

  try {
    submitting.value = true;
    let savedExp: AdminExperience;

    if (isEdit.value && props.experience) {
      const payload: UpdateExperiencePayload = {
        role: form.role.trim(),
        company: form.company.trim(),
        location: form.location.trim() || undefined,
        startDate: form.startDate.trim(),
        endDate: form.isCurrent ? undefined : (form.endDate.trim() || undefined),
        isCurrent: form.isCurrent,
        description: form.description.trim() || undefined,
        technologies: form.technologies.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedExp = await adminProfileService.updateExperience(props.experience.id, payload);
      swalSuccess("Cập nhật thành công!", `Kinh nghiệm tại "${savedExp.company}" đã được cập nhật.`);
    } else {
      const payload: CreateExperiencePayload = {
        role: form.role.trim(),
        company: form.company.trim(),
        location: form.location.trim() || undefined,
        startDate: form.startDate.trim(),
        endDate: form.isCurrent ? undefined : (form.endDate.trim() || undefined),
        isCurrent: form.isCurrent,
        description: form.description.trim() || undefined,
        technologies: form.technologies.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedExp = await adminProfileService.createExperience(payload);
      swalSuccess("Thêm mới thành công!", `Kinh nghiệm tại "${savedExp.company}" đã được tạo.`);
    }

    emit("saved", savedExp);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã có lỗi xảy ra khi lưu kinh nghiệm.";
    swalError("Lỗi lưu kinh nghiệm", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-experience-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.exp-modal-container {
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
        max-width: 300px;
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
.exp-form-body {
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

  .field-hint {
    font-size: 11px;
    color: #94a3b8;
  }
}

.input-custom-field {
  width: 100%;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 10px 14px;
  font-family: var(--font-headline, sans-serif);
  font-size: 13.5px;
  color: #0b1326;
  outline: none;
  transition: all 0.2s ease;

  &:focus:not(:disabled) {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }

  &:disabled {
    background: #f8fafc;
    color: #94a3b8;
    cursor: not-allowed;
  }
}

.textarea-custom-field {
  width: 100%;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 12px 14px;
  font-family: var(--font-body, sans-serif);
  font-size: 13.5px;
  line-height: 1.5;
  color: #0b1326;
  outline: none;
  resize: vertical;
  transition: all 0.2s ease;

  &:focus {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }
}

.date-range-box {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;

  .current-job-checkbox-label {
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;

    .checkbox-custom {
      accent-color: #df266a;
      width: 16px;
      height: 16px;
      cursor: pointer;
    }

    .checkbox-text {
      font-family: var(--font-headline, sans-serif);
      font-size: 13px;
      font-weight: 600;
      color: #334155;
    }
  }
}
</style>
