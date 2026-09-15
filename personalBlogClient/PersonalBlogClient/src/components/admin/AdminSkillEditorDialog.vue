<template>
  <q-dialog v-model="isOpen" persistent transition-show="scale" transition-hide="scale"
    class="admin-skill-editor-dialog">
    <div class="skill-modal-container">
      <!-- 1. Header Bar -->
      <header class="editor-header-bar">
        <div class="header-left">
          <button type="button" class="btn-back-circle" title="Đóng hộp thoại" @click="handleCancel">
            <q-icon name="fa-solid fa-xmark" size="14px" />
          </button>
          <!-- <div class="header-title-box">
            <span class="editor-kicker">
              {{ isEdit ? 'CHỈNH SỬA KỸ NĂNG CHUYÊN MÔN' : 'THÊM KỸ NĂNG MỚI' }}
            </span>
            <h3 class="editor-main-title">{{ form.name ? form.name : 'Chưa đặt tên kỹ năng...' }}</h3>
          </div> -->
        </div>

        <div class="header-actions">
          <button type="button" class="btn-cancel-flat" @click="handleCancel">
            Hủy bỏ
          </button>
          <button type="button" class="btn-save-primary" :disabled="submitting" @click="handleSubmit">
            <q-spinner v-if="submitting" size="14px" color="white" />
            <q-icon v-else name="fa-solid fa-check" size="13px" />
            <span>{{ isEdit ? 'Lưu thay đổi' : 'Tạo kỹ năng' }}</span>
          </button>
        </div>
      </header>

      <!-- 2. Modal Body Form -->
      <main class="skill-form-body">
        <div class="form-row-2col">
          <!-- Skill Name -->
          <div class="form-group-field">
            <label class="field-label-mono">
              TÊN KỸ NĂNG <span class="text-danger">*</span>
            </label>
            <input v-model="form.name" type="text" placeholder="Ví dụ: Vue.js, ASP.NET Core, PostgreSQL..."
              class="input-custom-field" />
          </div>

          <!-- Category -->
          <div class="form-group-field">
            <label class="field-label-mono">
              NHÓM PHÂN LOẠI <span class="text-danger">*</span>
            </label>
            <select v-model="form.category" class="select-custom-field">
              <option value="Frontend">Frontend Development</option>
              <option value="Backend">Backend Development</option>
              <option value="Database">Database & Cache</option>
              <option value="DevOps">DevOps & Cloud</option>
              <option value="Tools">Tools & Architecture</option>
            </select>
          </div>
        </div>

        <!-- Proficiency Slider & Number Input -->
        <div class="form-group-field">
          <div class="proficiency-header-row">
            <label class="field-label-mono">MỨC ĐỘ THÀNH THẠO (PROFICIENCY)</label>
            <span class="proficiency-val-badge">{{ form.proficiency }}%</span>
          </div>

          <div class="slider-wrap">
            <input v-model.number="form.proficiency" type="range" min="10" max="100" step="5" class="range-slider" />
          </div>
        </div>

        <div class="form-row-2col">
          <!-- Icon Class -->
          <div class="form-group-field">
            <label class="field-label-mono">
              ICON CLASS (FONTAWESOME)
            </label>
            <input v-model="form.icon" type="text" placeholder="Ví dụ: fa-brands fa-vuejs, fa-solid fa-database"
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

        <!-- Live Skill Preview Card -->
        <div class="skill-preview-card">
          <div class="preview-card-title">
            <q-icon name="fa-solid fa-eye" size="12px" class="q-mr-xs text-rose" />
            XEM TRƯỚC HIỂN THỊ
          </div>
          <div class="preview-skill-box">
            <div class="skill-icon-box">
              <q-icon :name="resolveSkillIcon({ name: form.name, icon: form.icon })" size="20px" class="preview-icon" />
            </div>
            <div class="skill-info-box">
              <div class="skill-name-row">
                <span class="preview-skill-name">{{ form.name || 'Tên kỹ năng' }}</span>
                <span class="preview-percent">{{ form.proficiency }}%</span>
              </div>
              <div class="preview-progress-bar">
                <div class="progress-fill" :style="{ width: `${form.proficiency}%` }"></div>
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
import type { AdminSkill, CreateSkillPayload, UpdateSkillPayload } from "@/types/admin-profile";
import { adminProfileService } from "@/services/admin-profile.service";
import { swalConfirm, swalSuccess, swalError } from "@/utils/swal";
import { resolveSkillIcon } from "@/utils/skill-icon";

const props = defineProps<{
  modelValue: boolean;
  skill?: AdminSkill | null;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", val: boolean): void;
  (e: "saved", skill: AdminSkill): void;
}>();

const isOpen = computed({
  get: () => props.modelValue,
  set: (val: boolean) => emit("update:modelValue", val)
});

const isEdit = computed(() => !!props.skill && props.skill.id > 0);
const submitting = ref(false);

const form = reactive<{
  name: string;
  category: string;
  proficiency: number;
  icon: string;
  displayOrder: number;
}>({
  name: "",
  category: "Frontend",
  proficiency: 80,
  icon: "fa-solid fa-code",
  displayOrder: 0
});

watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal) {
      if (props.skill && props.skill.id > 0) {
        form.name = props.skill.name;
        form.category = props.skill.category;
        form.proficiency = props.skill.proficiency;
        form.icon = props.skill.icon || "fa-solid fa-code";
        form.displayOrder = props.skill.displayOrder;
      } else {
        form.name = "";
        form.category = "Frontend";
        form.proficiency = 80;
        form.icon = "fa-solid fa-code";
        form.displayOrder = 0;
      }
    }
  }
);

async function handleCancel() {
  if (form.name.trim()) {
    const ok = await swalConfirm({
      title: "Hủy thay đổi?",
      text: "Thông tin kỹ năng bạn vừa nhập sẽ không được lưu lại. Bạn có chắc muốn thoát?",
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
    swalError("Thiếu tên kỹ năng", "Vui lòng nhập tên kỹ năng trước khi lưu.");
    return;
  }

  try {
    submitting.value = true;
    let savedSkill: AdminSkill;

    if (isEdit.value && props.skill) {
      const payload: UpdateSkillPayload = {
        name: form.name.trim(),
        category: form.category,
        proficiency: form.proficiency,
        icon: form.icon.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedSkill = await adminProfileService.updateSkill(props.skill.id, payload);
      swalSuccess("Cập nhật thành công!", `Kỹ năng "${savedSkill.name}" đã được cập nhật.`);
    } else {
      const payload: CreateSkillPayload = {
        name: form.name.trim(),
        category: form.category,
        proficiency: form.proficiency,
        icon: form.icon.trim() || undefined,
        displayOrder: form.displayOrder
      };
      savedSkill = await adminProfileService.createSkill(payload);
      swalSuccess("Thêm mới thành công!", `Kỹ năng "${savedSkill.name}" đã được tạo.`);
    }

    emit("saved", savedSkill);
    isOpen.value = false;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Đã có lỗi xảy ra khi lưu kỹ năng.";
    swalError("Lỗi lưu kỹ năng", msg);
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.admin-skill-editor-dialog {
  z-index: 6000;

  :deep(.q-dialog__inner) {
    padding: 24px 16px;
  }
}

.skill-modal-container {
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
.skill-form-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 18px;
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
}

.input-custom-field,
.select-custom-field {
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

  &:focus {
    border-color: #df266a;
    box-shadow: 0 0 0 3px rgba(223, 38, 106, 0.1);
  }
}

.proficiency-header-row {
  display: flex;
  align-items: center;
  justify-content: space-between;

  .proficiency-val-badge {
    font-family: var(--font-mono, monospace);
    font-size: 12px;
    font-weight: 800;
    color: #059669;
    background: #ecfdf5;
    border: 1px solid #a7f3d0;
    padding: 2px 8px;
    border-radius: 6px;
  }
}

.slider-wrap {
  margin-top: 4px;

  .range-slider {
    width: 100%;
    accent-color: #df266a;
    cursor: pointer;
  }
}

/* Preview Card */
.skill-preview-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;

  .preview-card-title {
    font-family: var(--font-mono, monospace);
    font-size: 10.5px;
    font-weight: 700;
    color: #64748b;
    letter-spacing: 0.05em;
  }

  .preview-skill-box {
    display: flex;
    align-items: center;
    gap: 14px;
    background: #f8fafc;
    border: 1px solid #e2e8f0;
    border-radius: 10px;
    padding: 12px 16px;

    .skill-icon-box {
      width: 40px;
      height: 40px;
      border-radius: 10px;
      background: #ffffff;
      border: 1px solid #e2e8f0;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #df266a;
      flex-shrink: 0;
    }

    .skill-info-box {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 6px;

      .skill-name-row {
        display: flex;
        align-items: center;
        justify-content: space-between;

        .preview-skill-name {
          font-family: var(--font-headline, sans-serif);
          font-size: 14px;
          font-weight: 700;
          color: #0b1326;
        }

        .preview-percent {
          font-family: var(--font-mono, monospace);
          font-size: 12px;
          font-weight: 700;
          color: #64748b;
        }
      }

      .preview-progress-bar {
        width: 100%;
        height: 6px;
        background: #e2e8f0;
        border-radius: 9999px;
        overflow: hidden;

        .progress-fill {
          height: 100%;
          background: linear-gradient(90deg, #df266a, #4f46e5);
          border-radius: 9999px;
          transition: width 0.3s ease;
        }
      }
    }
  }
}
</style>
